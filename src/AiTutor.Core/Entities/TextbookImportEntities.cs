using AiTutor.Core.Enums;

namespace AiTutor.Core.Entities;

/// <summary>
/// 教材导入任务实体。
/// </summary>
/// <remarks>
/// 记录教材上传、解析、OCR、切片和向量化等流程的总体状态。
/// 每次教材导入必须先创建任务，避免只保存文件路径而缺少处理主线。
/// </remarks>
public class TextbookImportJob : AuditableEntity
{
    public string? TextbookId { get; set; }

    public string? UserId { get; set; }

    public string JobName { get; set; } = string.Empty;

    public TextbookImportType ImportType { get; set; } = TextbookImportType.Pdf;

    public TextbookImportStatus Status { get; set; } = TextbookImportStatus.Pending;

    public string? SourceFilePath { get; set; }

    public int? TotalPages { get; set; }

    public int? ParsedPages { get; set; }

    public int? TotalChunks { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? FinishedTime { get; set; }
}

/// <summary>
/// 教材导入文件实体。
/// </summary>
/// <remarks>
/// 保存某个导入任务中上传的 PDF、图片或混合文件信息。
/// 文件实体只记录元数据和路径，物理文件由媒体或文件存储服务管理。
/// </remarks>
public class TextbookImportFile : AiTutorEntity
{
    public string ImportJobId { get; set; } = string.Empty;

    public string? TextbookId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string? MimeType { get; set; }

    public long? FileSize { get; set; }

    public int? PageCount { get; set; }

    public string? Hash { get; set; }
}

/// <summary>
/// 教材解析日志实体。
/// </summary>
/// <remarks>
/// 保存教材导入任务每个处理步骤的状态、消息和错误详情。
/// 用于排查 OCR、切片、解析等后台处理问题。
/// </remarks>
public class TextbookParseLog : AiTutorEntity
{
    public string ImportJobId { get; set; } = string.Empty;

    public string? TextbookId { get; set; }

    public int? PageNo { get; set; }

    public string StepName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Message { get; set; }

    public string? ErrorDetail { get; set; }
}
