using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Models;

/// <summary>
/// 自动推算出的题目区域，Bounds 使用原图像素坐标。拍照讲题只使用基础字段，作业检查会额外使用状态和对错。
/// </summary>
public sealed record QuestionRegion(
    string Id,
    string Title,
    RectF Bounds,
    string TextPreview,
    HomeworkQuestionStatus Status = HomeworkQuestionStatus.Pending,
    bool? IsCorrect = null);

