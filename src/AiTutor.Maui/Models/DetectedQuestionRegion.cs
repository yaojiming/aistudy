using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Models;

/// <summary>
/// AI 识别出的题目区域。Bounds 始终保存原图像素坐标，不保存页面显示坐标。
/// </summary>
public sealed record DetectedQuestionRegion(
    string Id,
    int Index,
    RectF Bounds,
    double Confidence,
    string Note);

