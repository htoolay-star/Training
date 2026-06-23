using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Contracts;

namespace WebApp.Services.Api;

public sealed class ApiClient
{
    public const string UnauthorizedCode = "UNAUTHORIZED";

    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ApiClient(HttpClient http) => _http = http;

    public Task<Result<T>> GetAsync<T>(string url, string? accessToken = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Get, url, body: null, accessToken, ct);

    public Task<Result<T>> PostAsync<T>(string url, object? body, string? accessToken = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Post, url, body, accessToken, ct);

    public Task<Result<T>> PutAsync<T>(string url, object? body, string? accessToken = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Put, url, body, accessToken, ct);

    public Task<Result<T>> DeleteAsync<T>(string url, string? accessToken = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Delete, url, body: null, accessToken, ct);

    public async Task<Result<T>> SendAsync<T>(
        HttpMethod method, string url, object? body, string? accessToken = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);

            if (accessToken is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (body is not null)
                request.Content = JsonContent.Create(body, body.GetType(), mediaType: null, JsonOptions);

            using var response = await _http.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result<T>.SetResponse(UnauthorizedCode, EnumRespType.Error);

            var result = await response.Content.ReadFromJsonAsync<Result<T>>(JsonOptions, ct);
            return result ?? Result<T>.Error($"Empty response from server ({(int)response.StatusCode}).");
        }
        catch (OperationCanceledException)
        {
            throw; 
        }
        catch (Exception ex)
        {
            return Result<T>.SystemError($"Could not reach the server. {ex.Message}");
        }
    }
}
