using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Localization;
using Ticketing.Client.Server.Components;
using Ticketing.Client.Server.Services.Api;
using Ticketing.Client.Server.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<SessionTokenStore>();
builder.Services.AddScoped<ITokenStore>(sp => sp.GetRequiredService<SessionTokenStore>());
builder.Services.AddScoped<AuthSessionInitializer>();

builder.Services.AddScoped<HttpClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Api:BaseUrl"] ?? "https://localhost:7146";
    var tokenStore = sp.GetRequiredService<ITokenStore>();

    var handler = new BearerTokenHandler(tokenStore)
    {
        InnerHandler = new HttpClientHandler(),
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri(baseUrl),
    };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<TicketsApiClient>();
builder.Services.AddScoped<CategoriesApiClient>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());

var supportedCultures = new[] { new CultureInfo("fr-FR") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("fr-FR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

var localizationOptions = app.Services.GetRequiredService<
    Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;

app.UseRequestLocalization(localizationOptions);

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
