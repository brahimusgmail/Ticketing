namespace Ticketing.Client.Wasm.Auth;

using System.Net;
using System.Net.Http.Headers;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;
    private readonly AuthApiClient _authApiClient;

    public BearerTokenHandler(
        ITokenStore tokenStore,
        AuthApiClient authApiClient)
    {
        _tokenStore = tokenStore;
        _authApiClient = authApiClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // ✅ Si pas 401 → OK
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (request.RequestUri?.AbsolutePath.Contains("/api/auth/login") == true ||
            request.RequestUri?.AbsolutePath.Contains("/api/auth/refresh") == true)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // 🔥 tentative refresh
        var refreshResult = await _authApiClient.RefreshAsync(cancellationToken);

        if (refreshResult is null)
        {
            await _tokenStore.RemoveTokenAsync();
            return response;
        }

        // ✅ sauvegarder nouveau token
        await _tokenStore.SetTokenAsync(refreshResult.AccessToken);

        // 🔁 rejouer la requête
        var retryRequest = await CloneHttpRequestMessageAsync(request);

        retryRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", refreshResult.AccessToken);

        response.Dispose();

        return await base.SendAsync(retryRequest, cancellationToken);
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(
    HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content is not null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0;

            clone.Content = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
