using AiTutor.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiTutor.Api.Controllers;

/// <summary>
/// 媒体资源 API，统一接收 MAUI 上传的学习图片。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaResourceService _mediaResourceService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaResourceService mediaResourceService, ILogger<MediaController> logger)
    {
        _mediaResourceService = mediaResourceService;
        _logger = logger;
    }

    /// <summary>
    /// 上传题目图片或作业图片。
    /// </summary>
    /// <param name="file">图片文件。</param>
    /// <param name="resourceType">资源类型，例如 question_photo 或 homework_photo。</param>
    /// <param name="userId">上传用户 Id。</param>
    /// <param name="sourceType">上传来源。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>媒体上传结果。</returns>
    [HttpPost("upload-image")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(
        IFormFile file,
        [FromForm] string? resourceType,
        [FromForm] string? userId,
        [FromForm] string? sourceType,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("图片文件不能为空。");
        }

        if (!IsAllowedImage(file.ContentType, file.FileName))
        {
            return BadRequest("只支持 jpg、jpeg、png、webp 图片。");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _mediaResourceService.SaveImageAsync(
                stream,
                file.FileName,
                file.ContentType,
                resourceType,
                userId,
                sourceType,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传图片失败。");
            return Problem("图片上传失败，请稍后再试。");
        }
    }

    /// <summary>
    /// 访问已上传图片文件。
    /// </summary>
    /// <param name="id">媒体资源 Id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>MVP 阶段占位响应。</returns>
    [HttpGet("file/{id}")]
    public async Task<IActionResult> GetFile(string id, CancellationToken cancellationToken)
    {
        var media = await _mediaResourceService.FindByUrlAsync(null, cancellationToken);
        _ = media;
        return NotFound("当前 MVP 仅需要上传后的 FilePath 供 Agent 使用，文件访问接口后续完善。");
    }

    /// <summary>
    /// 校验上传内容是否是允许的图片类型。
    /// </summary>
    /// <param name="contentType">请求携带的 MIME 类型。</param>
    /// <param name="fileName">上传文件名。</param>
    /// <returns>允许上传返回 true。</returns>
    private static bool IsAllowedImage(string? contentType, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var extensionAllowed = extension is ".jpg" or ".jpeg" or ".png" or ".webp";
        var contentTypeAllowed = string.IsNullOrWhiteSpace(contentType) || contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        return extensionAllowed && contentTypeAllowed;
    }
}
