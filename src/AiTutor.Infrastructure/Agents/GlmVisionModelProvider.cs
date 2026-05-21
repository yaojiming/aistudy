using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 鏅鸿氨 GLM 瑙嗚妯″瀷 Provider锛岃礋璐ｅ浘鐗囪棰樹笌浣滀笟妫€鏌ョ殑鐪熷疄瑙嗚妯″瀷璋冪敤銆?/// </summary>
/// <remarks>
/// 璋冪敤閾撅細VisionAgent / HomeworkCheckAgent -> IVisionModelProvider -> GlmVisionModelProvider銆?/// 鏈被閫氳繃 IHttpClientFactory 娉ㄥ叆 HttpClient锛屽苟鎸?OpenAI 鍏煎鐨勫妯℃€佹秷鎭牸寮忔彁浜ゅ浘鐗囧湴鍧€鍜屾暀瀛?Prompt銆?/// 鏁版嵁搴撳瓨鍌ㄣ€丄gentResponse 缁勮鍜?ModelCallLog 浠嶇敱涓婂眰 Agent 涓?AgentService 璐熻矗锛孭rovider 涓嶇洿鎺ユ搷浣?DbContext銆?/// </remarks>
public class GlmVisionModelProvider : IVisionModelProvider
{
    private const string FallbackBaseUrl = "https://open.bigmodel.cn/api/paas/v4";

    private readonly HttpClient _httpClient;
    private readonly AiProviderOptions _options;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<GlmVisionModelProvider> _logger;

    public GlmVisionModelProvider(
        HttpClient httpClient,
        IOptions<AiProviderOptions> options,
        IHostEnvironment hostEnvironment,
        ILogger<GlmVisionModelProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public string ProviderName => "GlmVision";

    public string ModelName => string.IsNullOrWhiteSpace(_options.Zhipu.VisionModel)
        ? "glm-4v-plus"
        : _options.Zhipu.VisionModel;

    /// <summary>
    /// 璋冪敤 GLM 瑙嗚妯″瀷璇嗗埆鍥剧墖骞剁敓鎴愯瑙ｃ€?    /// </summary>
    /// <param name="imageUrl">鍓嶇浼犲叆鐨勫浘鐗?URL 鎴栧彲璁块棶鍥剧墖鍦板潃銆?/param>
    /// <param name="prompt">PromptTemplateService 娓叉煋鍚庣殑鍥剧墖璁查鎴栦綔涓氭鏌?Prompt銆?/param>
    /// <param name="cancellationToken">璇锋眰鍙栨秷浠ょ墝銆?/param>
    /// <returns>瑙嗚妯″瀷杩斿洖鐨勮瘑鍒笌璁茶В鏂囨湰锛涘紓甯告垨閰嶇疆缂哄け鏃惰繑鍥炲弸濂介敊璇枃鏈€?/returns>
    /// <remarks>
    /// 浠ｇ爜閫昏緫锛?    /// 1. 鏍￠獙 Zhipu ApiKey 鍜屽浘鐗囧湴鍧€锛?    /// 2. 浣跨敤 Bearer Token 閴存潈锛?    /// 3. 灏?prompt 涓?image_url 鏀惧叆鍚屼竴鏉?user message 鐨?content 鏁扮粍锛?    /// 4. 閫氳繃 TimeoutSeconds 鎺у埗 HTTP 璇锋眰鏃堕暱锛?    /// 5. 瑙ｆ瀽 choices[0].message.content锛?    /// 6. 鍑洪敊鏃跺彧璁板綍瀹夊叏鏃ュ織锛屼笉娉勯湶 API Key锛屼笉璁╁悗绔穿婧冦€?    /// </remarks>
    public async Task<string> AnalyzeImageAsync(
        string imageUrl,
        string prompt,
        bool enableThinking = false,
        string? modelName = null,
        CancellationToken cancellationToken = default)
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
            var modelImageUrl = await ResolveModelImageUrlAsync(imageUrl, timeoutCts.Token);

            var endpoint = ProviderHttpHelper.BuildChatCompletionsUri(_options.Zhipu.BaseUrl, FallbackBaseUrl);
            var requestModelName = ResolveModelName(modelName);
            _logger.LogInformation(
                "GLM vision request prepared. Model={Model}, EnableThinking={EnableThinking}, ThinkingType={ThinkingType}, Stream={Stream}, PromptLength={PromptLength}",
                requestModelName,
                enableThinking,
                GetThinkingType(enableThinking),
                false,
                prompt.Length);

            using var response = await SendWith429RetryAsync(
                () => _httpClient.PostAsJsonAsync(endpoint, BuildVisionPayload(requestModelName, prompt, modelImageUrl, false, enableThinking, includeThinking: true), timeoutCts.Token),
                timeoutCts.Token);
            var responseText = await response.Content.ReadAsStringAsync(timeoutCts.Token);

            if (!response.IsSuccessStatusCode && !IsRateLimited(response) && IsThinkingUnsupported(response, responseText))
            {
                _logger.LogWarning(
                    "GLM vision model does not support thinking parameter. Model={Model}, EnableThinking={EnableThinking}. Retrying without thinking.",
                    requestModelName,
                    enableThinking);

                using var fallbackResponse = await SendWith429RetryAsync(
                    () => _httpClient.PostAsJsonAsync(endpoint, BuildVisionPayload(requestModelName, prompt, modelImageUrl, false, enableThinking, includeThinking: false), timeoutCts.Token),
                    timeoutCts.Token);
                responseText = await fallbackResponse.Content.ReadAsStringAsync(timeoutCts.Token);

                if (!fallbackResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GLM vision fallback call failed. StatusCode={StatusCode}, Body={Body}", fallbackResponse.StatusCode, TruncateForLog(responseText));
                    return IsRateLimited(fallbackResponse)
                        ? "GLM 视觉模型当前请求过于频繁或额度受限，请稍后再试。可以先等待 1 分钟，或临时切回 Mock 模式继续测试。"
                        : $"GLM 视觉模型调用失败，状态码：{(int)fallbackResponse.StatusCode}。请稍后再试，或切回 Mock 模式继续测试。";
                }

                var fallbackContent = ProviderHttpHelper.ReadFirstChoiceContent(responseText);
                return string.IsNullOrWhiteSpace(fallbackContent)
                    ? "GLM 视觉模型已返回响应，但没有解析到有效图片讲解内容。请检查模型响应格式。"
                    : fallbackContent;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GLM vision call failed. StatusCode={StatusCode}, Body={Body}", response.StatusCode, TruncateForLog(responseText));
                if (IsRateLimited(response))
                {
                    return "GLM 视觉模型当前请求过于频繁或额度受限，请稍后再试。可以先等待 1 分钟，或临时切回 Mock 模式继续测试。";
                }

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
    /// 璋冪敤 GLM 瑙嗚妯″瀷鐪熷疄娴佸紡鎺ュ彛锛岄€愭杩斿洖鍥剧墖璇嗗埆鍜岃瑙ｅ唴瀹广€?    /// </summary>
    /// <param name="imageUrl">鍙緵妯″瀷璁块棶鐨勫浘鐗囧湴鍧€鎴栧悗绔浘鐗囪矾寰勩€?/param>
    /// <param name="prompt">娓叉煋鍚庣殑瑙嗚 Prompt銆?/param>
    /// <param name="enableThinking">是否开启 GLM 官方 thinking 模式。</param>
    /// <param name="cancellationToken">鍙栨秷浠ょ墝銆?/param>
    /// <returns>瑙嗚妯″瀷澧為噺杈撳嚭鐗囨銆?/returns>
    /// <remarks>
    /// 璋冪敤閾撅細VisionAgent/HomeworkCheckAgent.StreamExecuteAsync -> IVisionModelProvider.AnalyzeImageStreamAsync銆?    /// 鏈柟娉曡缃?stream=true锛屽苟鎸?OpenAI 鍏煎 SSE 鍗忚瑙ｆ瀽 delta銆?    /// </remarks>
    public async IAsyncEnumerable<string> AnalyzeImageStreamAsync(
        string imageUrl,
        string prompt,
        bool enableThinking = false,
        string? modelName = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (ProviderHttpHelper.IsMissingOrPlaceholder(_options.Zhipu.ApiKey))
        {
            yield return "GLM 视觉模型还没有配置 API Key。请在后端开发配置中填写 Zhipu 密钥，或保持 UseMock=true 继续测试。";
            yield break;
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            yield return "没有收到图片地址，所以暂时不能进行图片讲题。请重新上传图片后再试。";
            yield break;
        }

        using var timeoutCts = CreateTimeoutToken(cancellationToken);
        ProviderHttpHelper.SetBearerToken(_httpClient, _options.Zhipu.ApiKey);
        var modelImageUrl = await ResolveModelImageUrlAsync(imageUrl, timeoutCts.Token);

        var endpoint = ProviderHttpHelper.BuildChatCompletionsUri(_options.Zhipu.BaseUrl, FallbackBaseUrl);
        var requestModelName = ResolveModelName(modelName);
        _logger.LogInformation(
            "GLM vision stream request prepared. Model={Model}, EnableThinking={EnableThinking}, ThinkingType={ThinkingType}, Stream={Stream}, PromptLength={PromptLength}",
            requestModelName,
            enableThinking,
            GetThinkingType(enableThinking),
            true,
            prompt.Length);

        yield return "AI老师正在读题...\n\n";

        HttpResponseMessage? response = null;
        var timeoutMessage = string.Empty;
        try
        {
            using var request = CreateVisionRequest(endpoint, requestModelName, prompt, modelImageUrl, true, enableThinking, includeThinking: true);
            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("GLM vision stream call timed out before response headers after {TimeoutSeconds} seconds.", _options.TimeoutSeconds);
            timeoutMessage = "AI老师读图时间有点久，这次请求超时了。可以再试一次，或把题目裁剪得更清楚一些。";
        }

        if (!string.IsNullOrWhiteSpace(timeoutMessage) || response is null)
        {
            yield return timeoutMessage;
            yield break;
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            if (!IsRateLimited(response) && IsThinkingUnsupported(response, errorText))
            {
                _logger.LogWarning(
                    "GLM vision model does not support thinking parameter. Model={Model}, EnableThinking={EnableThinking}. Retrying stream without thinking.",
                    requestModelName,
                    enableThinking);

                response.Dispose();
                using var fallbackRequest = CreateVisionRequest(endpoint, requestModelName, prompt, modelImageUrl, true, enableThinking, includeThinking: false);
                response = await _httpClient.SendAsync(fallbackRequest, HttpCompletionOption.ResponseHeadersRead, timeoutCts.Token);
                if (response.IsSuccessStatusCode)
                {
                    goto StreamResponseReady;
                }

                errorText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            }

            _logger.LogWarning("GLM vision stream call failed. StatusCode={StatusCode}, Body={Body}", response.StatusCode, TruncateForLog(errorText));
            if (IsRateLimited(response))
            {
                yield return "GLM 视觉模型当前请求过于频繁或额度受限，请稍后再试。可以先等待 1 分钟，或临时切回 Mock 模式继续测试。";
                yield break;
            }

            yield return "GLM 视觉模型流式请求失败。状态码：" + (int)response.StatusCode + "。请稍后再试。";
            yield break;
        }

    StreamResponseReady:
        yield return "AI老师正在分析题目...\n\n";

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (!string.Equals(mediaType, "text/event-stream", StringComparison.OrdinalIgnoreCase))
        {
            var responseText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
            _logger.LogInformation("GLM vision stream endpoint returned non-SSE response. ContentType={ContentType}, Body={Body}", mediaType, TruncateForLog(responseText));
            var content = ProviderHttpHelper.ReadFirstChoiceContent(responseText);
            yield return string.IsNullOrWhiteSpace(content)
                ? "AI老师收到了模型响应，但没有解析到讲解内容。请检查 GLM 返回格式。"
                : content;
            yield break;
        }

        Stream? stream = null;
        timeoutMessage = string.Empty;
        try
        {
            stream = await response.Content.ReadAsStreamAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("GLM vision stream call timed out while opening response stream after {TimeoutSeconds} seconds.", _options.TimeoutSeconds);
            timeoutMessage = "AI老师分析题目时间过长，这次请求超时了。请稍后再试。";
        }

        if (!string.IsNullOrWhiteSpace(timeoutMessage) || stream is null)
        {
            yield return timeoutMessage;
            yield break;
        }

        await using var streamScope = stream;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var hasStartedReasoningSection = false;
        var hasStartedContentSection = false;
        while (!reader.EndOfStream)
        {
            string? line;
            timeoutMessage = string.Empty;
            try
            {
                line = await reader.ReadLineAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("GLM vision stream call timed out while reading SSE after {TimeoutSeconds} seconds.", _options.TimeoutSeconds);
                timeoutMessage = "\n\nAI老师这次思考太久，请稍后再试，或把图片裁剪得更清晰一些。";
                line = null;
            }

            if (!string.IsNullOrWhiteSpace(timeoutMessage))
            {
                yield return timeoutMessage;
                yield break;
            }

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
            if (!string.IsNullOrEmpty(delta.Text))
            {
                _logger.LogDebug("GLM vision stream delta. Kind={Kind}, Length={Length}", delta.Kind, delta.Text.Length);

                if (delta.Kind == ModelDeltaKind.Reasoning)
                {
                    if (!enableThinking)
                    {
                        continue;
                    }

                    if (!hasStartedReasoningSection)
                    {
                        hasStartedReasoningSection = true;
                        _logger.LogInformation("GLM vision stream reasoning started. Model={Model}, EnableThinking={EnableThinking}", requestModelName, enableThinking);
                        yield return "\n\n【思路分析】\n";
                    }

                    yield return delta.Text;
                    continue;
                }

                if (!hasStartedContentSection)
                {
                    hasStartedContentSection = true;
                    _logger.LogInformation("GLM vision stream content started. Model={Model}, HasReasoning={HasReasoning}", requestModelName, hasStartedReasoningSection);
                    if (hasStartedReasoningSection)
                    {
                        yield return "\n\n【正式讲解】\n";
                    }
                }

                yield return delta.Text;
            }
        }
    }
    private string ResolveModelName(string? modelName)
    {
        return string.IsNullOrWhiteSpace(modelName) ? ModelName : modelName.Trim();
    }

    /// <summary>
    /// 构造 GLM 官方 thinking 参数。true 开启官方思考，false 显式关闭。
    /// </summary>
    private static object BuildThinkingOptions(bool enableThinking)
    {
        return new
        {
            type = GetThinkingType(enableThinking)
        };
    }

    private static string GetThinkingType(bool enableThinking)
    {
        return enableThinking ? "enabled" : "disabled";
    }

    private static Dictionary<string, object?> BuildVisionPayload(
        string model,
        string prompt,
        string modelImageUrl,
        bool stream,
        bool enableThinking,
        bool includeThinking)
    {
        var payload = new Dictionary<string, object?>
        {
            ["model"] = model,
            ["messages"] = new object[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = prompt },
                        new { type = "image_url", image_url = new { url = modelImageUrl } }
                    }
                }
            },
            ["temperature"] = 0.2,
            ["stream"] = stream
        };

        if (includeThinking)
        {
            payload["thinking"] = BuildThinkingOptions(enableThinking);
        }

        return payload;
    }

    private static HttpRequestMessage CreateVisionRequest(
        Uri endpoint,
        string model,
        string prompt,
        string modelImageUrl,
        bool stream,
        bool enableThinking,
        bool includeThinking)
    {
        return new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(BuildVisionPayload(model, prompt, modelImageUrl, stream, enableThinking, includeThinking))
        };
    }

    private static bool IsThinkingUnsupported(HttpResponseMessage response, string? responseText)
    {
        if ((int)response.StatusCode != 400)
        {
            return false;
        }

        var text = responseText ?? string.Empty;
        return text.Contains("thinking", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("unsupported", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("invalid parameter", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("invalid_param", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 浠?OpenAI 鍏煎娴佸紡 JSON 涓鍙?choices[0].delta.content銆?    /// </summary>
    /// <param name="json">鍗曟潯 SSE data 鐨?JSON 鍐呭銆?/param>
    /// <returns>澧為噺鏂囨湰锛涙棤娉曡В鏋愭椂杩斿洖 null銆?/returns>
    private static ModelDelta ReadDeltaContent(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var choice = document.RootElement.GetProperty("choices")[0];

            if (choice.TryGetProperty("delta", out var delta) &&
                (delta.TryGetProperty("reasoning_content", out var reasoning) ||
                 delta.TryGetProperty("reasoning", out reasoning) ||
                 delta.TryGetProperty("thought", out reasoning)))
            {
                var reasoningText = ReadContentElement(reasoning);
                if (!string.IsNullOrWhiteSpace(reasoningText))
                {
                    return new ModelDelta(ModelDeltaKind.Reasoning, reasoningText);
                }
            }

            if (choice.TryGetProperty("delta", out delta) &&
                delta.TryGetProperty("content", out var content))
            {
                var contentText = ReadContentElement(content);
                if (!string.IsNullOrEmpty(contentText))
                {
                    return new ModelDelta(ModelDeltaKind.Content, contentText);
                }
            }

            if (choice.TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out content))
            {
                var contentText = ReadContentElement(content);
                if (!string.IsNullOrEmpty(contentText))
                {
                    return new ModelDelta(ModelDeltaKind.Content, contentText);
                }
            }
        }
        catch
        {
            return ModelDelta.Empty;
        }

        return ModelDelta.Empty;
    }

    /// <summary>
    /// 兼容不同 OpenAI 风格响应中的 content 字段：字符串直接返回，数组则拼接其中的 text。
    /// </summary>
    /// <param name="content">模型返回的 content JSON 节点。</param>
    /// <returns>可展示的增量文本；无法识别时返回 null。</returns>
    private static string? ReadContentElement(JsonElement content)
    {
        if (content.ValueKind == JsonValueKind.String)
        {
            return content.GetString();
        }

        if (content.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var builder = new StringBuilder();
        foreach (var item in content.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.String)
            {
                builder.Append(item.GetString());
                continue;
            }

            if (item.ValueKind == JsonValueKind.Object &&
                item.TryGetProperty("text", out var text) &&
                text.ValueKind == JsonValueKind.String)
            {
                builder.Append(text.GetString());
            }
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    /// <summary>
    /// 涓哄崟娆?GLM 瑙嗚璇锋眰鍒涘缓甯﹁秴鏃剁殑鍙栨秷浠ょ墝銆?    /// </summary>
    /// <param name="cancellationToken">ASP.NET Core 璇锋眰浼犲叆鐨勫師濮嬪彇娑堜护鐗屻€?/param>
    /// <returns>閾炬帴鍘熷鍙栨秷浠ょ墝骞堕檮鍔?TimeoutSeconds 鐨?CancellationTokenSource銆?/returns>
    /// <remarks>
    /// 璋冪敤閾撅細AnalyzeImageAsync -> CreateTimeoutToken -> HttpClient.PostAsJsonAsync銆?    /// 瑙嗚妯″瀷閫氬父姣旂函鏂囨湰妯″瀷鏇存參锛屽崟娆¤秴鏃舵帶鍒跺彲浠ラ伩鍏?Swagger 鎴栧鎴风闀挎椂闂寸瓑寰呫€?    /// </remarks>
    private CancellationTokenSource CreateTimeoutToken(CancellationToken cancellationToken)
    {
        var timeoutSeconds = _options.TimeoutSeconds <= 0 ? 180 : Math.Max(_options.TimeoutSeconds, 180);
        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return timeoutCts;
    }

    /// <summary>
    /// 灏嗘湰鍦板獟浣撹矾寰勮浆鎹负瑙嗚妯″瀷鍙鍙栫殑鍥剧墖鍦板潃銆?    /// </summary>
    /// <remarks>
    /// GLM 浜戠鏃犳硶璁块棶 /uploads/... 杩欑鍚庣鏈満鐩稿璺緞锛涚湡瀹炶皟鐢ㄥ墠杞负 data URL銆?    /// MAUI 褰撳墠鍙笂浼犵敤鎴风‘璁ゅ悗鐨勮鍓浘锛屾墍浠ヨ繖閲岀紪鐮佺殑涔熸槸瑁佸壀鍚庣殑棰樼洰鍥剧墖銆?    /// </remarks>
    private async Task<string> ResolveModelImageUrlAsync(string imageUrl, CancellationToken cancellationToken)
    {
        if (imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl;
        }

        var relativePath = imageUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", relativePath);
        if (!File.Exists(physicalPath))
        {
            _logger.LogWarning("GLM vision image file not found. ImageUrl={ImageUrl}, PhysicalPath={PhysicalPath}", imageUrl, physicalPath);
            return imageUrl;
        }

        var bytes = await File.ReadAllBytesAsync(physicalPath, cancellationToken);
        var mimeType = GetImageMimeType(physicalPath);
        _logger.LogInformation("GLM vision local image converted to data URL. ImageUrl={ImageUrl}, Bytes={Bytes}", imageUrl, bytes.Length);
        return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
    }

    private static string GetImageMimeType(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "image/jpeg"
        };
    }

    private static string TruncateForLog(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return text.Length <= 1000 ? text : text[..1000] + "...";
    }

    private enum ModelDeltaKind
    {
        None,
        Content,
        Reasoning
    }

    private readonly record struct ModelDelta(ModelDeltaKind Kind, string? Text)
    {
        public static ModelDelta Empty => new(ModelDeltaKind.None, null);
    }

    private async Task<HttpResponseMessage> SendWith429RetryAsync(
        Func<Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken)
    {
        var response = await sendAsync();
        if (!IsRateLimited(response)) return response;

        var waitSeconds = Math.Max(GetRetryAfterSeconds(response), 35);
        _logger.LogWarning("GLM 429 rate limited, waiting {WaitSeconds}s before single retry", waitSeconds);
        response.Dispose();
        await Task.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken);
        return await sendAsync();
    }

    private static bool IsRateLimited(HttpResponseMessage response)
    {
        return (int)response.StatusCode == 429;
    }

    private static double GetRetryAfterSeconds(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta is { } delta && delta > TimeSpan.Zero)
            return Math.Min(delta.TotalSeconds, 60);
        if (response.Headers.RetryAfter?.Date is { } date)
        {
            var s = (date - DateTimeOffset.UtcNow).TotalSeconds;
            return s > 0 ? Math.Min(s, 60) : 0;
        }
        return 0;
    }
}
