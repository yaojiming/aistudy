using AiTutor.Maui.Models;

namespace AiTutor.Maui.Services;

/// <summary>
/// 错题本写入入口。MVP 阶段先做本地占位，后续可替换为后端真实接口。
/// </summary>
public interface IWrongBookService
{
    Task AddWrongQuestionAsync(
        HomeworkQuestionCheckResult result,
        string imagePath,
        HomeworkQuestionRegion region,
        CancellationToken cancellationToken = default);
}

