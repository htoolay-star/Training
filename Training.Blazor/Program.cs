using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Training.Blazor;
using Training.Blazor.Services;
using Training.Blazor.Services.AdminUser;
using Training.Blazor.Services.Role;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5254";

builder.Services.AddTransient<AuthInterceptor>();
builder.Services.AddScoped(sp =>
{
    var interceptor = sp.GetRequiredService<AuthInterceptor>();
    interceptor.InnerHandler = new HttpClientHandler();
    return new HttpClient(interceptor) { BaseAddress = new Uri(apiBaseUrl) };
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<IRoleApiClient, RoleApiClient>();
builder.Services.AddScoped<IAdminUserApiClient, AdminUserApiClient>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
