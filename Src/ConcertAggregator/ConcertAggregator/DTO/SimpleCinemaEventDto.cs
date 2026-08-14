namespace ConcertAggregator.DTO;

public record SimpleCinemaEventDto(
    Guid Id,
    int Cost,
    DateTime StartDate,
    string Name,
    string Description,
    string Uri,
    string Place
    );