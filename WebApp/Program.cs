using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Services;
using WebApp.Services.Api;
using WebApp.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddHttpClient<ApiClient>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7176/";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ITokenStore, TokenStore>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/login";
        option.Cookie.Name = "Cookie_Auth";
        option.SlidingExpiration = true;
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        option.Events.OnValidatePrincipal = async context =>
        {
            var sessionId = context.Principal?.FindFirst(SessionManager.SessionIdClaim)?.Value;
            var store = context.HttpContext.RequestServices.GetRequiredService<ITokenStore>();

            if (sessionId is null || !store.Exists(sessionId))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<SessionManager>();
builder.Services.AddScoped<InjectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost(PageUrl.Logout, async (HttpContext http, SessionManager session) =>
{
    await session.SignOutAsync(http);
    return Results.Redirect(PageUrl.Login);
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();