namespace AiTutor.Shared.Agent;

/// <summary>
/// 错题 DTO。
/// </summary>
public class WrongQuestionDto
{
    public string Id { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string? Grade { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? StudentAnswer { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? ErrorReason { get; set; }

    public string? Explanation { get; set; }

    public string MasteryStatus { get; set; } = "new";
}
