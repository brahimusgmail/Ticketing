namespace Ticketing.Client.Wasm.Auth;

public interface ITokenStore
{
    ValueTask SetTokenAsync(string token);

    ValueTask<string?> GetTokenAsync();

    ValueTask RemoveTokenAsync();
}
