using System.Text.Json.Serialization;
using ConcertAggregator.Models;
using Refit;

namespace ConcertAggregator.DAL;

// ---------- Языки ----------

public enum LanguageCode
{
    Kg = 1,
    Ru = 2,
    En = 3,
}

public interface ILibreTranslateApi
{
    [Post("/translate")]
    Task<TranslateResponse> TranslateAsync(
        [Body] TranslateRequest request,
        CancellationToken ct = default);

    [Get("/languages")]
    Task<List<LibreLanguageDto>> GetLanguagesAsync(CancellationToken ct = default);
}

public record TranslateRequest(
    [property: JsonPropertyName("q")] string Q,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("target")] string Target,
    [property: JsonPropertyName("format")] string Format = "text");

public record TranslateResponse(
    [property: JsonPropertyName("translatedText")] string TranslatedText);

public record LibreLanguageDto(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("name")] string Name);

// ---------- Сервис ----------

public class TranslationService(ILibreTranslateApi _api, ILogger<TranslationService> _logger)
{
    // Сопоставление доменных языков с ISO 639-1, которые понимает LibreTranslate.
    // Внимание: "Kg" здесь трактуется как кыргызский → ISO "ky".
    // Если в БД Language.Name = "Kg" и нужен именно Kongo (kg) — поменяй маппинг.
    private static readonly Dictionary<LanguageCode, string> Iso = new()
    {
        [LanguageCode.Kg] = "ky",
        [LanguageCode.Ru] = "ru",
        [LanguageCode.En] = "en",
    };

    public static string ToIso(LanguageCode code) => Iso[code];

    public static LanguageCode FromName(string name) =>
        Enum.TryParse<LanguageCode>(name, ignoreCase: true, out var code)
            ? code
            : throw new ArgumentException($"Unknown language name: {name}", nameof(name));

    /// <summary>Перевод текста с одного языка на другой. При ошибке возвращает исходный текст.</summary>
    public async Task<string> TranslateAsync(
        string text,
        LanguageCode from,
        LanguageCode to,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        if (from == to) return text;

        try
        {
            var response = await _api.TranslateAsync(
                new TranslateRequest(text, Iso[from], Iso[to]), ct);

            return string.IsNullOrWhiteSpace(response?.TranslatedText)
                ? text
                : response!.TranslatedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "LibreTranslate failed: {From} -> {To}", from, to);
            return text;
        }
    }

    /// <summary>Перевод с автоопределением языка-источника.</summary>
    public async Task<string> AutoTranslateAsync(
        string text,
        LanguageCode to,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        try
        {
            var response = await _api.TranslateAsync(
                new TranslateRequest(text, "auto", Iso[to]), ct);

            return string.IsNullOrWhiteSpace(response?.TranslatedText)
                ? text
                : response!.TranslatedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LibreTranslate auto -> {To} failed", to);
            return text;
        }
    }

    /// <summary>
    /// Перевести название и описание события из исходного языка на все целевые.
    /// Возвращает набор новых EventTranslation, готовых к сохранению.
    /// </summary>
    public async Task<IReadOnlyList<EventTranslation>> TranslateEventAsync(
        Event source,
        LanguageCode sourceLanguage,
        IEnumerable<Language> targetLanguages,
        CancellationToken ct = default)
    {
        var sourceTranslation = source.EventTranslations?
            .FirstOrDefault(t =>
                t.Language is not null &&
                string.Equals(t.Language.Name, sourceLanguage.ToString(),
                    StringComparison.OrdinalIgnoreCase));

        if (sourceTranslation is null)
            throw new InvalidOperationException(
                $"Source translation ({sourceLanguage}) not found for event {source.Id}");

        var tasks = targetLanguages
            .Select(target => TranslateOneAsync(source, sourceTranslation, sourceLanguage, target, ct))
            .ToList();

        var results = await Task.WhenAll(tasks);
        return results.Where(x => x is not null).Cast<EventTranslation>().ToList();
    }

    private async Task<EventTranslation?> TranslateOneAsync(
        Event source,
        EventTranslation sourceTranslation,
        LanguageCode sourceLanguage,
        Language targetLanguage,
        CancellationToken ct)
    {
        var target = FromName(targetLanguage.Name);
        if (target == sourceLanguage) return null;

        var nameTask = TranslateAsync(sourceTranslation.EventName, sourceLanguage, target, ct);
        var descTask = TranslateAsync(sourceTranslation.Description, sourceLanguage, target, ct);
        await Task.WhenAll(nameTask, descTask);

        return new EventTranslation
        {
            Event = source,
            Language = targetLanguage,
            EventName = Truncate(await nameTask, 128),
            Description = Truncate(await descTask, 512),
        };
    }

    private static string Truncate(string value, int max) =>
        string.IsNullOrEmpty(value) || value.Length <= max
            ? value
            : value[..max];
}