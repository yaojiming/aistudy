using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 作业检查服务。后端已改为非流式一次性返回整页 JSON，前端只负责提交图片并映射结果列表。
/// </summary>
public interface IHomeworkCheckWorkflowService
{
    Task<IReadOnlyList<HomeworkQuestionCheckResult>> CheckAsync(
        string imagePath,
        IReadOnlyList<HomeworkQuestionRegion> questions,
        string? modelName = null,
        bool enableThinking = false,
        byte[]? imageBytes = null,
        CancellationToken cancellationToken = default);
}
