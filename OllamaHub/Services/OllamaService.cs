using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using OllamaHub.Models;

namespace OllamaHub.Services;

public class OllamaService
{
    private readonly HttpClient _http;
    private readonly SettingsService _settings;

    public OllamaService(SettingsService settings)
    {
        _settings = settings;
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };
        UpdateBaseUrl();
        _settings.SettingsChanged += (_, _) => UpdateBaseUrl();
    }

    private void UpdateBaseUrl()
    {
        _http.BaseAddress = new Uri(_settings.Current.OllamaBaseUrl.TrimEnd('/') + "/");
    }

    public async Task<bool> IsOnlineAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync("api/tags", ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<OllamaModelInfo>> GetModelsAsync(CancellationToken ct = default)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<OllamaTagsResponse>("api/tags", ct);
            return result?.Models ?? new List<OllamaModelInfo>();
        }
        catch { return new List<OllamaModelInfo>(); }
    }

    public async IAsyncEnumerable<string> ChatStreamAsync(
        OllamaChatRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _http.PostAsync("api/chat", content, ct);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line)) continue;

            OllamaChatResponse? chunk = null;
            try { chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line); }
            catch { continue; }

            if (chunk?.Message?.Content is { Length: > 0 } text)
                yield return text;

            if (chunk?.Done == true) break;
        }
    }

    public async IAsyncEnumerable<OllamaPullResponse> PullModelAsync(
        string modelName,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var req = new OllamaPullRequest { Name = modelName };
        var json = JsonSerializer.Serialize(req);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _http.PostAsync("api/pull", content, ct);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line)) continue;

            OllamaPullResponse? chunk = null;
            try { chunk = JsonSerializer.Deserialize<OllamaPullResponse>(line); }
            catch { continue; }

            if (chunk != null) yield return chunk;
            if (chunk?.Status == "success") break;
        }
    }

    public async Task<bool> DeleteModelAsync(string modelName, CancellationToken ct = default)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, "api/delete")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { name = modelName }),
                    Encoding.UTF8, "application/json")
            };
            var response = await _http.SendAsync(req, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
