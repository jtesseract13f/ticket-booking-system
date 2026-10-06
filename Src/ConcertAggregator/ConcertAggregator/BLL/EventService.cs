using System.Globalization;
using ConcertAggregator.DAL;
using ConcertAggregator.DAL.Repositories;
using ConcertAggregator.DTO;
using ConcertAggregator.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.BLL;

public class EventService(
    NodeService _nodeService,
    NodeStrategy _nodeStrategy,
    EventRepository _eventRepository,
    TranslationService _translationService,
    NodeRepository _nodeRepository,
    ConcertDbContext _db,
    ILogger<EventService> _logger)
{
    public async Task<IEnumerable<EventDto>> GetAllEventsFiltered(
        Filter? filter,
        CancellationToken ct = default)
    {
        var events = await GetAllEvents(ct);
        return ApplyFilter(events, filter);
    }

    public static IEnumerable<EventDto> ApplyFilter(
        IEnumerable<EventDto> source,
        Filter? filter)
    {
        if (filter is null)
            return source;

        var query = source;

        if (!string.IsNullOrWhiteSpace(filter.EventType))
        {
            var type = filter.EventType.Trim();
            query = query.Where(e =>
                e.Name is not null &&
                e.Name.Contains(type, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.Price is int price)
        {
            query = query.Where(e => e.Cost.HasValue && e.Cost.Value <= price);
        }

        if (filter.DateFrom is DateTime from)
        {
            query = query.Where(e => e.StartDate.HasValue && e.StartDate.Value >= from);
        }

        if (filter.DateTo is DateTime to)
        {
            query = query.Where(e => e.StartDate.HasValue && e.StartDate.Value <= to);
        }

        return query;
    }
    
    public async Task<IEnumerable<EventDto>> GetAllEvents(CancellationToken ct = default)
    {
        var culture = CultureInfo.CurrentCulture;
        var targetCode = FromCulture(culture);
        var targetLanguage = await GetOrCreateLanguageAsync(targetCode, ct);

        _logger.LogInformation("Fetching events for culture {Culture} (lang={Lang})",
            culture.Name, targetLanguage.Name);

        var nodes = await _nodeService.GetAllNodes();
        var result = new List<EventDto>();

        foreach (var node in nodes)
        {
            if (node.IsBlocked) continue;
            IEnumerable<EventDto> externalEvents;
            try
            {
                externalEvents = await _nodeStrategy
                    .NodeApis[node.ApiType]
                    .GetAllEvents(new Uri(node.BaseUrl));
                _logger.LogError("Node {NodeId} ({ApiType}) here", node.Id, node.ApiType);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Node {NodeId} ({ApiType}) failed", node.Id, node.ApiType);
                continue; 
            }
            foreach (var ext in externalEvents)
            {
                var evt = await UpsertEventAsync(ext, node.Id, targetLanguage, ct);
                var translation = evt.EventTranslations
                    .FirstOrDefault(t => t.Language.Id == targetLanguage.Id);

                result.Add(new EventDto(
                    ExternalId: evt.Id.ToString(),
                    Cost: evt.Cost,
                    Place: evt.Place,
                    Name: translation?.EventName,
                    Description: translation?.Description,
                    StartDate: evt.StartDate,
                    Uri: evt.Uri));
            }
        }
        return result;
    }

    private static readonly TimeZoneInfo SourceTz =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Bishkek");

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc         => value,                            // уже UTC, ничего не делаем
            DateTimeKind.Local       => value.ToUniversalTime(),          // локальное время сервера
            DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeToUtc(value, SourceTz), // время Бишкека
            _                        => value
        };
    }
    
    private async Task<Event> UpsertEventAsync(
        EventDto ext, Guid nodeId, Language targetLanguage, CancellationToken ct)
    {
        var existing = await _eventRepository.GetEventById(Guid.Parse(ext.ExternalId));
        
        if (existing is null)
        {
            var node = await _nodeRepository.GetById(nodeId)
                       ?? throw new InvalidOperationException($"Node {nodeId} not found");

            var evt = new Event
            {
                Id = Guid.Parse(ext.ExternalId),
                Cost = ext.Cost ?? 0,
                Place = ext.Place,
                StartDate = ext.StartDate.HasValue
                    ? ToUtc(ext.StartDate.Value)
                    : DateTime.UtcNow,
                Uri = ext.Uri,
                Node = node, 
                EventTranslations = new List<EventTranslation>
                {
                    new()
                    {
                        EventName   = Truncate(ext.Name ?? string.Empty, 128),
                        Description = Truncate(ext.Description ?? string.Empty, 512),
                        Language    = targetLanguage,
                    }
                }
            };

            await _eventRepository.AddEvent(evt);
            return evt;
        }
        var hasTranslation = existing.EventTranslations
            .Any(t => t.Language.Id == targetLanguage.Id);

        if (!hasTranslation)
        {
            var source = existing.EventTranslations.First();
            var sourceCode = FromName(source.Language.Name);

            var created = await _translationService.TranslateEventAsync(
                existing, sourceCode, new[] { targetLanguage }, ct);

            foreach (var t in created)
                await _eventRepository.AddTranslation(t);

            existing.EventTranslations.AddRange(created);
        }

        return existing;
    }

    private async Task<Language> GetOrCreateLanguageAsync(LanguageCode code, CancellationToken ct)
    {
        var name = code.ToString();
        var lang = await _db.Languages.FirstOrDefaultAsync(l => l.Name == name, ct);
        if (lang is not null) return lang;

        lang = new Language { Name = name };
        _db.Languages.Add(lang);
        await _db.SaveChangesAsync(ct);
        return lang;
    }

    private static LanguageCode FromCulture(CultureInfo culture) =>
        culture.TwoLetterISOLanguageName switch
        {
            "ky" => LanguageCode.Kg,
            "ru" => LanguageCode.Ru,
            "en" => LanguageCode.En,
            _    => LanguageCode.Ru,
        };

    private static LanguageCode FromName(string name) =>
        Enum.TryParse<LanguageCode>(name, ignoreCase: true, out var c)
            ? c
            : LanguageCode.Ru;

    private static string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max];
}