namespace AiTutor.Shared.Agent;

/// <summary>
/// 媒体上传结果 DTO，供 MAUI 客户端把图片资源继续传给 Agent 网关。
/// </summary>
public class MediaUploadResultDto
{
    /// <summary>
    /// 后端保存媒体资源后生成的媒体 Id。
    /// </summary>
    public string MediaId { get; set; } = string.Empty;

    /// <summary>
    /// 媒体资源类型，例如 image、question_photo、homework_photo。
    /// </summary>
    public string ResourceType { get; set; } = "image";

    /// <summary>
    /// 客户端上传时的原始文件名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 后端保存后的相对文件路径，Agent 请求使用该值关联图片。
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 图片 MIME 类型。
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// 文件大小，单位字节。
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件访问地址预留字段。
    /// </summary>
    public string? Url { get; set; }
}
