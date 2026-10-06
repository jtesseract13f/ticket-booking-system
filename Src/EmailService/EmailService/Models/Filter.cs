namespace ConcertAggregator.Infrastructure.Models;

public class Filter
{
    public int Id { get; set; }
    public string? EventType { get; set; }
    public int? Price { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}