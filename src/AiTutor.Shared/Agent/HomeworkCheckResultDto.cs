namespace AiTutor.Shared.Agent;

/// <summary>
/// 作业检查结构化结果。
/// </summary>
public class HomeworkCheckResultDto
{
    public string Summary { get; set; } = string.Empty;

    public int TotalCount { get; set; }

    public int CorrectCount { get; set; }

    public int WrongCount { get; set; }

    public List<HomeworkCheckItemDto> Items { get; set; } = [];
}

/// <summary>
/// 作业检查单题结果。
/// </summary>
public class HomeworkCheckItemDto
{
    public string QuestionNo { get; set; } = string.Empty;

    public string QuestionText { get; set; } = string.Empty;

    public string? StudentAnswer { get; set; }

    public string? CorrectAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public string? ErrorReason { get; set; }

    public string Explanation { get; set; } = string.Empty;

    public string? KnowledgePointId { get; set; }

    public string? KnowledgePointName { get; set; }

    public string? WrongQuestionId { get; set; }

    /// <summary>
    /// 题目区域坐标。作业检查场景中由视觉模型返回 normalized_1000 坐标。
    /// </summary>
    public HomeworkCheckBBoxDto? BBox { get; set; }
}

/// <summary>
/// 题目区域坐标，默认使用 normalized_1000 坐标系。
/// </summary>
public class HomeworkCheckBBoxDto
{
    public string CoordinateSystem { get; set; } = "normalized_1000";

    public float X1 { get; set; }

    public float Y1 { get; set; }

    public float X2 { get; set; }

    public float Y2 { get; set; }
}
