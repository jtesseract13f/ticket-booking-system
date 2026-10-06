namespace EmailService;

public interface ITokenAccessor
{
    string? Token { get; set; }
}

public class AsyncLocalTokenAccessor : ITokenAccessor
{
    private static readonly AsyncLocal<string?> _token = new();
    public string? Token
    {
        get => _token.Value;
        set => _token.Value = value;
    }
}