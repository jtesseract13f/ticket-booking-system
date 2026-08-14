using System.ComponentModel.DataAnnotations;

namespace ConcertAggregator.Models;

public class Event
{
    [Key]
    public Guid Id { get; set; }
    public int Cost { get; set; }
    [MaxLength(256)]
    public string? Place { get; set; }
    public DateTime StartDate { get; set; }
    [MaxLength(128)]
    public string Uri { get; set; }
    public Node Node { get; set; }
    public List<EventTranslation> EventTranslations { get; set; }
}