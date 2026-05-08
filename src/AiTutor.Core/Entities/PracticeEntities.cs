using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 练习题实体。
/// </summary>
/// <remarks>
/// 保存由错题、知识点、阶段任务、复习计划或人工创建的练习题。
/// 与 WrongQuestion 分表保存，避免把原始错题和生成练习混在一起。
/// </remarks>
public class PracticeQuestion : AuditableEntity
{
    public string? UserId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Grade { get; set; }

    public string? KnowledgePointId { get; set; }

    public string? WrongQuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? QuestionImagePath { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? Explanation { get; set; }

    public int DifficultyLevel { get; set; } = 1;

    public PracticeGenerateType GenerateType { get; set; } = PracticeGenerateType.Manual;

    public PracticeSourceType? SourceType { get; set; }

    public string? ModelName { get; set; }

    public string? PromptVersion { get; set; }
}

/// <summary>
/// 练习题来源实体。
/// </summary>
/// <remarks>
/// 记录练习题来自哪道错题、哪个知识点、哪段教材或哪个学习阶段。
/// 后续分析练习质量和追溯生成依据时使用该实体。
/// </remarks>
public class PracticeQuestionSource : AiTutorEntity
{
    public string PracticeQuestionId { get; set; } = string.Empty;

    public PracticeSourceType SourceType { get; set; } = PracticeSourceType.Manual;

    public string SourceId { get; set; } = string.Empty;
}

/// <summary>
/// 练习作答记录实体。
/// </summary>
/// <remarks>
/// 保存学生提交练习题后的答案、判断结果、AI 反馈和耗时。
/// 可用于更新掌握状态、复习计划和学习报告。
/// </remarks>
public class PracticeAnswerRecord : AiTutorEntity
{
    public string UserId { get; set; } = string.Empty;

    public string PracticeQuestionId { get; set; } = string.Empty;

    public string? UserAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public string? AiFeedback { get; set; }

    public int? DurationSeconds { get; set; }
}
