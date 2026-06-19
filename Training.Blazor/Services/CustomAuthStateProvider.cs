using Contracts;
using Contracts.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Training.Blazor.Services;

public sealed class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;

    public CustomAuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/auth/Me");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<LoginResponse>>();

                if (result is not null && result.IsSuccess && result.Data is not null && !string.IsNullOrEmpty(result.Data.UserName))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, result.Data.UserName),
                        new(ClaimTypes.Role, result.Data.RoleCode)
                    };

                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
        }
        catch
        {
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
