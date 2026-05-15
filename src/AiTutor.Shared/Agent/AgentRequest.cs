namespace AiTutor.Shared.Agent;

/// <summary>
/// 统一 Agent 请求 DTO，供 MAUI 客户端调用后端 Agent 网关。
/// </summary>
public class AgentRequest
{
    public string UserId { get; set; } = string.Empty;

    public string? SessionId { get; set; }

    public string? Grade { get; set; }

    public string? Subject { get; set; }

    public string InputType { get; set; } = "text";

    public string Mode { get; set; } = "ask";

    /// <summary>
    /// 是否开启模型官方思考模式。true 表示请求支持 thinking 的模型启用思考，false 表示关闭。
    /// </summary>
    public bool EnableThinking { get; set; }

    /// <summary>
    /// Optional model name selected by the client. API keys and provider selection stay on the server.
    /// </summary>
    public string? ModelName { get; set; }

    public string? QuestionText { get; set; }

    /// <summary>
    /// 当前会话最近几轮对话摘要。用于让模型理解“3”“不会”“为什么”等短回复的上下文。
    /// </summary>
    public string? ConversationContext { get; set; }

    public string? ImageUrl { get; set; }

    public string? AudioUrl { get; set; }

    public string? RelatedWrongQuestionId { get; set; }

    public string? RelatedKnowledgePointId { get; set; }
}
