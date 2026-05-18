using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTutor.Shared.Agent;

namespace AiTutor.Wpf.Services;

public interface IApiClientService
{
    Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default);

    Task<AgentResponse?> AskStreamAsync(
        AgentRequest request,
        Func<string, Task> onDeltaAsync,
        CancellationToken cancellationToken = default);

    Task<MediaUploadResultDto> UploadImageAsync(
        string imagePath,
        string resourceType,
        string userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// WPF 端统一 API 客户端。客户端只调用 AiTutor.Api，不保存也不使用模型 API Key。
/// </summary>
public sealed class ApiClientService : IApiClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IAppSettingsService _settings;

    public ApiClientService(HttpClient httpClient, IAppSettingsService settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<AgentResponse> AskAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        using var linkedCts = CreateTimeoutToken(cancellationToken);
        var response = await _httpClient.PostAsJsonAsync(
            BuildUri("/api/agent/ask"),
            request,
            JsonOptions,
            linkedCts.Token);

        return await ReadResponseAsync<AgentResponse>(response, linkedCts.Token);
    }

    public async Task<AgentResponse?> AskStreamAsync(
        AgentRequest request,
        Func<string, Task> onDeltaAsync,
        CancellationToken cancellationToken = default)
    {
        using var linkedCts = CreateTimeoutToken(cancellationToken);
        using var message = new HttpRequestMessage(HttpMethod.Post, BuildUri("/api/agent/ask-stream"))
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };

        using var response = await _httpClient.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            linkedCts.Token);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(linkedCts.Token);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        AgentResponse? finalResponse = null;
        while (true)
        {
            var line = await reader.ReadLineAsync(linkedCts.Token);
            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var json = line.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                ? line[5..].Trim()
                : line.Trim();

            if (json == "[DONE]")
            {
                break;
            }

            var chunk = JsonSerializer.Deserialize<AgentStreamChunkDto>(json, JsonOptions);
            if (chunk is null)
            {
                continue;
            }

            if (chunk.Type.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(chunk.ErrorMessage ?? "AI 流式输出失败。");
            }

            if (chunk.Type.Equals("final", StringComparison.OrdinalIgnoreCase))
            {
                finalResponse = chunk.FinalResponse;
                continue;
            }

            if (!string.IsNullOrEmpty(chunk.Text))
            {
                await onDeltaAsync(chunk.Text);
            }
        }

        return finalResponse;
    }

    public async Task<MediaUploadResultDto> UploadImageAsync(
        string imagePath,
        string resourceType,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("图片文件不存在。", imagePath);
        }

        using var linkedCts = CreateTimeoutToken(cancellationToken);
        await using var fileStream = File.OpenRead(imagePath);
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(imagePath));
        content.Add(fileContent, "file", Path.GetFileName(imagePath));
        content.Add(new StringContent(resourceType), "resourceType");
        content.Add(new StringContent(userId), "userId");
        content.Add(new StringContent("wpf"), "sourceType");

        var response = await _httpClient.PostAsync(BuildUri("/api/media/upload-image"), content, linkedCts.Token);
        return await ReadResponseAsync<MediaUploadResultDto>(response, linkedCts.Token);
    }

    private Uri BuildUri(string path)
    {
        var baseUrl = _settings.Current.Api.BaseUrl.TrimEnd('/');
        return new Uri($"{baseUrl}{path}");
    }

    private CancellationTokenSource CreateTimeoutToken(CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromSeconds(_settings.Current.Api.TimeoutSeconds);
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(timeout);
        return linkedCts;
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"后端请求失败：{(int)response.StatusCode} {body}");
        }

        return JsonSerializer.Deserialize<T>(body, JsonOptions)
            ?? throw new InvalidOperationException("后端返回内容为空。");
    }

    private static string GetMimeType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
