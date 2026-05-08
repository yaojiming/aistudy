using System.Net.Http.Headers;
using System.Text.Json;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 真实模型 Provider 共用的 HTTP 与 JSON 辅助方法。
/// </summary>
/// <remarks>
/// 调用链：DeepSeekTextModelProvider / GlmVisionModelProvider -> ProviderHttpHelper。
/// 这里集中处理鉴权 Header、chat/completions 地址拼接和响应解析，避免两个 Provider 重复实现同一段易错逻辑。
/// 该类不会记录或输出 API Key，确保日志与异常提示里不泄露密钥。
/// </remarks>
internal static class ProviderHttpHelper
{
    /// <summary>
    /// 为 HttpClient 设置 Bearer Token。
    /// </summary>
    /// <param name="client">由 IHttpClientFactory 创建并注入的 HttpClient。</param>
    /// <param name="apiKey">后端配置读取到的 API Key。</param>
    /// <remarks>
    /// 调用链：真实 Provider 在发起请求前调用本方法。
    /// 本方法只把密钥放进 Authorization Header，不写日志、不拼接到 URL，也不返回给上层。
    /// </remarks>
    public static void SetBearerToken(HttpClient client, string apiKey)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    /// 根据配置的 BaseUrl 生成 chat/completions 请求地址。
    /// </summary>
    /// <param name="baseUrl">平台根地址或完整 chat/completions 地址。</param>
    /// <param name="fallbackBaseUrl">配置为空时使用的默认平台根地址。</param>
    /// <returns>可直接用于 PostAsJsonAsync 的绝对 URI。</returns>
    /// <remarks>
    /// 调用链：真实 Provider 构建 HTTP 请求时调用。
    /// 如果用户把 BaseUrl 配成平台根地址，会自动追加 /chat/completions；
    /// 如果已经配置到 chat/completions，则直接使用，减少本地开发配置出错的概率。
    /// </remarks>
    public static Uri BuildChatCompletionsUri(string? baseUrl, string fallbackBaseUrl)
    {
        var value = string.IsNullOrWhiteSpace(baseUrl) ? fallbackBaseUrl : baseUrl.Trim();
        value = value.TrimEnd('/');

        if (!value.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase))
        {
            value += "/chat/completions";
        }

        return new Uri(value, UriKind.Absolute);
    }

    /// <summary>
    /// 从 OpenAI 兼容响应中提取第一个候选回答文本。
    /// </summary>
    /// <param name="json">模型接口返回的 JSON 字符串。</param>
    /// <returns>模型回答文本；如果结构不符合预期则返回空字符串。</returns>
    /// <remarks>
    /// 调用链：真实 Provider 收到 HTTP 响应后调用。
    /// DeepSeek 与智谱 OpenAI 兼容接口通常都会返回 choices[0].message.content，
    /// 因此统一在这里解析，Provider 只关心业务错误处理。
    /// </remarks>
    public static string ReadFirstChoiceContent(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("choices", out var choices) ||
            choices.ValueKind != JsonValueKind.Array ||
            choices.GetArrayLength() == 0)
        {
            return string.Empty;
        }

        var first = choices[0];
        if (!first.TryGetProperty("message", out var message) ||
            !message.TryGetProperty("content", out var content))
        {
            return string.Empty;
        }

        return content.ValueKind == JsonValueKind.String ? content.GetString() ?? string.Empty : content.ToString();
    }

    /// <summary>
    /// 判断配置值是否仍然是默认占位符。
    /// </summary>
    /// <param name="value">待检查的配置值。</param>
    /// <returns>为空或包含 YOUR_ 前缀时返回 true。</returns>
    /// <remarks>
    /// 调用链：真实 Provider 在请求外部模型前校验 ApiKey。
    /// 这样 UseMock=false 但未配置真实密钥时，会得到明确提示，而不是把占位符发给外部平台。
    /// </remarks>
    public static bool IsMissingOrPlaceholder(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ||
            value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase);
    }
}
