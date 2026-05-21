namespace AiTutor.Maui.Models;

/// <summary>
/// 单道作业题的检查结果，供前端详情展示和加入错题本使用。
/// </summary>
public sealed class HomeworkQuestionCheckResult
{
    public string QuestionId { get; set; } = string.Empty;

    public string QuestionNo { get; set; } = string.Empty;

    public string QuestionText { get; set; } = string.Empty;

    public Microsoft.Maui.Graphics.RectF BBox { get; set; }

    public string? StudentAnswer { get; set; }

    public string? CorrectAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public string ShortResult { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string? MistakeReason { get; set; }

    public string? KnowledgePoint { get; set; }

    public string StreamingCheckText { get; set; } = string.Empty;

    public bool CanAddToWrongBook => IsCorrect == false;
}
