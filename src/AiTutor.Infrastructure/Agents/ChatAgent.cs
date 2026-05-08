using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 普通文字问答 Agent，负责处理默认文本提问和数学讲题。
/// </summary>
/// <remarks>
/// 调用链：AgentService -> AgentRouter 选择 ChatAgent -> ChatAgent -> PromptTemplateService -> ITextModelProvider。
/// Phase 3 后 ITextModelProvider 可由 DI 切换为 MockTextModelProvider 或 DeepSeekTextModelProvider。
/// 本类不直接保存数据库，AnswerRecord、SessionMessage 和 ModelCallLog 统一由 AgentService 保存。
/// </remarks>
public class ChatAgent : IAgent
{
    private readonly ITextModelProvider _modelProvider;
    private readonly IPromptTemplateService _promptTemplateService;

    public ChatAgent(ITextModelProvider modelProvider, IPromptTemplateService promptTemplateService)
    {
        _modelProvider = modelProvider;
        _promptTemplateService = promptTemplateService;
    }

    public string Name => "ChatAgent";

    /// <summary>
    /// 执行普通文字问答。
    /// </summary>
    /// <param name="request">学生文本提问请求。</param>
    /// <param name="route">AgentRouter 生成的路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>适合小学生阅读的 AgentResponse。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 数学学科优先使用 math_problem_explain 模板，其余使用 chat_text_explain；
    /// 2. 将年级、学科、问题和教材上下文渲染进 Prompt；
    /// 3. 调用当前注入的文本模型 Provider；
    /// 4. 用 ResponseFactory 统一转换为 AgentResponse。
    /// </remarks>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var templateCode = string.Equals(request.Subject, "数学", StringComparison.OrdinalIgnoreCase)
            ? "math_problem_explain"
            : "chat_text_explain";

        var prompt = _promptTemplateService.Render(templateCode, new Dictionary<string, string?>
        {
            ["subject"] = request.Subject ?? "学习",
            ["grade"] = request.Grade ?? "小学",
            ["question"] = request.QuestionText,
            ["textbook_context"] = string.Empty
        });

        var answer = await _modelProvider.GenerateAsync(prompt, cancellationToken);
        return ResponseFactory.Create(route, answer, canAddToWrongBook: true);
    }
}
