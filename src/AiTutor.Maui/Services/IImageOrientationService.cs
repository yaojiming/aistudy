using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 本地图片方向纠正服务。当前只按 EXIF Orientation 做基础旋转，避免额外 OCR 判断方向造成选图卡顿。
/// </summary>
public interface IImageOrientationService
{
    Task<CorrectedImageResult> CorrectOrientationAsync(string imagePath, CancellationToken cancellationToken = default);
}

