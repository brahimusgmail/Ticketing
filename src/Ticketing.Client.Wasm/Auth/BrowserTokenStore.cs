namespace Ticketing.Client.Wasm.Auth;

using Microsoft.JSInterop;

public sealed class BrowserTokenStore : ITokenStore
{
    private const string _tokenKey = "authToken";
    private readonly IJSRuntime _jsRuntime;

    public BrowserTokenStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask SetTokenAsync(string token)
    {
        return _jsRuntime.InvokeVoidAsync("localStorage.setItem", _tokenKey, token);
    }

    public ValueTask<string?> GetTokenAsync()
    {
        return _jsRuntime.InvokeAsync<string?>("localStorage.getItem", _tokenKey);
    }

    public ValueTask RemoveTokenAsync()
    {
        return _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _tokenKey);
    }
}
