namespace AiTutor.Infrastructure.Options;

/// <summary>
/// AI Provider 配置根对象，对应 appsettings.json 中的 AiProviders 节点。
/// </summary>
/// <remarks>
/// 调用链：Program -> AddAiTutorInfrastructure -> IOptions&lt;AiProviderOptions&gt; -> Provider 与 AgentRouter。
/// 本类只承载配置，不保存任何真实密钥；真实 API Key 应放在 appsettings.Development.json、用户机密或环境变量中。
/// UseMock 控制依赖注入最终注入 Mock Provider 还是真实 DeepSeek / GLM Provider。
/// </remarks>
public class AiProviderOptions
{
    /// <summary>
    /// 是否使用 Mock Provider。
    /// </summary>
    /// <remarks>
    /// true 时后端不需要真实 API Key，Swagger 可以直接跑通；false 时会切换到真实 Provider，
    /// Provider 会校验 ApiKey，缺失时返回友好的错误文本而不是让后端崩溃。
    /// </remarks>
    public bool UseMock { get; set; } = true;

    /// <summary>
    /// 调用外部大模型接口的超时时间，单位为秒。
    /// </summary>
    /// <remarks>
    /// DeepSeekTextModelProvider 和 GlmVisionModelProvider 会为每次 HTTP 请求创建链接取消令牌，
    /// 当接口超过该时间未响应时主动取消，并返回适合小学生场景的友好提示。
    /// </remarks>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// DeepSeek 文本模型配置。
    /// </summary>
    public DeepSeekOptions DeepSeek { get; set; } = new();

    /// <summary>
    /// 智谱 GLM 视觉模型配置。
    /// </summary>
    public ZhipuOptions Zhipu { get; set; } = new();
}

/// <summary>
/// DeepSeek 文本模型配置项。
/// </summary>
/// <remarks>
/// BaseUrl 支持填写平台根地址，例如 https://api.deepseek.com，也支持直接填写 chat/completions 端点。
/// ApiKey 不应写入默认 appsettings.json 的真实值。
/// </remarks>
public class DeepSeekOptions
{
    public string BaseUrl { get; set; } = "https://api.deepseek.com";

    public string ApiKey { get; set; } = string.Empty;

    public string DefaultModel { get; set; } = "deepseek-chat";
}

/// <summary>
/// 智谱 GLM 视觉模型配置项。
/// </summary>
/// <remarks>
/// VisionModel 用于图片讲题和作业检查场景；当前 Provider 按 OpenAI 兼容的 chat/completions 结构调用，
/// 便于后续替换为平台专属 SDK 或接口细节。
/// </remarks>
public class ZhipuOptions
{
    public string BaseUrl { get; set; } = "https://open.bigmodel.cn/api/paas/v4";

    public string ApiKey { get; set; } = string.Empty;

    public string VisionModel { get; set; } = "glm-4v-plus";
}
