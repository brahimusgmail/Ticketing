using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Ticketing.Client.Wasm;
using Ticketing.Client.Wasm.Auth;
using Ticketing.Client.Wasm.Services;
using Ticketing.Client.Wasm.Services.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ITokenStore, BrowserTokenStore>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<BearerTokenHandler>();

var baseUrl = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:7146/";

builder.Services.AddScoped<AuthApiClient>(sp =>
{
    return new AuthApiClient(new HttpClient
    {
        BaseAddress = new Uri(baseUrl),
    });
});

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<BearerTokenHandler>();
    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(baseUrl),
    };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TicketsApiClient>();
builder.Services.AddScoped<CategoriesApiClient>();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-FR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-FR");

await builder.Build().RunAsync();
