using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using AiTutor.Shared.Voice;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Services;

public sealed class RealSpeechToTextService : ISpeechToTextService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly VoiceAiOptions _options;
    private readonly ILogger<RealSpeechToTextService> _logger;

    public RealSpeechToTextService(
        HttpClient httpClient,
        IOptions<VoiceAiOptions> options,
        ILogger<RealSpeechToTextService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SpeechToTextResult> ConvertSpeechToTextAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var asr = _options.Asr;
        if (asr.Provider.Equals("Xunfei", StringComparison.OrdinalIgnoreCase))
        {
            return await ConvertWithXunfeiAsync(audioStream, fileName, cancellationToken);
        }

        if (IsMissing(asr.BaseUrl) || IsMissing(asr.ApiKey))
        {
            _logger.LogWarning("ASR service is not configured. Provider={Provider}, BaseUrl={BaseUrl}, Model={Model}", asr.Provider, asr.BaseUrl, asr.Model);
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音服务未配置"
            };
        }

        var endpoint = BuildEndpoint(asr.BaseUrl, "/v1/audio/transcriptions");
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(asr.TimeoutSeconds, 5, 300)));

        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", asr.ApiKey);

            using var form = new MultipartFormDataContent();
            using var audioContent = new StreamContent(audioStream);
            audioContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "audio/mp4" : contentType);
            form.Add(audioContent, "file", string.IsNullOrWhiteSpace(fileName) ? "voice.m4a" : fileName);
            form.Add(new StringContent(string.IsNullOrWhiteSpace(asr.Model) ? "whisper-1" : asr.Model), "model");
            request.Content = form;

            _logger.LogInformation("ASR request started. Provider={Provider}, BaseUrl={BaseUrl}, Model={Model}, FileName={FileName}", asr.Provider, asr.BaseUrl, asr.Model, fileName);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
                _logger.LogWarning("ASR request failed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, ErrorLength={ErrorLength}", (int)response.StatusCode, stopwatch.ElapsedMilliseconds, errorText.Length);
                return new SpeechToTextResult
                {
                    Success = false,
                    ErrorMessage = "语音识别失败，请重试",
                    StatusCode = (int)response.StatusCode,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, timeoutCts.Token);
            var text = TryReadString(payload, "text")
                ?? TryReadString(payload, "result")
                ?? TryReadString(payload, "transcript")
                ?? string.Empty;

            _logger.LogInformation("ASR request completed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, TextLength={TextLength}", (int)response.StatusCode, stopwatch.ElapsedMilliseconds, text.Length);
            return new SpeechToTextResult
            {
                Success = !string.IsNullOrWhiteSpace(text),
                Text = text.Trim(),
                ErrorMessage = string.IsNullOrWhiteSpace(text) ? "语音识别失败，请重试" : string.Empty,
                StatusCode = (int)response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "ASR request timed out. TimeoutSeconds={TimeoutSeconds}, ElapsedMs={ElapsedMs}", asr.TimeoutSeconds, stopwatch.ElapsedMilliseconds);
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音识别超时，请重试",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "ASR request failed unexpectedly. ElapsedMs={ElapsedMs}", stopwatch.ElapsedMilliseconds);
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音识别失败，请重试",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
    }

    private async Task<SpeechToTextResult> ConvertWithXunfeiAsync(
        Stream audioStream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var asr = _options.Asr;
        if (IsMissing(asr.BaseUrl) || IsMissing(asr.AppId) || IsMissing(asr.ApiKey) || IsMissing(asr.ApiSecret))
        {
            _logger.LogWarning("Xunfei ASR service is not configured. BaseUrl={BaseUrl}, HasAppId={HasAppId}, HasApiKey={HasApiKey}, HasApiSecret={HasApiSecret}",
                asr.BaseUrl,
                !IsMissing(asr.AppId),
                !IsMissing(asr.ApiKey),
                !IsMissing(asr.ApiSecret));
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音服务未配置"
            };
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(asr.TimeoutSeconds, 5, 300)));
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await using var audioBuffer = new MemoryStream();
            await audioStream.CopyToAsync(audioBuffer, timeoutCts.Token);
            var audioBytes = audioBuffer.ToArray();
            if (audioBytes.Length == 0)
            {
                return new SpeechToTextResult { Success = false, ErrorMessage = "录音文件为空" };
            }

            var endpoint = BuildXunfeiAuthorizedUrl(asr.BaseUrl, asr.ApiKey, asr.ApiSecret);
            using var webSocket = new ClientWebSocket();
            _logger.LogInformation("Xunfei ASR request started. BaseUrl={BaseUrl}, FileName={FileName}, Bytes={Bytes}", asr.BaseUrl, fileName, audioBytes.Length);
            await webSocket.ConnectAsync(endpoint, timeoutCts.Token);
            await SendXunfeiAsrFramesAsync(webSocket, audioBytes, asr.AppId, timeoutCts.Token);
            var text = await ReceiveXunfeiAsrTextAsync(webSocket, timeoutCts.Token);
            stopwatch.Stop();

            _logger.LogInformation("Xunfei ASR request completed. ElapsedMs={ElapsedMs}, TextLength={TextLength}", stopwatch.ElapsedMilliseconds, text.Length);
            return new SpeechToTextResult
            {
                Success = !string.IsNullOrWhiteSpace(text),
                Text = text.Trim(),
                ErrorMessage = string.IsNullOrWhiteSpace(text) ? "语音识别失败，请重试" : string.Empty,
                StatusCode = 200,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Xunfei ASR request timed out. TimeoutSeconds={TimeoutSeconds}, ElapsedMs={ElapsedMs}", asr.TimeoutSeconds, stopwatch.ElapsedMilliseconds);
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音识别超时，请重试",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Xunfei ASR request failed. ElapsedMs={ElapsedMs}", stopwatch.ElapsedMilliseconds);
            return new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "语音识别失败，请重试",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
    }

    private static bool IsMissing(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            || value.Contains("真实", StringComparison.OrdinalIgnoreCase)
            || value.Contains("your-", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task SendXunfeiAsrFramesAsync(
        ClientWebSocket webSocket,
        byte[] audioBytes,
        string appId,
        CancellationToken cancellationToken)
    {
        const int frameSize = 1280;
        var offset = 0;
        var firstFrame = true;
        while (offset < audioBytes.Length)
        {
            var length = Math.Min(frameSize, audioBytes.Length - offset);
            var audio = Convert.ToBase64String(audioBytes, offset, length);
            var status = firstFrame ? 0 : 1;
            var payload = new Dictionary<string, object?>
            {
                ["data"] = new { status, format = "audio/L16;rate=16000", encoding = "raw", audio }
            };
            if (firstFrame)
            {
                payload["common"] = new { app_id = appId };
                payload["business"] = new { language = "zh_cn", domain = "iat", accent = "mandarin" };
            }

            await SendJsonAsync(webSocket, payload, cancellationToken);
            offset += length;
            firstFrame = false;
            await Task.Delay(40, cancellationToken);
        }

        await SendJsonAsync(webSocket, new
        {
            data = new { status = 2, format = "audio/L16;rate=16000", encoding = "raw", audio = string.Empty }
        }, cancellationToken);
    }

    private static async Task<string> ReceiveXunfeiAsrTextAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        var answer = new StringBuilder();
        var buffer = new byte[8192];
        while (webSocket.State == WebSocketState.Open)
        {
            using var message = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return answer.ToString();
                }

                message.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            var json = Encoding.UTF8.GetString(message.ToArray());
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var code = root.TryGetProperty("code", out var codeElement) && codeElement.ValueKind == JsonValueKind.Number
                ? codeElement.GetInt32()
                : 0;
            if (code != 0)
            {
                var error = root.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : "Xunfei ASR failed";
                throw new InvalidOperationException(error);
            }

            if (root.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("result", out var resultElement)
                    && resultElement.TryGetProperty("ws", out var words)
                    && words.ValueKind == JsonValueKind.Array)
                {
                    foreach (var word in words.EnumerateArray())
                    {
                        if (!word.TryGetProperty("cw", out var candidates) || candidates.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (var candidate in candidates.EnumerateArray())
                        {
                            if (candidate.TryGetProperty("w", out var text) && text.ValueKind == JsonValueKind.String)
                            {
                                answer.Append(text.GetString());
                                break;
                            }
                        }
                    }
                }

                if (data.TryGetProperty("status", out var statusElement)
                    && statusElement.ValueKind == JsonValueKind.Number
                    && statusElement.GetInt32() == 2)
                {
                    return answer.ToString();
                }
            }
        }

        return answer.ToString();
    }

    private static async Task SendJsonAsync(ClientWebSocket webSocket, object payload, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
    }

    private static Uri BuildXunfeiAuthorizedUrl(string baseUrl, string apiKey, string apiSecret)
    {
        var uri = new Uri(baseUrl);
        var date = DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture);
        var signatureOrigin = $"host: {uri.Host}\ndate: {date}\nGET {uri.AbsolutePath} HTTP/1.1";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureOrigin)));
        var authorizationOrigin = $"api_key=\"{apiKey}\", algorithm=\"hmac-sha256\", headers=\"host date request-line\", signature=\"{signature}\"";
        var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(authorizationOrigin));
        var query = $"authorization={Uri.EscapeDataString(authorization)}&date={Uri.EscapeDataString(date)}&host={Uri.EscapeDataString(uri.Host)}";
        return new Uri($"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}?{query}");
    }

    private static Uri BuildEndpoint(string baseUrl, string defaultPath)
    {
        var normalized = baseUrl.Trim().TrimEnd('/');
        if (normalized.EndsWith("/audio/transcriptions", StringComparison.OrdinalIgnoreCase))
        {
            return new Uri(normalized);
        }

        return new Uri(normalized + defaultPath);
    }

    private static string? TryReadString(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }
}
