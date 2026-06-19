using System.Net.Http.Json;
using System.Text.Json;
using Contracts;
using Contracts.Auth;

namespace Training.Blazor.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly TokenStorage _tokenStorage;
    private readonly CustomAuthStateProvider _authProvider;

    public ApiClient(
        HttpClient http,
        TokenStorage tokenStorage,
        CustomAuthStateProvider authProvider)
    {
        _http = http;
        _tokenStorage = tokenStorage;
        _authProvider = authProvider;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<AuthTokenResponse>?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/Login", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Result<AuthTokenResponse>>(JsonOptions);

        if (result is { IsSuccess: true, Data: not null })
        {
            await _tokenStorage.SetAsync(result.Data);
            _authProvider.NotifyStateChanged();
        }

        return result;
    }

    public async Task<Result<object>?> LogoutAsync()
    {
        var refreshToken = await _tokenStorage.GetRefreshTokenAsync();
        var request = new LogoutRequest { RefreshToken = refreshToken ?? "" };
        var response = await _http.PostAsJsonAsync("api/Auth/Logout", request);

        await _tokenStorage.ClearAsync();
        _authProvider.NotifyStateChanged();

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Result<object>>(JsonOptions);
        }

        return null;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _tokenStorage.GetAccessTokenAsync();
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token ?? "");
    }

    public async Task<Result<T>?> GetAsync<T>(string url)
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> PostAsync<T>(string url, object body)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> PutAsync<T>(string url, object body)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> DeleteAsync<T>(string url)
    {
        await SetAuthHeaderAsync();
        var response = await _http.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }
}
