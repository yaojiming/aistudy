using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 学习计划实体。
/// </summary>
/// <remarks>
/// 保存面向知识点提升、错题复习或考试准备的阶段化学习计划。
/// 计划会拆分为多个 StudyStage，并最终落到具体 StageTask。
/// </remarks>
public class StudyPlan : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string? Grade { get; set; }

    public StudyPlanType PlanType { get; set; } = StudyPlanType.KnowledgePointPractice;

    public StudyPlanStatus Status { get; set; } = StudyPlanStatus.Draft;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Summary { get; set; }
}

/// <summary>
/// 学习阶段实体。
/// </summary>
/// <remarks>
/// 表示学习计划中的一个阶段，例如概念理解、基础练习、错题强化或复习检测。
/// 阶段用于组织任务并跟踪阶段级完成状态。
/// </remarks>
public class StudyStage : AuditableEntity
{
    public string StudyPlanId { get; set; } = string.Empty;

    public int StageNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Goal { get; set; }

    public StageTaskStatus Status { get; set; } = StageTaskStatus.Pending;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 阶段任务实体。
/// </summary>
/// <remarks>
/// 表示阶段中的具体学习动作，包括阅读讲解、做练习、重做错题、语音讨论或观看数字人视频。
/// 第一阶段只建立数据结构，语音和数字人能力暂不真实实现。
/// </remarks>
public class StageTask : AuditableEntity
{
    public string StudyStageId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public StageTaskType TaskType { get; set; } = StageTaskType.ReadExplanation;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? RelatedKnowledgePointId { get; set; }

    public string? RelatedWrongQuestionId { get; set; }

    public string? RelatedPracticeQuestionId { get; set; }

    public int SortIndex { get; set; }

    public StageTaskStatus Status { get; set; } = StageTaskStatus.Pending;
}

/// <summary>
/// 阶段任务完成记录实体。
/// </summary>
/// <remarks>
/// 保存学生完成阶段任务时的动作、结果、得分、耗时和备注。
/// 用于评估计划执行情况和生成学习报告。
/// </remarks>
public class StageTaskRecord : AiTutorEntity
{
    public string StageTaskId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string ActionType { get; set; } = string.Empty;

    public string? Result { get; set; }

    public decimal? Score { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Remark { get; set; }
}
