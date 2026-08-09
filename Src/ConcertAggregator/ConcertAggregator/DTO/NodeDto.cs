namespace ConcertAggregator.DTO;

public record AddNodeDto(
    string ApiType,
    string BaseUrl
    );
    
public record UpdateNodeDto(
    Guid Id,
    string? ApiType,
    string? BaseUrl,
    bool? IsBlocked);
    
public record DeleteNodeDto(
    Guid Id);
    
public record GetNodeDto(
    Guid Id,
    string ApiType,
    string BaseUrl,
    bool IsBlocked);