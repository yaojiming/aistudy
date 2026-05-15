using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// DeepSeek 文本模型 Provider，负责把后端 Prompt 转换为 DeepSeek Chat Completion 请求。
/// </summary>
/// <remarks>
/// 调用链：ChatAgent / WrongBookAgent / TextbookRagAgent 等文本 Agent -> ITextModelProvider -> DeepSeekTextModelProvider。
/// 本类只负责外部模型 HTTP 调用和错误兜底，不直接写数据库；ModelCallLog 仍由 AgentService 统一保存。
/// </remarks>
public class DeepSeekTextModelProvider : ITextModelProvider
{
    private const string FallbackBaseUrl = "https://api.deepseek.com";

    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;
    private readonly ILogger<DeepSeekTextModelProvider> _logger;

    public DeepSeekTextModelProvider(
        HttpClient httpClient,
        IOptions<AiProviderOptions> options,
        ILogger<DeepSeekTextModelProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => "DeepSeek";

    public string ModelName => string.IsNullOrWhiteSpace(_options.DeepSeek.DefaultModel)
        ? "deepseek-chat"
        : _options.DeepSeek.DefaultModel;

    /// <summary>
    /// 调用 DeepSeek 文本模型生成完整回答。
    /// </summary>
    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (ProviderHttpHelper.IsMissingOrPlaceholder(_options.DeepSeek.ApiKey))
        {
            return "DeepSeek 文本模型还没有配置 API Key。请在后端开发配置中填写密钥，或保持 UseMock=true 继续测试。";
        }

        try
        {
            using var timeoutCts = CreateTimeoutToken(cancellationToken);
            ProviderHttpHelper.SetBearerToken(_httpClient, _options.DeepSeek.ApiKey);

            var endpoint = ProviderHttpHelper.BuildChatCompletionsUri(_options.DeepSeek.BaseUrl, FallbackBaseUrl);
            var payload = new
            {
                model = ModelName,
                messages = new[]
                {
                    new { role = "system", content = BuildSystemPrompt(enableThinking: false) },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3,
                stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync(endpoint, payload, timeoutCts.Token);
            var responseText = await response.Content.ReadAsStringAsync(timeoutCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("DeepSeek call failed. StatusCode={StatusCode}", response.StatusCode);
                return $"DeepSeek 文本模型调用失败，状态码：{(int)response.StatusCode}。请稍后再试，或切回 Mock 模式继续测试。";
            }

            var content = ProviderHttpHelper.ReadFirstChoiceContent(responseText);
            return string.IsNullOrWhiteSpace(content)
                ? "DeepSeek 已返回响应，但没有解析到有效回答内容。请检查模型响应格式。"
                : content;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("DeepSeek call timed out after {TimeoutSeconds} seconds.", _options.TimeoutSeconds);
            return "DeepSeek 文本模型响应超时了。可以稍后再试，或者先切回 Mock 模式继续调试。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeepSeek call failed with an unexpected error.");
            return "DeepSeek 文本模型调用时遇到问题。后端已经记录错误，请稍后再试。";
        }
    }

    /// <summary>
    /// 调用 DeepSeek Chat Completions 流式接口，逐段返回 choices.delta.content。
    /// </summary>
    /// <param name="prompt">渲染后的教学 Prompt。</param>
    /// <param name="enableThinking">是否启用更细致的可展示讲解。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async IAsyncEnumerable<string> GenerateStreamAsync(
        string prompt,
        bool enableThinking = false,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (ProviderHttpHelper.IsMissingOrPlaceholder(_options.DeepSeek.ApiKey))
        {
            yield return await GenerateAsync(prompt, cancellationToken);
            yield break;
        }

        using var timeoutCts = CreateTimeoutToken(cancellationToken);
        ProviderHttpHelper.SetBearerToken(_httpClient, _options.DeepSeek.ApiKey);

        var endpoint = ProviderHttpHelper.BuildChatCompletionsUri(_options.DeepSeek.BaseUrl, FallbackBaseUrl);
        var payload = new
        {
            model = ModelName,
            messages = new[]
            {
                new { role = "system", content = BuildSystemPrompt(enableThinking) },
                new { role = "user", content = prompt }
            },
            temperature = 0.3,
            stream = true
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(payload)
        };

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
        if (!response.IsSuccessStatusCode)
        {
            yield return await GenerateAsync(prompt, cancellationToken);
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(timeoutCts.Token);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(timeoutCts.Token);
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var data = line["data:".Length..].Trim();
            if (data == "[DONE]")
            {
                yield break;
            }

            var delta = ReadDeltaContent(data);
            if (!string.IsNullOrEmpty(delta))
            {
                yield return delta;
            }
        }
    }

    /// <summary>
    /// 根据思考开关生成系统提示词。文本模型没有官方 thinking 参数，这里只控制可展示讲解的细致程度。
    /// </summary>
    private static string BuildSystemPrompt(bool enableThinking)
    {
        var depth = enableThinking
            ? "请适当展开给学生看的分析步骤，说明先看什么、再怎么算、为什么这样做。"
            : "请清楚分段，步骤适中，不要额外展开长篇分析。";

        return $"你是一名耐心、鼓励、适合小学阶段学生的 AI 老师。回答要分步骤讲解，不要只给答案。{depth}";
    }

    /// <summary>
    /// 从 OpenAI 兼容流式 JSON 中读取 choices[0].delta.content。
    /// </summary>
    private static string? ReadDeltaContent(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var choice = document.RootElement.GetProperty("choices")[0];
            if (choice.TryGetProperty("delta", out var delta) &&
                delta.TryGetProperty("content", out var content))
            {
                return content.GetString();
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    /// <summary>
    /// 为单次 DeepSeek 请求创建带超时的取消令牌。
    /// </summary>
    private CancellationTokenSource CreateTimeoutToken(CancellationToken cancellationToken)
    {
        var timeoutSeconds = _options.TimeoutSeconds <= 0 ? 30 : _options.TimeoutSeconds;
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return timeoutCts;
    }
}
