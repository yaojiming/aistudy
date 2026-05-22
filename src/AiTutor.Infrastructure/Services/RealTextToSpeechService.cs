using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using AiTutor.Shared.Voice;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Services;

public sealed class RealTextToSpeechService : ITextToSpeechService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly VoiceAiOptions _options;
    private readonly ILogger<RealTextToSpeechService> _logger;

    public RealTextToSpeechService(
        HttpClient httpClient,
        IOptions<VoiceAiOptions> options,
        ILogger<RealTextToSpeechService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<TtsResult> ConvertTextToSpeechAsync(string text, CancellationToken cancellationToken = default)
    {
        var tts = _options.Tts;
        if (string.IsNullOrWhiteSpace(text))
        {
            return new TtsResult { Success = false, ErrorMessage = "合成文本为空" };
        }

        if (tts.Provider.Equals("Xunfei", StringComparison.OrdinalIgnoreCase))
        {
            return await ConvertWithXunfeiAsync(text, cancellationToken);
        }

        if (IsMissing(tts.BaseUrl) || IsMissing(tts.ApiKey))
        {
            _logger.LogWarning("TTS service is not configured. Provider={Provider}, BaseUrl={BaseUrl}, Model={Model}, Voice={Voice}", tts.Provider, tts.BaseUrl, tts.Model, tts.Voice);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音服务未配置"
            };
        }

        var endpoint = BuildEndpoint(tts.BaseUrl, "/v1/audio/speech");
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(tts.TimeoutSeconds, 5, 300)));

        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tts.ApiKey);
            request.Content = JsonContent.Create(new
            {
                model = string.IsNullOrWhiteSpace(tts.Model) ? "tts-1" : tts.Model,
                input = text,
                voice = string.IsNullOrWhiteSpace(tts.Voice) ? "alloy" : tts.Voice,
                response_format = string.IsNullOrWhiteSpace(tts.Format) ? "mp3" : tts.Format
            }, options: JsonOptions);

            _logger.LogInformation("TTS request started. Provider={Provider}, BaseUrl={BaseUrl}, Model={Model}, Voice={Voice}, Format={Format}, TextLength={TextLength}", tts.Provider, tts.BaseUrl, tts.Model, tts.Voice, tts.Format, text.Length);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
                _logger.LogWarning("TTS request failed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, ErrorLength={ErrorLength}", (int)response.StatusCode, stopwatch.ElapsedMilliseconds, errorText.Length);
                return new TtsResult
                {
                    Success = false,
                    ErrorMessage = "语音合成失败",
                    StatusCode = (int)response.StatusCode,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
                mediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                var format = NormalizeAudioFormat(tts.Format, mediaType);
                var audioDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voice");
                Directory.CreateDirectory(audioDirectory);
                var fileName = $"tts-{Guid.NewGuid():N}.{format}";
                var audioPath = Path.Combine(audioDirectory, fileName);

                await using (var output = File.Create(audioPath))
                {
                    await response.Content.CopyToAsync(output, timeoutCts.Token);
                }

                var audioUrl = $"/voice/{fileName}";
                _logger.LogInformation("TTS request completed as binary audio. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, AudioUrl={AudioUrl}", (int)response.StatusCode, stopwatch.ElapsedMilliseconds, audioUrl);
                return new TtsResult
                {
                    Success = true,
                    AudioFilePath = audioPath,
                    AudioUrl = audioUrl,
                    StatusCode = (int)response.StatusCode,
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, timeoutCts.Token);
            var returnedUrl = TryReadString(json, "audioUrl")
                ?? TryReadString(json, "url")
                ?? TryReadString(json, "audio_url")
                ?? string.Empty;

            _logger.LogInformation("TTS request completed as JSON. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, HasAudioUrl={HasAudioUrl}", (int)response.StatusCode, stopwatch.ElapsedMilliseconds, !string.IsNullOrWhiteSpace(returnedUrl));
            return new TtsResult
            {
                Success = !string.IsNullOrWhiteSpace(returnedUrl),
                AudioUrl = returnedUrl,
                ErrorMessage = string.IsNullOrWhiteSpace(returnedUrl) ? "语音合成失败" : string.Empty,
                StatusCode = (int)response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "TTS request timed out. TimeoutSeconds={TimeoutSeconds}, ElapsedMs={ElapsedMs}", tts.TimeoutSeconds, stopwatch.ElapsedMilliseconds);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音合成超时",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "TTS request failed unexpectedly. ElapsedMs={ElapsedMs}", stopwatch.ElapsedMilliseconds);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音合成失败",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
    }

    private async Task<TtsResult> ConvertWithXunfeiAsync(string text, CancellationToken cancellationToken)
    {
        var tts = _options.Tts;
        if (IsMissing(tts.BaseUrl) || IsMissing(tts.AppId) || IsMissing(tts.ApiKey) || IsMissing(tts.ApiSecret))
        {
            _logger.LogWarning("Xunfei TTS service is not configured. BaseUrl={BaseUrl}, HasAppId={HasAppId}, HasApiKey={HasApiKey}, HasApiSecret={HasApiSecret}, Voice={Voice}",
                tts.BaseUrl,
                !IsMissing(tts.AppId),
                !IsMissing(tts.ApiKey),
                !IsMissing(tts.ApiSecret),
                tts.Voice);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音服务未配置"
            };
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(tts.TimeoutSeconds, 5, 300)));
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var endpoint = BuildXunfeiAuthorizedUrl(tts.BaseUrl, tts.ApiKey, tts.ApiSecret);
            using var webSocket = new ClientWebSocket();
            _logger.LogInformation("Xunfei TTS request started. BaseUrl={BaseUrl}, Voice={Voice}, Format={Format}, TextLength={TextLength}", tts.BaseUrl, tts.Voice, tts.Format, text.Length);
            await webSocket.ConnectAsync(endpoint, timeoutCts.Token);

            var payload = new
            {
                common = new { app_id = tts.AppId },
                business = new
                {
                    aue = ResolveXunfeiAudioEncoding(tts.Format),
                    sfl = 1,
                    auf = "audio/L16;rate=16000",
                    vcn = string.IsNullOrWhiteSpace(tts.Voice) ? "xiaoyan" : tts.Voice,
                    tte = "UTF8"
                },
                data = new
                {
                    status = 2,
                    text = Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
                }
            };

            await SendJsonAsync(webSocket, payload, timeoutCts.Token);
            var audioBytes = await ReceiveXunfeiTtsAudioAsync(webSocket, timeoutCts.Token);
            stopwatch.Stop();

            if (audioBytes.Length == 0)
            {
                return new TtsResult
                {
                    Success = false,
                    ErrorMessage = "语音合成失败",
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }

            var format = NormalizeAudioFormat(tts.Format, "audio/mpeg");
            var audioDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voice");
            Directory.CreateDirectory(audioDirectory);
            var fileName = $"tts-{Guid.NewGuid():N}.{format}";
            var audioPath = Path.Combine(audioDirectory, fileName);
            await File.WriteAllBytesAsync(audioPath, audioBytes, timeoutCts.Token);

            var audioUrl = $"/voice/{fileName}";
            _logger.LogInformation("Xunfei TTS request completed. ElapsedMs={ElapsedMs}, AudioBytes={AudioBytes}, AudioUrl={AudioUrl}", stopwatch.ElapsedMilliseconds, audioBytes.Length, audioUrl);
            return new TtsResult
            {
                Success = true,
                AudioFilePath = audioPath,
                AudioUrl = audioUrl,
                StatusCode = 200,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Xunfei TTS request timed out. TimeoutSeconds={TimeoutSeconds}, ElapsedMs={ElapsedMs}", tts.TimeoutSeconds, stopwatch.ElapsedMilliseconds);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音合成超时",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Xunfei TTS request failed. ElapsedMs={ElapsedMs}", stopwatch.ElapsedMilliseconds);
            return new TtsResult
            {
                Success = false,
                ErrorMessage = "语音合成失败",
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

    private static async Task<byte[]> ReceiveXunfeiTtsAudioAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        await using var audio = new MemoryStream();
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
                    return audio.ToArray();
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
                var error = root.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : "Xunfei TTS failed";
                throw new InvalidOperationException(error);
            }

            if (root.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("audio", out var audioElement)
                    && audioElement.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(audioElement.GetString()))
                {
                    var bytes = Convert.FromBase64String(audioElement.GetString()!);
                    await audio.WriteAsync(bytes, cancellationToken);
                }

                if (data.TryGetProperty("status", out var statusElement)
                    && statusElement.ValueKind == JsonValueKind.Number
                    && statusElement.GetInt32() == 2)
                {
                    return audio.ToArray();
                }
            }
        }

        return audio.ToArray();
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

    private static string ResolveXunfeiAudioEncoding(string format)
    {
        return format.Equals("wav", StringComparison.OrdinalIgnoreCase) || format.Equals("pcm", StringComparison.OrdinalIgnoreCase)
            ? "raw"
            : "lame";
    }

    private static Uri BuildEndpoint(string baseUrl, string defaultPath)
    {
        var normalized = baseUrl.Trim().TrimEnd('/');
        if (normalized.EndsWith("/audio/speech", StringComparison.OrdinalIgnoreCase))
        {
            return new Uri(normalized);
        }

        return new Uri(normalized + defaultPath);
    }

    private static string NormalizeAudioFormat(string configuredFormat, string mediaType)
    {
        if (!string.IsNullOrWhiteSpace(configuredFormat))
        {
            return configuredFormat.Trim().TrimStart('.').ToLowerInvariant();
        }

        return mediaType.Contains("wav", StringComparison.OrdinalIgnoreCase) ? "wav" : "mp3";
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
