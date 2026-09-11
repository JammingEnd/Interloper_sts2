using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Interloper.InterloperCode.Telemetry;

/// <summary>
/// Minimal Supabase REST/Data API client using only the public/publishable anon key.
/// No service-role key or secrets are ever used. Failures are swallowed by callers.
/// </summary>
public class SupabaseClient
{
    private readonly HttpClient _http = new();
    private readonly string _baseUrl;
    private readonly string _anonKey;

    public SupabaseClient(string baseUrl, string anonKey)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _anonKey = anonKey;
    }

    public async Task PostAsync(string table, object payload)
    {
        var request = BuildRequest(HttpMethod.Post, table);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task PatchAsync(string table, string id, object payload)
    {
        var request = BuildRequest(HttpMethod.Patch, $"{table}?id=eq.{id}");
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, $"{_baseUrl}/rest/v1/{path}");
        request.Headers.Add("apikey", _anonKey);
        request.Headers.Add("Authorization", $"Bearer {_anonKey}");
        return request;
    }
}