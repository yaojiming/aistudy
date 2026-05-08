using System.Net.Http.Json;
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
/// 当 UseMock=false 且 DeepSeek ApiKey 正确配置时，依赖注入会把 ITextModelProvider 切换到本实现。
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
    /// 调用 DeepSeek 文本模型生成回答。
    /// </summary>
    /// <param name="prompt">PromptTemplateService 渲染后的完整教学 Prompt。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>模型生成的回答；异常或配置缺失时返回友好错误文本。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 校验 ApiKey，避免把占位符密钥发给 DeepSeek；
    /// 2. 使用 IHttpClientFactory 注入的 HttpClient 发送 chat/completions 请求；
    /// 3. 使用 TimeoutSeconds 创建本次请求的超时取消令牌；
    /// 4. 解析 choices[0].message.content；
    /// 5. 任何 HTTP、超时或 JSON 异常都记录安全日志，并返回可展示给学生的提示。
    /// </remarks>
    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (ProviderHttpHelper.IsMissingOrPlaceholder(_options.DeepSeek.ApiKey))
        {
            return "DeepSeek 文本模型还没有配置 API Key。请让家长或开发者在后端开发配置中填写密钥，当前不会影响 Mock 模式测试。";
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
                    new { role = "system", content = "你是一名耐心、鼓励、适合小学阶段学生的 AI 老师。回答要分步骤讲解，不要只给答案。" },
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
    /// 为单次 DeepSeek 请求创建带超时的取消令牌。
    /// </summary>
    /// <param name="cancellationToken">ASP.NET Core 请求传入的原始取消令牌。</param>
    /// <returns>链接原始取消令牌并附加 TimeoutSeconds 的 CancellationTokenSource。</returns>
    /// <remarks>
    /// 调用链：GenerateAsync -> CreateTimeoutToken -> HttpClient.PostAsJsonAsync。
    /// 这样既能响应客户端取消，也能避免外部模型长时间无响应拖住 API 线程。
    /// </remarks>
    private CancellationTokenSource CreateTimeoutToken(CancellationToken cancellationToken)
    {
        var timeoutSeconds = _options.TimeoutSeconds <= 0 ? 30 : _options.TimeoutSeconds;
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return timeoutCts;
    }
}
