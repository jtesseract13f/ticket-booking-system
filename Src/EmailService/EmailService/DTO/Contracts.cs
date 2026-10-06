namespace EmailService.DTO;

public record FilterDto(string? EventType, int? Price, DateTime? DateFrom, DateTime? DateTo);
public record CreateMailingRequest(FilterDto Filter, string Email);
public record UpdateMailingRequest(FilterDto Filter, string Email, bool IsActive);
public record MailingResponse(
    int Id,
    FilterDto Filter,
    string Email,
    bool IsActive,
    DateTime CreatedAt);