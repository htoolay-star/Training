using Contracts.Auth;
using Microsoft.JSInterop;

namespace Training.Blazor.Services;

public sealed class TokenStorage
{
    private const string KeyAccessToken = "access_token";
    private const string KeyRefreshToken = "refresh_token";
    private const string KeyUserName = "user_name";
    private const string KeyRoleCode = "role_code";
    private const string KeyExpiresAt = "expires_at";

    private readonly IJSRuntime _js;

    public TokenStorage(IJSRuntime js)
    {
        _js = js;
    }

    public async ValueTask SetAsync(AuthTokenResponse tokens)
    {
        await using var localStorage = await GetLocalStorage();
        await localStorage.InvokeVoidAsync("setItem", KeyAccessToken, tokens.AccessToken);
        await localStorage.InvokeVoidAsync("setItem", KeyRefreshToken, tokens.RefreshToken);
        await localStorage.InvokeVoidAsync("setItem", KeyUserName, tokens.UserName);
        await localStorage.InvokeVoidAsync("setItem", KeyRoleCode, tokens.RoleCode);
        await localStorage.InvokeVoidAsync("setItem", KeyExpiresAt, tokens.AccessTokenExpiresAt.ToString("O"));
    }

    public async ValueTask<string?> GetAccessTokenAsync()
    {
        await using var localStorage = await GetLocalStorage();
        return await localStorage.InvokeAsync<string?>("getItem", KeyAccessToken);
    }

    public async ValueTask<string?> GetRefreshTokenAsync()
    {
         await using var localStorage = await GetLocalStorage();
        return await localStorage.InvokeAsync<string?>("getItem", KeyRefreshToken);
    }

    public async ValueTask<string?> GetUserNameAsync()
    {
        await using var localStorage = await GetLocalStorage();
        return await localStorage.InvokeAsync<string?>("getItem", KeyUserName);
    }

    public async ValueTask<string?> GetRoleCodeAsync()
    {
        await using var localStorage = await GetLocalStorage();
        return await localStorage.InvokeAsync<string?>("getItem", KeyRoleCode);
    }

    public async ValueTask<DateTime?> GetExpiresAtAsync()
    {
        await using var localStorage = await GetLocalStorage();
        var val = await localStorage.InvokeAsync<string?>("getItem", KeyExpiresAt);
        if (DateTime.TryParse(val, out var dt))
            return dt;
        return null;
    }

    public async ValueTask ClearAsync()
    {
        await using var localStorage = await GetLocalStorage();
        await localStorage.InvokeVoidAsync("removeItem", KeyAccessToken);
        await localStorage.InvokeVoidAsync("removeItem", KeyRefreshToken);
        await localStorage.InvokeVoidAsync("removeItem", KeyUserName);
        await localStorage.InvokeVoidAsync("removeItem", KeyRoleCode);
        await localStorage.InvokeVoidAsync("removeItem", KeyExpiresAt);
    }

    private async ValueTask<IJSObjectReference> GetLocalStorage()
    {
        return await _js.InvokeAsync<IJSObjectReference>("eval", "window.localStorage");
    }
}
