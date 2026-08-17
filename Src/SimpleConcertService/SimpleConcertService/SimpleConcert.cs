namespace SimpleConcertService;

public record SimpleConcert(
    Guid Id,
    int Cost,
    DateTime StartDate,
    string Name,
    string Description,
    string Uri,
    string Place
);