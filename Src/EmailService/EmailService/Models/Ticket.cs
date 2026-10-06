namespace ConcertAggregator.Infrastructure.Models;

public class Ticket
{
    public int Id { get; set; }
    public string EventName { get; set; } = null!;
    public int Price { get; set; }
    public string Place { get; set; } = null!;
    public DateTime EventDate { get; set; }
}