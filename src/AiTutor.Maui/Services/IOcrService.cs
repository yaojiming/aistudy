using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 本地 OCR 服务。Android 实现使用 Google ML Kit Text Recognition v2，不调用在线 OCR 服务。
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// 识别图片中的文字行和文字坐标。
    /// </summary>
    /// <param name="imagePath">本地图片路径。</param>
    /// <returns>原图像素坐标系下的 OCR 行。</returns>
    Task<IReadOnlyList<OcrLineInfo>> RecognizeLinesAsync(string imagePath);
}
