using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// 不支持本地 OCR 的平台降级实现，返回空结果但不影响图片上传和 AI 调用。
/// </summary>
public sealed class NoopOcrService : IOcrService
{
    private readonly ILogger<NoopOcrService> _logger;

    public NoopOcrService(ILogger<NoopOcrService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<OcrLineInfo>> RecognizeLinesAsync(string imagePath)
    {
        _logger.LogInformation("当前平台未启用本地 OCR，跳过题目区域自动识别。ImagePath={ImagePath}", imagePath);
        return Task.FromResult<IReadOnlyList<OcrLineInfo>>([]);
    }
}
