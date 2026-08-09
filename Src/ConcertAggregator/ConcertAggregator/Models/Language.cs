using System.ComponentModel.DataAnnotations;

namespace ConcertAggregator.Models;

public class Language
{ 
    [Key]
    public int Id { get; set; }
    
    [MaxLength(50)]
    public string Name { get; set; }
}