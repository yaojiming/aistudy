using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 作业题目区域识别服务，返回结构化题号、题目文本、学生答案和像素坐标。
/// </summary>
public interface IHomeworkQuestionDetectService
{
    Task<IReadOnlyList<HomeworkQuestionRegion>> DetectQuestionsAsync(
        string correctedImagePath,
        QuestionDetectionMode detectionMode = QuestionDetectionMode.Local,
        CancellationToken cancellationToken = default);
}

public enum QuestionDetectionMode
{
    Local,
    Ai,
    LocalThenAi
}
