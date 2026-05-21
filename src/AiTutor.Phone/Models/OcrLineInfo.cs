using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Models;

/// <summary>
/// Android 端侧 OCR 返回的一行文字及其原图像素坐标。
/// </summary>
/// <param name="Text">识别出的文字。</param>
/// <param name="Bounds">文字行在原图像素坐标系中的边界框。</param>
/// <param name="Confidence">识别置信度，ML Kit 行级结果没有提供时为空。</param>
/// <param name="LineIndex">文字行序号。</param>
public sealed record OcrLineInfo(string Text, RectF Bounds, float? Confidence = null, int LineIndex = 0);
