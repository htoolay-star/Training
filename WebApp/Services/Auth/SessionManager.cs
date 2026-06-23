using System.Security.Claims;
using Contracts;
using Contracts.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Services.Api;

namespace WebApp.Services.Auth;

public sealed class SessionManager
{
    public const string SessionIdClaim = "session_id";

    private const string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;
    private const string SessionExpiredMessage = "Your session has expired. Please sign in again.";

    private readonly ApiClient _api;
    private readonly ITokenStore _store;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authState;

    public SessionManager(
        ApiClient api,
        ITokenStore store,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authState)
    {
        _api = api;
        _store = store;
        _httpContextAccessor = httpContextAccessor;
        _authState = authState;
    }

    public async Task<Result<T>> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct = default)
    {
        var sessionId = await GetSessionIdAsync();
        var tokens = sessionId is null ? null : _store.Get(sessionId);
        if (sessionId is null || tokens is null)
            return Result<T>.Error(SessionExpiredMessage);

        var result = await _api.SendAsync<T>(method, url, body, tokens.AccessToken, ct);
        if (result.Code != ApiClient.UnauthorizedCode)
            return result;

        if (!await RefreshAsync(sessionId, tokens.AccessToken, ct))
            return Result<T>.Error(SessionExpiredMessage);

        var refreshed = _store.Get(sessionId);
        return refreshed is null
            ? Result<T>.Error(SessionExpiredMessage)
            : await _api.SendAsync<T>(method, url, body, refreshed.AccessToken, ct);
    }

    private async Task<bool> RefreshAsync(string sessionId, string usedAccessToken, CancellationToken ct)
    {
        var gate = _store.GetRefreshLock(sessionId);
        await gate.WaitAsync(ct);
        try
        {
            var current = _store.Get(sessionId);
            if (current is null)
                return false;

            // check again for concurrent request (token is already generate while we're waiting)
            if (current.AccessToken != usedAccessToken)
                return true;

            var result = await _api.PostAsync<AuthTokenResponse>(
                ApiEndpoint.Refresh, new RefreshTokenRequest { RefreshToken = current.RefreshToken }, ct: ct);

            if (result.IsError || result.Data is null)
            {
                _store.Remove(sessionId); // invalid or expire refresh token and login again .
                return false;
            }

            usedAccessToken = result.Data.AccessToken;
            _store.Set(sessionId, new TokenEntry(result.Data.AccessToken, result.Data.RefreshToken));
            return true;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<Result<AuthTokenResponse>> SignInAsync(HttpContext http, LoginRequest request, CancellationToken ct = default)
    {
        var result = await _api.PostAsync<AuthTokenResponse>(ApiEndpoint.Login, request, ct: ct);
        if (result.IsError || result.Data is null)
            return result;

        var data = result.Data;
        var sessionId = Guid.NewGuid().ToString("N");
        _store.Set(sessionId, new TokenEntry(data.AccessToken, data.RefreshToken));

        var claims = new List<Claim>
        {
            new(SessionIdClaim, sessionId),
            new(ClaimTypes.Name, data.UserName),
            new(ClaimTypes.Role, data.RoleCode),
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme));

        await http.SignInAsync(Scheme, principal, new AuthenticationProperties { IsPersistent = true });
        return result;
    }

    public async Task SignOutAsync(HttpContext http, CancellationToken ct = default)
    {
        var sessionId = http.User.FindFirst(SessionIdClaim)?.Value;
        if (sessionId is not null)
        {
            var tokens = _store.Get(sessionId);
            if (tokens is not null)
                await _api.PostAsync<bool>(ApiEndpoint.Logout, new LogoutRequest { RefreshToken = tokens.RefreshToken }, ct: ct);

            _store.Remove(sessionId);
        }

        await http.SignOutAsync(Scheme);
    }

    private async Task<string?> GetSessionIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            user = (await _authState.GetAuthenticationStateAsync()).User;

        return user?.FindFirst(SessionIdClaim)?.Value;
    }
}
