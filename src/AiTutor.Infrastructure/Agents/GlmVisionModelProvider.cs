using System.Net.Http.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 智谱 GLM 视觉模型 Provider，负责图片讲题与作业检查的真实视觉模型调用。
/// </summary>
/// <remarks>
/// 调用链：VisionAgent / HomeworkCheckAgent -> IVisionModelProvider -> GlmVisionModelProvider。
/// 本类通过 IHttpClientFactory 注入 HttpClient，并按 OpenAI 兼容的多模态消息格式提交图片地址和教学 Prompt。
/// 数据库存储、AgentResponse 组装和 ModelCallLog 仍由上层 Agent 与 AgentService 负责，Provider 不直接操作 DbContext。
/// </remarks>
public class GlmVisionModelProvider : IVisionModelProvider
{
    private const string FallbackBaseUrl = "https://open.bigmodel.cn/api/paas/v4";

    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;
    private readonly ILogger<GlmVisionModelProvider> _logger;

    public GlmVisionModelProvider(
        HttpClient httpClient,
        IOptions<AiProviderOptions> options,
        ILogger<GlmVisionModelProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public string ProviderName => "GlmVision";

    public string ModelName => string.IsNullOrWhiteSpace(_options.Zhipu.VisionModel)
        ? "glm-4v-plus"
        : _options.Zhipu.VisionModel;

    /// <summary>
    /// 调用 GLM 视觉模型识别图片并生成讲解。
    /// </summary>
    /// <param name="imageUrl">前端传入的图片 URL 或可访问图片地址。</param>
    /// <param name="prompt">PromptTemplateService 渲染后的图片讲题或作业检查 Prompt。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>视觉模型返回的识别与讲解文本；异常或配置缺失时返回友好错误文本。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 校验 Zhipu ApiKey 和图片地址；
    /// 2. 使用 Bearer Token 鉴权；
    /// 3. 将 prompt 与 image_url 放入同一条 user message 的 content 数组；
    /// 4. 通过 TimeoutSeconds 控制 HTTP 请求时长；
    /// 5. 解析 choices[0].message.content；
    /// 6. 出错时只记录安全日志，不泄露 API Key，不让后端崩溃。
    /// </remarks>
    public async Task<string> AnalyzeImageAsync(string imageUrl, string prompt, CancellationToken cancellationToken = default)
    {
        if (ProviderHttpHelper.IsMissingOrPlaceholder(_options.Zhipu.ApiKey))
        {
            return "GLM 视觉模型还没有配置 API Key。请在后端开发配置中填写 Zhipu 密钥，或保持 UseMock=true 继续测试。";
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return "没有收到图片地址，所以暂时不能进行图片讲题。请重新上传图片后再试。";
        }

        try
        {
            using var timeoutCts = CreateTimeoutToken(cancellationToken);
            ProviderHttpHelper.SetBearerToken(_httpClient, _options.Zhipu.ApiKey);

            var endpoint = ProviderHttpHelper.BuildChatCompletionsUri(_options.Zhipu.BaseUrl, FallbackBaseUrl);
            var payload = new
            {
                model = ModelName,
                messages = new object[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = prompt },
                            new { type = "image_url", image_url = new { url = imageUrl } }
                        }
                    }
                },
                temperature = 0.2,
                stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync(endpoint, payload, timeoutCts.Token);
            var responseText = await response.Content.ReadAsStringAsync(timeoutCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GLM vision call failed. StatusCode={StatusCode}", response.StatusCode);
                return $"GLM 视觉模型调用失败，状态码：{(int)response.StatusCode}。请稍后再试，或切回 Mock 模式继续测试。";
            }

            var content = ProviderHttpHelper.ReadFirstChoiceContent(responseText);
            return string.IsNullOrWhiteSpace(content)
                ? "GLM 视觉模型已返回响应，但没有解析到有效图片讲解内容。请检查模型响应格式。"
                : content;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("GLM vision call timed out after {TimeoutSeconds} seconds.", _options.TimeoutSeconds);
            return "GLM 视觉模型响应超时了。可以稍后再试，或者先切回 Mock 模式继续调试。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GLM vision call failed with an unexpected error.");
            return "GLM 视觉模型调用时遇到问题。后端已经记录错误，请稍后再试。";
        }
    }

    /// <summary>
    /// 为单次 GLM 视觉请求创建带超时的取消令牌。
    /// </summary>
    /// <param name="cancellationToken">ASP.NET Core 请求传入的原始取消令牌。</param>
    /// <returns>链接原始取消令牌并附加 TimeoutSeconds 的 CancellationTokenSource。</returns>
    /// <remarks>
    /// 调用链：AnalyzeImageAsync -> CreateTimeoutToken -> HttpClient.PostAsJsonAsync。
    /// 视觉模型通常比纯文本模型更慢，单次超时控制可以避免 Swagger 或客户端长时间等待。
    /// </remarks>
    private CancellationTokenSource CreateTimeoutToken(CancellationToken cancellationToken)
    {
        var timeoutSeconds = _options.TimeoutSeconds <= 0 ? 30 : _options.TimeoutSeconds;
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return timeoutCts;
    }
}
