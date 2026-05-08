namespace AiTutor.Shared.Agent;

/// <summary>
/// 统一 Agent 响应 DTO，承载文本、结构化结果、语音和数字人预留输出。
/// </summary>
public class AgentResponse
{
    public string SessionId { get; set; } = string.Empty;

    public string? QuestionRecordId { get; set; }

    public string? AnswerRecordId { get; set; }

    public string OutputType { get; set; } = "text";

    public string AnswerText { get; set; } = string.Empty;

    public object? AnswerJson { get; set; }

    public HomeworkCheckResultDto? HomeworkCheckResult { get; set; }

    public List<TextbookReferenceDto> TextbookReferences { get; set; } = [];

    public string? AudioUrl { get; set; }

    public string? VideoUrl { get; set; }

    public AvatarScriptDto? AvatarScript { get; set; }

    public List<SuggestionDto> Suggestions { get; set; } = [];

    public bool CanAddToWrongBook { get; set; }

    public string ModelUsed { get; set; } = "mock-text-model";

    public string AgentName { get; set; } = string.Empty;

    public string RouteReason { get; set; } = string.Empty;

    public int DurationMs { get; set; }
}
