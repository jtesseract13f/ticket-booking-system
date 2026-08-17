namespace SimpleCinemaService;


public record SimpleCinema(
    Guid Id,
    int Cost,
    DateTime StartDate,
    string Name,
    string Description,
    string Uri,
    string Place
);