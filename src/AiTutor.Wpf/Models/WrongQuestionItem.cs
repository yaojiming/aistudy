namespace AiTutor.Wpf.Models;

public sealed class WrongQuestionItem
{
    public string Title { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string KnowledgePoint { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public string NextReviewTime { get; set; } = string.Empty;
}
