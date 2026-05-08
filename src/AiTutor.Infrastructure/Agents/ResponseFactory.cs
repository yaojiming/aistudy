using AiTutor.Shared.Agent;

namespace AiTutor.Infrastructure.Agents;

/// <summary>
/// AgentResponse 工厂，集中创建通用响应骨架。
/// </summary>
/// <remarks>
/// 调用链：各具体 Agent -> ResponseFactory.Create -> AgentService 保存 AnswerRecord / SessionMessage。
/// 这里统一填充 OutputType、模型名、Agent 名、路由原因和基础建议，避免每个 Agent 重复拼装相同字段。
/// </remarks>
internal static class ResponseFactory
{
    /// <summary>
    /// 创建文本类 AgentResponse。
    /// </summary>
    /// <param name="route">AgentRouter 生成的路由结果。</param>
    /// <param name="answerText">Provider 或占位 Agent 返回的回答文本。</param>
    /// <param name="canAddToWrongBook">当前回答是否允许加入错题本。</param>
    /// <returns>包含通用字段和默认建议的 AgentResponse。</returns>
    /// <remarks>
    /// 调用链：ChatAgent / VisionAgent / HomeworkCheckAgent / WrongBookAgent / TextbookRagAgent 等具体 Agent。
    /// Phase 3 中 route.ModelName 会随着 UseMock 切换为 mock 模型、DeepSeek 模型或 GLM 视觉模型。
    /// </remarks>
    public static AgentResponse Create(AgentRouteResult route, string answerText, bool canAddToWrongBook)
    {
        return new AgentResponse
        {
            OutputType = "text",
            AnswerText = answerText,
            CanAddToWrongBook = canAddToWrongBook,
            ModelUsed = route.ModelName,
            AgentName = route.AgentName,
            RouteReason = route.RouteReason,
            Suggestions =
            [
                new SuggestionDto { Text = "再讲简单一点", Action = "explain_simpler" },
                new SuggestionDto { Text = "给我一道类似题", Action = "practice_generate" }
            ]
        };
    }
}
