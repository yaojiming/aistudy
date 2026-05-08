using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 学习会话实体。
/// </summary>
/// <remarks>
/// 表示一次完整学习交互，例如文字问答、拍照讲题、作业检查、错题复习或教材问答。
/// 会话下可以包含多条消息、问题记录、回答记录和模型调用日志。
/// </remarks>
public class LearningSession : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public SessionType SessionType { get; set; } = SessionType.TextChat;

    public string? Subject { get; set; }

    public string? Grade { get; set; }

    public string? Title { get; set; }

    public string? RelatedQuestionId { get; set; }

    public string? RelatedWrongId { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    public DateTime? EndTime { get; set; }

    public string? Summary { get; set; }
}

/// <summary>
/// 会话消息实体，预留文本、图片、音频、视频和数字人脚本。
/// </summary>
/// <remarks>
/// 保存学习会话中的每条用户或 AI 消息。
/// 第一阶段支持文本和图片主线，同时为语音讨论和数字人讲解保留字段。
/// </remarks>
public class SessionMessage : AiTutorEntity
{
    public string SessionId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public MessageRole Role { get; set; } = MessageRole.User;

    public MessageContentType ContentType { get; set; } = MessageContentType.Text;

    public string? TextContent { get; set; }

    public string? ImagePath { get; set; }

    public string? AudioPath { get; set; }

    public string? VideoPath { get; set; }

    public string? MessageJson { get; set; }
}

/// <summary>
/// 学生提问记录实体。
/// </summary>
/// <remarks>
/// 保存学生一次输入的问题，包括文本、图片、音频、识别文本、学科年级和模式。
/// AgentRouter 会基于该记录的输入类型和模式选择合适的 Agent。
/// </remarks>
public class QuestionRecord : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public string? SessionId { get; set; }

    public string? Subject { get; set; }

    public string? Grade { get; set; }

    public AgentInputType InputType { get; set; } = AgentInputType.Text;

    public string? QuestionText { get; set; }

    public string? ImagePath { get; set; }

    public string? AudioPath { get; set; }

    public string? RecognizedText { get; set; }

    public QuestionMode? Mode { get; set; }

    public string? SourceType { get; set; }
}

/// <summary>
/// AI 回答记录实体。
/// </summary>
/// <remarks>
/// 保存 Agent 生成的回答文本、结构化 JSON、模型信息、Prompt 版本和多媒体输出。
/// 后续语音和数字人讲解也会通过音频、视频和脚本字段与回答关联。
/// </remarks>
public class AnswerRecord : AiTutorEntity
{
    public string QuestionRecordId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string AnswerText { get; set; } = string.Empty;

    public string? AnswerJson { get; set; }

    public string? ModelName { get; set; }

    public string? AgentName { get; set; }

    public string? PromptVersion { get; set; }

    public AgentOutputType? OutputType { get; set; }

    public string? AudioPath { get; set; }

    public string? VideoPath { get; set; }

    public string? AvatarScriptJson { get; set; }
}

/// <summary>
/// 作业检查拆题结果实体。
/// </summary>
/// <remarks>
/// 表示作业检查中识别出的一道题及其判断结果。
/// 错题可从该实体自动进入 WrongQuestion，并尽量关联知识点。
/// </remarks>
public class HomeworkCheckItem : AuditableEntity
{
    public string QuestionRecordId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string? Grade { get; set; }

    public string? QuestionNo { get; set; }

    public string? QuestionText { get; set; }

    public string? StudentAnswer { get; set; }

    public string? CorrectAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public string? ErrorReason { get; set; }

    public string? Explanation { get; set; }

    public string? KnowledgePointId { get; set; }

    public string? ImageCropPath { get; set; }
}
