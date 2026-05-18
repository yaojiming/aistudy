using AiTutor.Core.Interfaces;
using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// 普通文字问答 Agent，负责处理默认文本提问和数学讲题。
/// </summary>
public class ChatAgent : IStreamingAgent
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
    /// 执行非流式文字问答，供普通 /api/agent/ask 接口使用。
    /// </summary>
    /// <param name="request">学生文字提问请求。</param>
    /// <param name="route">AgentRouter 生成的路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完整 AgentResponse。</returns>
    public async Task<AgentResponse> ExecuteAsync(AgentRequest request, AgentRouteResult route, CancellationToken cancellationToken = default)
    {
        var answer = await _modelProvider.GenerateAsync(BuildPrompt(request), request.EnableThinking, cancellationToken);
        return ResponseFactory.Create(route, answer, canAddToWrongBook: true);
    }

    /// <summary>
    /// 执行真实流式文字问答，直接消费 ITextModelProvider 的增量输出。
    /// </summary>
    /// <param name="request">学生文字提问请求，包含 EnableThinking。</param>
    /// <param name="route">AgentRouter 生成的路由结果。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>delta 文本片段和最终 AgentResponse。</returns>
    public async IAsyncEnumerable<AgentStreamChunkDto> StreamExecuteAsync(
        AgentRequest request,
        AgentRouteResult route,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var answerText = string.Empty;
        await foreach (var delta in _modelProvider.GenerateStreamAsync(BuildPrompt(request), request.EnableThinking, cancellationToken))
        {
            answerText += delta;
            yield return new AgentStreamChunkDto { Type = "delta", Text = delta };
        }

        yield return new AgentStreamChunkDto
        {
            Type = "final",
            FinalResponse = ResponseFactory.Create(route, answerText, canAddToWrongBook: true)
        };
    }

    /// <summary>
    /// 根据年级、学科和问题内容渲染集中管理的 Prompt 模板。
    /// </summary>
    /// <param name="request">Agent 请求。</param>
    /// <returns>渲染后的完整 Prompt。</returns>
    private string BuildPrompt(AgentRequest request)
    {
        var templateCode = string.Equals(request.Subject, "数学", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(request.Subject, "鏁板", StringComparison.OrdinalIgnoreCase)
            ? "math_problem_explain"
            : "chat_text_explain";

        return _promptTemplateService.Render(templateCode, new Dictionary<string, string?>
        {
            ["subject"] = request.Subject ?? "学习",
            ["grade"] = request.Grade ?? "小学",
            ["question"] = request.QuestionText,
            ["conversation_context"] = request.ConversationContext,
            ["textbook_context"] = string.Empty
        });
    }
}
