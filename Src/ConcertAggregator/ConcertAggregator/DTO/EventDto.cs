namespace ConcertAggregator.DTO;

public record EventDto(string ExternalId, 
    int? Cost,
    string? Place,
    string? Name,
    string? Description,
    DateTime? StartDate,
    string? Uri
    );