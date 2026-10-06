using ConcertAggregator.Infrastructure.Models;
using Refit;

namespace ConcertAggregator.Infrastructure.BLL;

public interface ITicketApi
{
    [Get("/api/tickets")]
    Task<List<Ticket>> GetTicketsAsync(
        [Query] string? eventType,
        [Query] int? price,
        [Query] DateTime? dateFrom,
        [Query] DateTime? dateTo,
        CancellationToken ct = default);
}