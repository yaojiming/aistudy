using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Options;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// Agent 规则路由器，负责根据请求模式和输入类型选择具体 Agent。
/// </summary>
/// <remarks>
/// 调用链：AgentService.AskAsync -> AgentRouter.Route -> AgentService 根据 AgentName 找到具体 IAgent。
/// Phase 3 仍然保持规则路由，不引入 LLM 路由；新增的职责是根据 AiProviders:UseMock 记录当前会使用的模型提供方和模型名。
/// </remarks>
public class AgentRouter : IAgentRouter
{
    private readonly AiProviderOptions _options;

    public AgentRouter(IOptions<AiProviderOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// 根据统一 AgentRequest 选择 Agent、模型提供方和模型名称。
    /// </summary>
    /// <param name="request">前端提交的统一 Agent 请求。</param>
    /// <returns>包含 Agent 名称、模型提供方、模型名和路由原因的结果。</returns>
    /// <remarks>
    /// 路由优先级与需求保持一致：
    /// 1. Mode=check_homework 进入 HomeworkCheckAgent；
    /// 2. Mode=wrong_review/textbook_qa/voice_chat/avatar_explain 分别进入对应 Agent；
    /// 3. InputType=image 进入 VisionAgent；
    /// 4. 其他情况进入 ChatAgent。
    /// AgentService 会把本方法返回的结果保存到 AgentRouteLog。
    /// </remarks>
    public AgentRouteResult Route(AgentRequest request)
    {
        var mode = Normalize(request.Mode);
        var inputType = Normalize(request.InputType);
        var textProviderName = GetTextProviderName();
        var visionProviderName = GetVisionProviderName();
        var textModelName = GetTextModelName();
        var visionModelName = GetVisionModelName();

        return mode switch
        {
            "check_homework" => Create("HomeworkCheckAgent", visionProviderName, visionModelName, "Mode = check_homework，按规则进入作业检查 Agent。"),
            "wrong_review" => Create("WrongBookAgent", textProviderName, textModelName, "Mode = wrong_review，按规则进入错题复习 Agent。"),
            "textbook_qa" => Create("TextbookRagAgent", textProviderName, textModelName, "Mode = textbook_qa，按规则进入教材知识库 Agent。"),
            "voice_chat" => Create("VoiceAgent", textProviderName, textModelName, "Mode = voice_chat，进入语音讨论占位 Agent。"),
            "avatar_explain" => Create("AvatarAgent", textProviderName, textModelName, "Mode = avatar_explain，进入数字人讲解占位 Agent。"),
            _ when inputType == "image" => Create("VisionAgent", visionProviderName, visionModelName, "InputType = image，按规则进入图片讲题 Agent。"),
            _ => Create("ChatAgent", textProviderName, textModelName, "未命中特殊模式，默认进入普通问答 Agent。")
        };
    }

    /// <summary>
    /// 创建标准路由结果。
    /// </summary>
    /// <param name="agentName">按规则选中的 Agent 名称。</param>
    /// <param name="modelProvider">当前配置下选中的模型提供方。</param>
    /// <param name="modelName">当前配置下选中的模型名称。</param>
    /// <param name="reason">保存到 AgentRouteLog 的路由原因。</param>
    /// <returns>统一的 AgentRouteResult。</returns>
    /// <remarks>
    /// 调用链：Route -> Create -> AgentService 保存 AgentRouteLog。
    /// Phase 3 后这里不再硬编码 Mock，而是把 UseMock 决定出的 Provider 和模型名写入路由结果。
    /// </remarks>
    private static AgentRouteResult Create(string agentName, string modelProvider, string modelName, string reason)
    {
        return new AgentRouteResult
        {
            AgentName = agentName,
            ModelProvider = modelProvider,
            ModelName = modelName,
            RouteReason = reason
        };
    }

    /// <summary>
    /// 获取当前文本模型提供方名称。
    /// </summary>
    /// <remarks>
    /// 调用链：Route 在选择文本类 Agent 时调用。
    /// UseMock=true 记录 Mock；UseMock=false 记录 DeepSeek，便于日志追踪真实模型调用。
    /// </remarks>
    private string GetTextProviderName() => _options.UseMock ? "Mock" : "DeepSeek";

    /// <summary>
    /// 获取当前视觉模型提供方名称。
    /// </summary>
    /// <remarks>
    /// 调用链：Route 在选择图片讲题或作业检查 Agent 时调用。
    /// UseMock=true 记录 Mock；UseMock=false 记录 GlmVision，便于区分文本模型和视觉模型。
    /// </remarks>
    private string GetVisionProviderName() => _options.UseMock ? "Mock" : "GlmVision";

    /// <summary>
    /// 获取当前文本模型名称。
    /// </summary>
    /// <remarks>
    /// 调用链：Route -> 文本类路由结果。
    /// Mock 模式使用 mock-text-model；真实模式读取 DeepSeek:DefaultModel，缺失时回退到 deepseek-chat。
    /// </remarks>
    private string GetTextModelName()
    {
        return _options.UseMock
            ? "mock-text-model"
            : string.IsNullOrWhiteSpace(_options.DeepSeek.DefaultModel) ? "deepseek-chat" : _options.DeepSeek.DefaultModel;
    }

    /// <summary>
    /// 获取当前视觉模型名称。
    /// </summary>
    /// <remarks>
    /// 调用链：Route -> 视觉类路由结果。
    /// Mock 模式使用 mock-vision-model；真实模式读取 Zhipu:VisionModel，缺失时回退到 glm-4v-plus。
    /// </remarks>
    private string GetVisionModelName()
    {
        return _options.UseMock
            ? "mock-vision-model"
            : string.IsNullOrWhiteSpace(_options.Zhipu.VisionModel) ? "glm-4v-plus" : _options.Zhipu.VisionModel;
    }

    /// <summary>
    /// 标准化前端传入的轻量字符串。
    /// </summary>
    /// <param name="value">待标准化的 mode 或 inputType。</param>
    /// <returns>去空格并转小写后的字符串。</returns>
    /// <remarks>
    /// 调用链：Route 解析 Mode 和 InputType 时调用。
    /// 这样 Swagger 手工测试时即便大小写不一致，也不会影响规则路由。
    /// </remarks>
    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
