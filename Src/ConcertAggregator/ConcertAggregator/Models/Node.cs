using System.ComponentModel.DataAnnotations;

namespace ConcertAggregator.Models;

public class Node
{
    public Guid Id { get; set; }
    [MaxLength(128)]
    public string ApiType { get; set; }
    [MaxLength(128)]
    public string BaseUrl { get; set; }
    public bool IsBlocked { get; set; }
    
    public ICollection<Event> Events { get; set; }
}