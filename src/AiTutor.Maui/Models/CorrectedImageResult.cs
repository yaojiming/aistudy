namespace AiTutor.Maui.Models;

/// <summary>
/// 图片方向纠正结果。OriginalImagePath 保留用户原始图片，CorrectedImagePath 用于后续预览、OCR 和裁剪。
/// </summary>
public sealed record CorrectedImageResult(
    string OriginalImagePath,
    string CorrectedImagePath,
    int RotationDegrees,
    bool IsOrientationDetected,
    string? Reason);

