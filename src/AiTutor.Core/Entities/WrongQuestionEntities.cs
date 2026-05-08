using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 错题记录实体。
/// </summary>
/// <remarks>
/// 保存学生做错或手动加入错题本的题目、答案、错因、讲解和掌握状态。
/// 它是错题复习、同类练习生成和学习计划生成的重要来源。
/// </remarks>
public class WrongQuestion : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string? Grade { get; set; }

    public string? QuestionRecordId { get; set; }

    public string? HomeworkCheckItemId { get; set; }

    public string? KnowledgePointId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? QuestionImagePath { get; set; }

    public string? StudentAnswer { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? ErrorReason { get; set; }

    public string? Explanation { get; set; }

    public int DifficultyLevel { get; set; } = 1;

    public MasteryStatus MasteryStatus { get; set; } = MasteryStatus.New;

    public int ReviewCount { get; set; }

    public DateTime? LastReviewTime { get; set; }

    public DateTime? NextReviewTime { get; set; }
}

/// <summary>
/// 错题复习记录实体。
/// </summary>
/// <remarks>
/// 保存学生每次复习错题的作答、正确性、AI 反馈和耗时。
/// 这些记录可用于更新掌握状态和下一次复习计划。
/// </remarks>
public class WrongQuestionReview : AiTutorEntity
{
    public string WrongQuestionId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string ReviewType { get; set; } = string.Empty;

    public string? UserAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public string? AiFeedback { get; set; }

    public int? ReviewDurationSec { get; set; }
}

/// <summary>
/// 复习计划实体。
/// </summary>
/// <remarks>
/// 保存错题、知识点或练习题的复习目标、复习等级、下次复习时间和状态。
/// 第一阶段先建立结构，为后续错题复习节奏和学习报告打基础。
/// </remarks>
public class ReviewSchedule : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public ReviewTargetType ReviewTargetType { get; set; } = ReviewTargetType.WrongQuestion;

    public string ReviewTargetId { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string? Grade { get; set; }

    public string? KnowledgePointId { get; set; }

    public int ReviewLevel { get; set; } = 1;

    public DateTime NextReviewTime { get; set; }

    public DateTime? LastReviewTime { get; set; }

    public int ReviewCount { get; set; }

    public ReviewScheduleStatus Status { get; set; } = ReviewScheduleStatus.Pending;
}
