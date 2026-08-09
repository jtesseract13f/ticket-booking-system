using System.ComponentModel.DataAnnotations;

namespace ConcertAggregator.Models;

public class EventTranslation
{ //TODO: прикрутить локальный ИИ для авто-генерации перевода
    [Key]
    public int Id { get; set; }
    [MaxLength(128)]
    public string EventName { get; set; }
    [MaxLength(512)]
    public string Description { get; set; }
    
    public Event Event { get; set; }
    public Language Language { get; set; }
}