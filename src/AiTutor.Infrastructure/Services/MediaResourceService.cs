using AiTutor.Core.Entities;
using AiTutor.Core.Enums;
using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Data;
using AiTutor.Shared.Agent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AiTutor.Infrastructure.Services;

/// <summary>
/// 媒体资源查询服务。
/// </summary>
public class MediaResourceService : IMediaResourceService
{
    private readonly AiTutorDbContext _dbContext;
    private readonly IHostEnvironment _hostEnvironment;

    public MediaResourceService(AiTutorDbContext dbContext, IHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _hostEnvironment = hostEnvironment;
    }

    /// <summary>
    /// 按文件路径查找媒体资源。
    /// </summary>
    /// <param name="url">AgentRequest.ImageUrl 或媒体相对路径。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的媒体资源；不存在时返回 null。</returns>
    public Task<MediaResource?> FindByUrlAsync(string? url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Task.FromResult<MediaResource?>(null);
        }

        return _dbContext.MediaResources.AsNoTracking().FirstOrDefaultAsync(x => x.FilePath == url, cancellationToken);
    }

    /// <summary>
    /// 保存 MAUI 上传的图片文件，并写入 MediaResource 表。
    /// </summary>
    /// <param name="content">图片文件流。</param>
    /// <param name="fileName">原始文件名。</param>
    /// <param name="mimeType">图片 MIME 类型。</param>
    /// <param name="resourceType">业务资源类型，例如 question_photo 或 homework_photo。</param>
    /// <param name="userId">上传用户 Id。</param>
    /// <param name="sourceType">上传来源，例如 maui_android_tablet。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>媒体上传结果 DTO。</returns>
    public async Task<MediaUploadResultDto> SaveImageAsync(
        Stream content,
        string fileName,
        string? mimeType,
        string? resourceType,
        string? userId,
        string? sourceType,
        CancellationToken cancellationToken = default)
    {
        if (content.Length > 10 * 1024 * 1024)
        {
            throw new InvalidOperationException("图片不能超过 10MB。");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var now = DateTime.UtcNow;
        var resourceId = Guid.NewGuid().ToString("N");
        var safeFileName = $"{resourceId}{extension.ToLowerInvariant()}";
        var relativeDirectory = Path.Combine("uploads", "images", now.ToString("yyyy"), now.ToString("MM"));
        var storageRoot = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot");
        var physicalDirectory = Path.Combine(storageRoot, relativeDirectory);
        Directory.CreateDirectory(physicalDirectory);

        // 文件名由服务端生成，避免客户端控制最终物理路径。
        var physicalPath = Path.Combine(physicalDirectory, safeFileName);
        await using (var output = File.Create(physicalPath))
        {
            await content.CopyToAsync(output, cancellationToken);
        }

        var fileInfo = new FileInfo(physicalPath);
        var hash = await ComputeSha256Async(physicalPath, cancellationToken);
        var filePath = "/" + Path.Combine(relativeDirectory, safeFileName).Replace('\\', '/');
        var mediaResourceType = ParseResourceType(resourceType);

        var media = new MediaResource
        {
            Id = resourceId,
            UserId = userId,
            ResourceType = mediaResourceType,
            FileName = fileName,
            FilePath = filePath,
            MimeType = mimeType,
            FileSize = fileInfo.Length,
            Hash = hash,
            SourceType = sourceType
        };

        _dbContext.MediaResources.Add(media);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MediaUploadResultDto
        {
            MediaId = media.Id,
            ResourceType = ToClientResourceType(mediaResourceType),
            FileName = media.FileName ?? safeFileName,
            FilePath = media.FilePath,
            MimeType = media.MimeType,
            FileSize = media.FileSize ?? 0,
            Url = $"/api/media/file/{media.Id}"
        };
    }

    /// <summary>
    /// 计算文件 SHA256，用于后续去重、审计或资源校验。
    /// </summary>
    /// <param name="path">物理文件路径。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>小写十六进制 Hash。</returns>
    private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hashBytes = await System.Security.Cryptography.SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// 将客户端传入的轻量资源类型转换为领域枚举。
    /// </summary>
    /// <param name="resourceType">客户端资源类型字符串。</param>
    /// <returns>MediaResourceType 枚举。</returns>
    private static MediaResourceType ParseResourceType(string? resourceType)
    {
        return (resourceType ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "homework_photo" => MediaResourceType.HomeworkPhoto,
            "question_photo" => MediaResourceType.QuestionPhoto,
            _ => MediaResourceType.Image
        };
    }

    /// <summary>
    /// 将领域枚举转换为客户端可读的资源类型字符串。
    /// </summary>
    /// <param name="resourceType">领域资源枚举。</param>
    /// <returns>客户端资源类型字符串。</returns>
    private static string ToClientResourceType(MediaResourceType resourceType)
    {
        return resourceType switch
        {
            MediaResourceType.HomeworkPhoto => "homework_photo",
            MediaResourceType.QuestionPhoto => "question_photo",
            _ => "image"
        };
    }
}
