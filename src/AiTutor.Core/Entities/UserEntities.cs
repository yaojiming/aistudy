namespace AiTutor.Core.Entities;

/// <summary>
/// 用户基础信息实体。
/// </summary>
/// <remarks>
/// 用于保存 AiTutor 中学生、家长或教材维护人员的基础身份资料。
/// 第一阶段主要服务学生学习场景，后续可扩展家长端和管理后台。
/// </remarks>
public class UserProfile : AuditableEntity
{
    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarPath { get; set; }

    public string UserType { get; set; } = "student";

    public string? PhoneNumber { get; set; }
}

/// <summary>
/// 学生学习档案实体。
/// </summary>
/// <remarks>
/// 保存学生的年级、学期、学校、默认学科和教材版本等学习画像信息。
/// Agent、教材知识库和练习计划可基于该档案选择更适合小学生的讲解方式。
/// </remarks>
public class LearningProfile : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Grade { get; set; } = string.Empty;

    public string? Semester { get; set; }

    public string? SchoolName { get; set; }

    public string? DefaultSubject { get; set; }

    public string? TextbookVersion { get; set; }
}
