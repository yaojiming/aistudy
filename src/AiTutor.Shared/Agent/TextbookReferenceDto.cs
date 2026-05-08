namespace AiTutor.Shared.Agent;

/// <summary>
/// 教材知识库引用 DTO。
/// </summary>
public class TextbookReferenceDto
{
    public string? TextbookId { get; set; }

    public string? KnowledgePointId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int? PageNo { get; set; }

    public double Score { get; set; }
}
