namespace AiTutor.Shared.Agent;

/// <summary>
/// Agent 流式输出片段 DTO。
/// </summary>
public class AgentStreamChunkDto
{
    /// <summary>
    /// 流片段类型：delta 表示增量文本，final 表示完整响应，error 表示错误。
    /// </summary>
    public string Type { get; set; } = "delta";

    /// <summary>
    /// 当前增量文本片段。
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// 流式输出结束时返回的完整 AgentResponse，用于保存结构化结果。
    /// </summary>
    public AgentResponse? FinalResponse { get; set; }

    /// <summary>
    /// 流式输出失败时返回的错误信息。
    /// </summary>
    public string? ErrorMessage { get; set; }
}
