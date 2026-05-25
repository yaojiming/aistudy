using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using AiTutor.Shared.Agent;
using AiTutor.Shared.Voice;

namespace AiTutor.Maui.Services;

internal static class VoiceApiDiagnostics
{
    public static string? ValidateDeviceCanReachBaseUrl(string baseUrl)
    {
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            return $"后端地址格式不正确：{baseUrl}";
        }

        var host = uri.Host;
        if (DeviceInfo.Current.DeviceType == DeviceType.Physical
            && (host.Equals("10.0.2.2", StringComparison.OrdinalIgnoreCase)
                || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
                || host.Equals("localhost", StringComparison.OrdinalIgnoreCase)))
        {
            return $"真机不能使用 {baseUrl} 访问电脑后端，请在设置里改成电脑局域网地址，例如 http://192.168.x.x:5088";
        }

        return null;
    }

    public static string BuildConnectionErrorMessage(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                messages.Add(current.Message);
            }
        }

        return string.Join("；", messages.Distinct());
    }
}

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

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var options = await _settingsService.GetApiOptionsAsync(cancellationToken);
            var apiUri = BuildApiUri(options, "/api/voice/asr");
            var localAddressError = VoiceApiDiagnostics.ValidateDeviceCanReachBaseUrl(options.BaseUrl);
            if (!string.IsNullOrWhiteSpace(localAddressError))
            {
                return new SpeechToTextResult
                {
                    Success = false,
                    ErrorMessage = localAddressError,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            using var timeoutCts = CreateTimeoutTokenSource(options, cancellationToken);

            await using var stream = File.OpenRead(audioFilePath);
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ResolveAudioContentType(audioFilePath));
            content.Add(fileContent, "file", Path.GetFileName(audioFilePath));

            using var response = await _httpClient.PostAsync(apiUri, content, timeoutCts.Token);
            var result = await ReadResponseAsync<SpeechToTextResult>(response, timeoutCts.Token);
            result.ElapsedMilliseconds = result.ElapsedMilliseconds == 0 ? stopwatch.ElapsedMilliseconds : result.ElapsedMilliseconds;
            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音识别请求超时，请检查后端地址或网络",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = $"语音识别请求失败：{VoiceApiDiagnostics.BuildConnectionErrorMessage(ex)}",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
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
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var options = await _settingsService.GetApiOptionsAsync(cancellationToken);
            var apiUri = BuildApiUri(options, "/api/voice/tts");
            var localAddressError = VoiceApiDiagnostics.ValidateDeviceCanReachBaseUrl(options.BaseUrl);
            if (!string.IsNullOrWhiteSpace(localAddressError))
            {
                return new TtsResult
                {
                    Success = false,
                    ErrorMessage = localAddressError,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            using var timeoutCts = CreateTimeoutTokenSource(options, cancellationToken);

            using var response = await _httpClient.PostAsJsonAsync(apiUri, new TtsRequest
            {
                Text = text
            }, JsonOptions, timeoutCts.Token);

            var result = await ReadResponseAsync<TtsResult>(response, timeoutCts.Token);
            result.ElapsedMilliseconds = result.ElapsedMilliseconds == 0 ? stopwatch.ElapsedMilliseconds : result.ElapsedMilliseconds;
            if (!string.IsNullOrWhiteSpace(result.AudioUrl) && result.AudioUrl.StartsWith("/", StringComparison.Ordinal))
            {
                result.AudioUrl = new Uri(new Uri(options.BaseUrl.TrimEnd('/') + "/"), result.AudioUrl.TrimStart('/')).ToString();
            }

            // 后端返回的 AudioFilePath 是服务器物理路径，手机端不能直接播放。
            // 手机端只保留真实存在的本机文件；否则把 AudioUrl 下载到本机缓存后播放。
            if (!string.IsNullOrWhiteSpace(result.AudioFilePath) && !File.Exists(result.AudioFilePath))
            {
                result.AudioFilePath = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(result.AudioFilePath) && !string.IsNullOrWhiteSpace(result.AudioUrl))
            {
                result.AudioFilePath = await TryDownloadAudioToCacheAsync(result.AudioUrl, timeoutCts.Token);
            }

            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音合成请求超时，请检查后端地址或网络",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            return new TtsResult
            {
                Success = false,
                ErrorMessage = $"语音合成请求失败：{VoiceApiDiagnostics.BuildConnectionErrorMessage(ex)}",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
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
    private async Task<string> TryDownloadAudioToCacheAsync(string audioUrl, CancellationToken cancellationToken)
    {
        try
        {
            if (!Uri.TryCreate(audioUrl, UriKind.Absolute, out var uri))
            {
                return string.Empty;
            }

            using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var extension = Path.GetExtension(uri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".mp3";
            }

            var voiceDirectory = Path.Combine(FileSystem.CacheDirectory, "voice");
            Directory.CreateDirectory(voiceDirectory);
            var localPath = Path.Combine(voiceDirectory, $"tts-{Guid.NewGuid():N}{extension}");
            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var target = File.Create(localPath);
            await source.CopyToAsync(target, cancellationToken);
            return localPath;
        }
        catch
        {
            return string.Empty;
        }
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
