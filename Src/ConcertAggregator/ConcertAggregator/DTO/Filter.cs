namespace ConcertAggregator.DTO;

public class Filter
{
    public string? EventType { get; set; }
    public int? Price { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}