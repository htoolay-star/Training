using Contracts;
using Contracts.Auth;
using System.Net.Http.Json;
using System.Text.Json;

namespace Training.Blazor.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;

    public ApiClient(
        HttpClient http,
        CustomAuthStateProvider authProvider)
    {
        _http = http;
        _authProvider = authProvider;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<AuthTokenResponse>?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/Login", request);

        var result = await response.Content.ReadFromJsonAsync<Result<AuthTokenResponse>>(JsonOptions);

        if (result is { IsSuccess: true, Data: not null })
        {
            _authProvider.NotifyStateChanged();
        }

        return result;
    }

    public async Task<Result<object>?> LogoutAsync()
    {
        var response = await _http.PostAsync("api/Auth/Logout", null);

        _authProvider.NotifyStateChanged();

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Result<object>>(JsonOptions);
        }

        return null;
    }

    public async Task<Result<T>?> GetAsync<T>(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> PostAsync<T>(string url, object body)
    {
        var response = await _http.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> PutAsync<T>(string url, object body)
    {
        var response = await _http.PutAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }

    public async Task<Result<T>?> DeleteAsync<T>(string url)
    {
        var response = await _http.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions);
    }
}
