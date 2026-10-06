using EmailService.BLL;

namespace ConcertAggregator.Infrastructure.BLL;


public class TestEmailSender : IEmailSender
{
    private readonly ILogger<TestEmailSender> _logger;

    public TestEmailSender(ILogger<TestEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "\n===== TEST EMAIL =====\nTo: {To}\nSubject: {Subject}\n\n{Body}\n======================",
            to, subject, body);
        return Task.CompletedTask;
    }
}