using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 通过 AiTutor.Api 调用多模态模型识别整页作业中的题目区域。
/// </summary>
public interface IAiQuestionRegionDetector
{
    Task<IReadOnlyList<DetectedQuestionRegion>> DetectAsync(
        string imagePath,
        float imageWidth,
        float imageHeight,
        CancellationToken cancellationToken = default);
}

