using System.ComponentModel.DataAnnotations;

namespace ConcertAggregator.Models;

public class Event
{
    [Key]
    public Guid Id { get; set; }
    public int Cost { get; set; }
    public string Place { get; set; }
    public DateTime StartDate { get; set; }
    public Node Node { get; set; }
    public List<EventTranslation> EventTranslations { get; set; }
}