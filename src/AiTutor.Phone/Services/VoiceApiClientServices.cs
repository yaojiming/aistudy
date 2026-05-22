using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using AiTutor.Shared.Agent;
using AiTutor.Shared.Voice;

namespace AiTutor.Maui.Services;

public sealed class ApiSpeechToTextService : ISpeechToTextService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IAppSettingsService _settingsService;

    public ApiSpeechToTextService(HttpClient httpClient, IAppSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    public async Task<SpeechToTextResult> ConvertSpeechToTextAsync(string audioFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(audioFilePath))
        {
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "录音文件不存在"
            };
        }

        var options = await _settingsService.GetApiOptionsAsync(cancellationToken);
        using var timeoutCts = CreateTimeoutTokenSource(options, cancellationToken);
        var stopwatch = Stopwatch.StartNew();

        await using var stream = File.OpenRead(audioFilePath);
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ResolveAudioContentType(audioFilePath));
        content.Add(fileContent, "file", Path.GetFileName(audioFilePath));

        using var response = await _httpClient.PostAsync(BuildApiUri(options, "/api/voice/asr"), content, timeoutCts.Token);
        var result = await ReadResponseAsync<SpeechToTextResult>(response, timeoutCts.Token);
        result.ElapsedMilliseconds = result.ElapsedMilliseconds == 0 ? stopwatch.ElapsedMilliseconds : result.ElapsedMilliseconds;
        return result;
    }

    private static Uri BuildApiUri(ApiClientOptions options, string path)
    {
        return new Uri(new Uri(options.BaseUrl.TrimEnd('/') + "/"), path.TrimStart('/'));
    }

    private static string ResolveAudioContentType(string audioFilePath)
    {
        var extension = Path.GetExtension(audioFilePath);
        return extension.Equals(".pcm", StringComparison.OrdinalIgnoreCase)
            ? "application/octet-stream"
            : "audio/mp4";
    }

    private static CancellationTokenSource CreateTimeoutTokenSource(ApiClientOptions options, CancellationToken cancellationToken)
    {
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 5, 900)));
        return timeoutCts;
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"服务返回错误：{(int)response.StatusCode} {errorText}");
        }

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("服务返回内容为空。");
    }
}

public sealed class ApiTextToSpeechService : ITextToSpeechService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IAppSettingsService _settingsService;

    public ApiTextToSpeechService(HttpClient httpClient, IAppSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    public async Task<TtsResult> ConvertTextToSpeechAsync(string text, CancellationToken cancellationToken = default)
    {
        var options = await _settingsService.GetApiOptionsAsync(cancellationToken);
        using var timeoutCts = CreateTimeoutTokenSource(options, cancellationToken);
        var stopwatch = Stopwatch.StartNew();

        using var response = await _httpClient.PostAsJsonAsync(BuildApiUri(options, "/api/voice/tts"), new TtsRequest
        {
            Text = text
        }, JsonOptions, timeoutCts.Token);

        var result = await ReadResponseAsync<TtsResult>(response, timeoutCts.Token);
        result.ElapsedMilliseconds = result.ElapsedMilliseconds == 0 ? stopwatch.ElapsedMilliseconds : result.ElapsedMilliseconds;
        if (!string.IsNullOrWhiteSpace(result.AudioUrl) && result.AudioUrl.StartsWith("/", StringComparison.Ordinal))
        {
            result.AudioUrl = new Uri(new Uri(options.BaseUrl.TrimEnd('/') + "/"), result.AudioUrl.TrimStart('/')).ToString();
        }

        return result;
    }

    private static Uri BuildApiUri(ApiClientOptions options, string path)
    {
        return new Uri(new Uri(options.BaseUrl.TrimEnd('/') + "/"), path.TrimStart('/'));
    }

    private static CancellationTokenSource CreateTimeoutTokenSource(ApiClientOptions options, CancellationToken cancellationToken)
    {
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 5, 900)));
        return timeoutCts;
    }

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"服务返回错误：{(int)response.StatusCode} {errorText}");
        }

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("服务返回内容为空。");
    }
}

public sealed class ApiAiChatService : IAiChatService
{
    private readonly IApiClientService _apiClientService;
    private readonly IAppSettingsService _settingsService;
    private readonly ICurrentUserService _currentUserService;

    public ApiAiChatService(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        ICurrentUserService currentUserService)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _currentUserService = currentUserService;
    }

    public async Task<AiChatResult> AskAsync(string userText, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _apiClientService.AskAsync(new AgentRequest
        {
            UserId = _currentUserService.UserId,
            Grade = _settingsService.GetCurrentGrade(),
            Subject = _settingsService.GetCurrentSubject(),
            InputType = "text",
            Mode = "ask",
            QuestionText = userText
        }, cancellationToken);

        stopwatch.Stop();
        return new AiChatResult
        {
            Success = !string.IsNullOrWhiteSpace(response.AnswerText),
            AnswerText = response.AnswerText,
            ErrorMessage = string.IsNullOrWhiteSpace(response.AnswerText) ? "AI 暂时没有返回内容" : string.Empty,
            StatusCode = 200,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
        };
    }
}
