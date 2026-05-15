using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Models;

/// <summary>
/// 作业图片中的一道题，BBox 使用纠正后原图的像素坐标。
/// </summary>
public sealed record HomeworkQuestionRegion(
    string QuestionId,
    string QuestionNo,
    RectF BBox,
    string QuestionText,
    string? StudentAnswer,
    double Confidence);

