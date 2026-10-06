using System.Text;
using ConcertAggregator.Infrastructure.Models;
using EmailService.BLL;
using EmailService.DTO;
using Microsoft.EntityFrameworkCore;

namespace ConcertAggregator.Infrastructure.BLL;

public class MailingService
{
    private readonly AppDbContext _db;
    private readonly ITicketApi _ticketApi;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<MailingService> _logger;

    public MailingService(
        AppDbContext db,
        ITicketApi ticketApi,
        IEmailSender emailSender,
        ILogger<MailingService> logger)
    {
        _db = db;
        _ticketApi = ticketApi;
        _emailSender = emailSender;
        _logger = logger;
    }
    
    public async Task<List<MailingResponse>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Mailings
            .Include(m => m.Filter)
            .Select(m => ToResponse(m))
            .ToListAsync(ct);
    }

    public async Task<MailingResponse?> GetAsync(int id, CancellationToken ct)
    {
        var m = await _db.Mailings
            .Include(x => x.Filter)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToResponse(m);
    }

    public async Task<MailingResponse> CreateAsync(CreateMailingRequest req, CancellationToken ct)
    {
        var mailing = new Mailing
        {
            Email = req.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Filter = new Filter
            {
                EventType = req.Filter.EventType,
                Price = req.Filter.Price,
                DateFrom = req.Filter.DateFrom,
                DateTo = req.Filter.DateTo
            }
        };

        _db.Mailings.Add(mailing);
        await _db.SaveChangesAsync(ct);
        return ToResponse(mailing);
    }

    public async Task<MailingResponse?> UpdateAsync(int id, UpdateMailingRequest req, CancellationToken ct)
    {
        var m = await _db.Mailings
            .Include(x => x.Filter)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return null;

        m.Email = req.Email;
        m.IsActive = req.IsActive;
        m.Filter.EventType = req.Filter.EventType;
        m.Filter.Price = req.Filter.Price;
        m.Filter.DateFrom = req.Filter.DateFrom;
        m.Filter.DateTo = req.Filter.DateTo;

        await _db.SaveChangesAsync(ct);
        return ToResponse(m);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var m = await _db.Mailings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (m is null) return false;

        _db.Mailings.Remove(m);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ---------- Обработка рассылок ----------

    public async Task ProcessAllAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var mailings = await _db.Mailings
            .Include(m => m.Filter)
            .Include(m => m.SentTickets)
            .Where(m => m.IsActive)
            .ToListAsync(ct);

        foreach (var mailing in mailings)
        {
            // Если DateTo у фильтра задан и уже прошёл — деактивируем рассылку
            if (mailing.Filter.DateTo.HasValue && mailing.Filter.DateTo.Value < now)
            {
                mailing.IsActive = false;
                _logger.LogInformation("Mailing {Id} deactivated: DateTo passed", mailing.Id);
                continue;
            }

            await ProcessOneAsync(mailing, ct);
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task ProcessOneAsync(Mailing mailing, CancellationToken ct)
    {
        List<Ticket> tickets;
        try
        {
            tickets = await _ticketApi.GetTicketsAsync(
                mailing.Filter.EventType,
                mailing.Filter.Price,
                mailing.Filter.DateFrom,
                mailing.Filter.DateTo,
                ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось получить билеты для рассылки {Id}", mailing.Id);
            return;
        }

        var sentIds = mailing.SentTickets.Select(st => st.TicketId).ToHashSet();
        var newTickets = tickets.Where(t => !sentIds.Contains(t.Id)).ToList();

        if (newTickets.Count == 0)
        {
            _logger.LogInformation("Нет новых билетов для рассылки {Id}", mailing.Id);
            return;
        }

        var body = BuildEmailBody(newTickets);
        await _emailSender.SendAsync(mailing.Email, "Новые билеты по вашей подписке", body, ct);

        foreach (var t in newTickets)
            mailing.SentTickets.Add(new SentTicket { MailingId = mailing.Id, TicketId = t.Id });

        _logger.LogInformation("Рассылка {Id}: отправлено {Count} новых билетов", mailing.Id, newTickets.Count);
    }

    private static string BuildEmailBody(IEnumerable<Ticket> tickets)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Найдены новые билеты:");
        foreach (var t in tickets)
            sb.AppendLine($"• {t.EventName} | {t.Price} ₽ | {t.Place} | {t.EventDate:yyyy-MM-dd HH:mm}");
        return sb.ToString();
    }

    private static MailingResponse ToResponse(Mailing m) => new(
        m.Id,
        new FilterDto(m.Filter.EventType, m.Filter.Price, m.Filter.DateFrom, m.Filter.DateTo),
        m.Email,
        m.IsActive,
        m.CreatedAt);
}