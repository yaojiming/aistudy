using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 媒体资源实体。
/// </summary>
/// <remarks>
/// 统一管理学生上传图片、作业照片、教材 PDF、音频、视频、裁剪图和数字人脚本等资源。
/// 业务实体只关联媒体路径或 Id，避免文件信息散落在各处。
/// </remarks>
public class MediaResource : AuditableEntity
{
    public string? UserId { get; set; }

    public MediaResourceType ResourceType { get; set; } = MediaResourceType.Image;

    public string? FileName { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string? MimeType { get; set; }

    public long? FileSize { get; set; }

    public int? DurationMs { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Hash { get; set; }

    public string? SourceType { get; set; }
}

/// <summary>
/// 模型调用日志实体。
/// </summary>
/// <remarks>
/// 记录每次文本或视觉模型调用的 Prompt、响应、Token、成本、耗时和错误信息。
/// 所有真实或 Mock Provider 调用都应写入该实体，方便审计和排障。
/// </remarks>
public class ModelCallLog : AiTutorEntity
{
    public string? UserId { get; set; }

    public string? SessionId { get; set; }

    public string? QuestionRecordId { get; set; }

    public string? AgentName { get; set; }

    public ModelProviderType? ModelProvider { get; set; }

    public string? ModelName { get; set; }

    public string? RequestType { get; set; }

    public string? PromptText { get; set; }

    public string? ResponseText { get; set; }

    public int? InputTokens { get; set; }

    public int? OutputTokens { get; set; }

    public decimal? Cost { get; set; }

    public int? DurationMs { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Agent 路由日志实体。
/// </summary>
/// <remarks>
/// 记录 AgentRouter 根据输入类型和问题模式选择 Agent 与模型提供方的过程。
/// 第一阶段采用规则路由，每次路由都应保存该日志。
/// </remarks>
public class AgentRouteLog : AiTutorEntity
{
    public string? UserId { get; set; }

    public string? SessionId { get; set; }

    public string? QuestionRecordId { get; set; }

    public AgentInputType? InputType { get; set; }

    public QuestionMode? QuestionMode { get; set; }

    public string SelectedAgent { get; set; } = string.Empty;

    public ModelProviderType? SelectedModelProvider { get; set; }

    public string? SelectedModelName { get; set; }

    public string? RouteReason { get; set; }
}
