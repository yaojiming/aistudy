using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Models;

/// <summary>
/// 自动推算出的题目区域，Bounds 使用原图像素坐标。
/// </summary>
/// <param name="Id">区域 Id。</param>
/// <param name="Title">显示标题，例如“第 1 题”。</param>
/// <param name="Bounds">题目区域在原图像素坐标系中的边界框。</param>
/// <param name="TextPreview">该区域内 OCR 文本摘要。</param>
public sealed record QuestionRegion(string Id, string Title, RectF Bounds, string TextPreview);
