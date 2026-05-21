using System.Text.Json;
using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// 错题本本地占位实现。真实后端接口完成后，只需要替换 IWrongBookService 的实现。
/// </summary>
public sealed class LocalWrongBookService : IWrongBookService
{
    private readonly ILogger<LocalWrongBookService> _logger;

    public LocalWrongBookService(ILogger<LocalWrongBookService> logger)
    {
        _logger = logger;
    }

    public async Task AddWrongQuestionAsync(
        HomeworkQuestionCheckResult result,
        string imagePath,
        HomeworkQuestionRegion region,
        CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(FileSystem.AppDataDirectory, "wrong-book");
        Directory.CreateDirectory(folder);

        var payload = new
        {
            result.QuestionId,
            result.QuestionNo,
            result.QuestionText,
            result.StudentAnswer,
            result.CorrectAnswer,
            result.MistakeReason,
            result.KnowledgePoint,
            result.Explanation,
            ImagePath = imagePath,
            BBox = new { region.BBox.X, region.BBox.Y, region.BBox.Width, region.BBox.Height },
            CreatedTime = DateTimeOffset.Now
        };

        var path = Path.Combine(folder, $"{DateTimeOffset.Now:yyyyMMddHHmmss}-{result.QuestionId}.json");
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
        _logger.LogInformation("错题已写入本地占位错题本。Path={Path}", path);
    }
}

