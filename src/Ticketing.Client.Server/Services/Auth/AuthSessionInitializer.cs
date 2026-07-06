namespace Ticketing.Client.Server.Services.Auth;

public sealed class AuthSessionInitializer
{
    private readonly SessionTokenStore _tokenStore;
    private readonly JwtAuthStateProvider _authStateProvider;

    private bool _initialized;
    private Task? _initializationTask;

    public AuthSessionInitializer(SessionTokenStore tokenStore, JwtAuthStateProvider authStateProvider)
    {
        _tokenStore = tokenStore;
        _authStateProvider = authStateProvider;
    }

    public Task EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return Task.CompletedTask;
        }

        _initializationTask ??= InitializeCoreAsync();
        return _initializationTask;
    }

    private async Task InitializeCoreAsync()
    {
        await _tokenStore.LoadFromBrowserAsync();
        _authStateProvider.ForceNotify();
        _initialized = true;
    }
}
