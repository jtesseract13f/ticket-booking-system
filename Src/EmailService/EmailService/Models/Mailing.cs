namespace ConcertAggregator.Infrastructure.Models;

public class Mailing
{
    public int Id { get; set; }
    public Filter Filter { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<SentTicket> SentTickets { get; set; } = new();
}