namespace Ticketing.Client.Server.Services.Auth;

using System.Net.Http.Headers;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public BearerTokenHandler(ITokenStore tokenStore) => _tokenStore = tokenStore;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
