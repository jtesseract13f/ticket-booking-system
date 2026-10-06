using System.Net.Http.Headers;

namespace EmailService;
public class BearerTokenForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _http;
    private readonly ITokenAccessor _tokenAccessor;

    public BearerTokenForwardingHandler(
        IHttpContextAccessor http,
        ITokenAccessor tokenAccessor)
    {
        _http = http;
        _tokenAccessor = tokenAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var token =
            _http.HttpContext?.Request.Headers.Authorization.FirstOrDefault()
            ?? (_tokenAccessor.Token is { } t ? $"Bearer {t}" : null);

        if (!string.IsNullOrWhiteSpace(token) &&
            AuthenticationHeaderValue.TryParse(token, out var parsed))
        {
            request.Headers.Authorization = parsed;
        }

        return await base.SendAsync(request, ct);
    }
}