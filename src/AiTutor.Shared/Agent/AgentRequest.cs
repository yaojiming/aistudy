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

    public string? ThinkingMode { get; set; } = "standard";

    public string? QuestionText { get; set; }

    public string? ImageUrl { get; set; }

    public string? AudioUrl { get; set; }

    public string? RelatedWrongQuestionId { get; set; }

    public string? RelatedKnowledgePointId { get; set; }
}
