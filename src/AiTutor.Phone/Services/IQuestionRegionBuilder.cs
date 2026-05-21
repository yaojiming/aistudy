using AiTutor.Maui.Models;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// 根据 OCR 行坐标推算题目区域。
/// </summary>
public interface IQuestionRegionBuilder
{
    /// <summary>
    /// 从 OCR 行构造题目框。
    /// </summary>
    /// <param name="lines">OCR 文字行，Bounds 为原图像素坐标。</param>
    /// <param name="imageSize">原图像素尺寸。</param>
    /// <returns>题目区域集合。</returns>
    IReadOnlyList<QuestionRegion> Build(IReadOnlyList<OcrLineInfo> lines, SizeF imageSize);
}
