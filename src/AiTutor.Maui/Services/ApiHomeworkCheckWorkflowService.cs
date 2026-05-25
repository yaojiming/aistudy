using AiTutor.Maui.Models;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// 真实作业检查服务：整页图片上传到 AiTutor.Api，非流式一次性返回题目区域和逐题检查结果。
/// </summary>
public sealed class ApiHomeworkCheckWorkflowService : IHomeworkCheckWorkflowService
{
    private readonly IApiClientService _apiClientService;
    private readonly IAppSettingsService _settingsService;
    private readonly IImageCropService _imageCropService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApiHomeworkCheckWorkflowService> _logger;

    public ApiHomeworkCheckWorkflowService(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        IImageCropService imageCropService,
        ICurrentUserService currentUserService,
        ILogger<ApiHomeworkCheckWorkflowService> logger)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _imageCropService = imageCropService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// 一次性提交整页作业图片并返回完整结果列表。这里不做流式、不做 Channel、不逐题 yield。
    /// </summary>
    public async Task<IReadOnlyList<HomeworkQuestionCheckResult>> CheckAsync(
        string imagePath,
        IReadOnlyList<HomeworkQuestionRegion> questions,
        string? modelName = null,
        bool enableThinking = false,
        byte[]? imageBytes = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("作业图片不存在，请重新选择图片。", imagePath);
        }

        _logger.LogInformation("Homework check started. ImagePath={ImagePath}", imagePath);

        var imageSize = await _imageCropService.GetImageSizeAsync(imagePath);
        imageBytes ??= await File.ReadAllBytesAsync(imagePath, cancellationToken);
        var upload = await _apiClientService.UploadImageBytesAsync(
            imageBytes,
            $"homework-page-{DateTime.UtcNow:yyyyMMddHHmmss}.jpg",
            "homework_photo",
            _currentUserService.UserId,
            cancellationToken);

        var response = await _apiClientService.AskAsync(new AgentRequest
        {
            UserId = _currentUserService.UserId,
            Grade = _settingsService.GetCurrentGrade(),
            Subject = "数学",
            InputType = "image",
            Mode = "check_homework",
            EnableThinking = enableThinking,
            ModelName = modelName,
            ImageUrl = upload.FilePath,
            QuestionText = "请直接检查整页作业，并在每道题结果中返回 normalized_1000 的 bbox 坐标。即使学生未作答，也要返回正确答案"
        }, cancellationToken);

        var results = BuildQuestionResults(response, imageSize);
        _logger.LogInformation(
            "Homework check API response mapped. DtoItemCount={DtoItemCount}, ResultCount={ResultCount}, QuestionNos={QuestionNos}",
            response.HomeworkCheckResult?.Items.Count ?? 0,
            results.Count,
            string.Join(",", results.Select(result => result.QuestionNo)));

        foreach (var result in results)
        {
            _logger.LogInformation(
                "Homework check mapped result. QuestionNo={QuestionNo}, QuestionId={QuestionId}, IsCorrect={IsCorrect}, BBox={BBox}, StudentAnswerLength={StudentAnswerLength}, CorrectAnswerLength={CorrectAnswerLength}, ExplanationLength={ExplanationLength}",
                result.QuestionNo,
                result.QuestionId,
                result.IsCorrect,
                result.BBox,
                result.StudentAnswer?.Length ?? 0,
                result.CorrectAnswer?.Length ?? 0,
                result.Explanation.Length);
        }

        return results;
    }

    private static IReadOnlyList<HomeworkQuestionCheckResult> BuildQuestionResults(AgentResponse response, SizeF imageSize)
    {
        var items = response.HomeworkCheckResult?.Items ?? [];
        var results = new List<HomeworkQuestionCheckResult>(items.Count);

        for (var index = 0; index < items.Count; index++)
        {
            var dto = items[index];
            var questionNo = FirstText(dto.QuestionNo, (index + 1).ToString());
            var bbox = ConvertBBox(dto.BBox, imageSize, index, items.Count);
            var explanation = FirstNonJsonText(dto.Explanation, "AI 已返回结果，但未提供这道题的详细解析。");
            var shortResult = BuildShortResult(dto);

            results.Add(new HomeworkQuestionCheckResult
            {
                QuestionId = $"ai-q{questionNo}",
                QuestionNo = questionNo,
                BBox = bbox,
                QuestionText = FirstText(dto.QuestionText, $"第 {questionNo} 题"),
                StudentAnswer = FirstNullableText(dto.StudentAnswer),
                CorrectAnswer = FirstNullableText(dto.CorrectAnswer),
                IsCorrect = dto.IsCorrect,
                ShortResult = shortResult,
                Explanation = explanation,
                MistakeReason = dto.ErrorReason,
                KnowledgePoint = dto.KnowledgePointName,
                StreamingCheckText = BuildDetailText(questionNo, dto, explanation)
            });
        }

        return results;
    }

    private static RectF ConvertBBox(HomeworkCheckBBoxDto? bbox, SizeF imageSize, int index, int totalCount)
    {
        if (bbox?.CoordinateSystem?.Equals("normalized_1000", StringComparison.OrdinalIgnoreCase) == true)
        {
            var leftNorm = Math.Clamp(Math.Min(bbox.X1, bbox.X2), 0, 1000);
            var rightNorm = Math.Clamp(Math.Max(bbox.X1, bbox.X2), 0, 1000);
            var topNorm = Math.Clamp(Math.Min(bbox.Y1, bbox.Y2), 0, 1000);
            var bottomNorm = Math.Clamp(Math.Max(bbox.Y1, bbox.Y2), 0, 1000);

            var left = leftNorm / 1000f * imageSize.Width;
            var top = topNorm / 1000f * imageSize.Height;
            var right = rightNorm / 1000f * imageSize.Width;
            var bottom = bottomNorm / 1000f * imageSize.Height;
            if (right - left > 4 && bottom - top > 4)
            {
                return new RectF(left, top, right - left, bottom - top);
            }
        }

        var safeTotal = Math.Max(1, totalCount);
        var height = imageSize.Height / safeTotal * 0.82f;
        var topFallback = imageSize.Height / safeTotal * index + imageSize.Height * 0.02f;
        return new RectF(imageSize.Width * 0.06f, topFallback, imageSize.Width * 0.88f, height);
    }

    private static string BuildShortResult(HomeworkCheckItemDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.ErrorReason))
        {
            return OneLine(dto.ErrorReason, 48);
        }

        if (!string.IsNullOrWhiteSpace(dto.Explanation))
        {
            return OneLine(dto.Explanation, 48);
        }

        return dto.IsCorrect switch
        {
            true => "答案正确。",
            false => "答案有误，需要订正。",
            _ => "暂不确定，请查看详情。"
        };
    }

    private static string BuildDetailText(string questionNo, HomeworkCheckItemDto dto, string explanation)
    {
        var status = dto.IsCorrect switch
        {
            true => "正确",
            false => "错误",
            _ => "不确定"
        };

        var builder = new System.Text.StringBuilder();
        builder.AppendLine($"## 第 {questionNo} 题");
        builder.AppendLine();
        builder.AppendLine($"**检查结果：** {status}");
        builder.AppendLine();
        builder.AppendLine($"**题目：** {FirstText(dto.QuestionText, "未识别到题干")}");
        builder.AppendLine();
        builder.AppendLine($"**学生作答：** {FirstText(dto.StudentAnswer, "未识别")}");
        builder.AppendLine();
        builder.AppendLine($"**正确答案：** {FirstText(dto.CorrectAnswer, "待确认")}");

        if (!string.IsNullOrWhiteSpace(dto.ErrorReason))
        {
            builder.AppendLine();
            builder.AppendLine($"**错因分析：** {dto.ErrorReason}");
        }

        if (!string.IsNullOrWhiteSpace(dto.KnowledgePointName))
        {
            builder.AppendLine();
            builder.AppendLine($"**知识点：** {dto.KnowledgePointName}");
        }

        builder.AppendLine();
        builder.AppendLine("### 讲解");
        builder.AppendLine(explanation);
        return builder.ToString();
    }

    private static string FirstText(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

    private static string FirstNonJsonText(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value) && !LooksLikeJsonPayload(value)) ?? string.Empty;
    }

    private static bool LooksLikeJsonPayload(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.TrimStart();
        return text.StartsWith("{", StringComparison.Ordinal)
            || text.StartsWith("[", StringComparison.Ordinal)
            || text.Contains("\"items\"", StringComparison.OrdinalIgnoreCase)
            || text.Contains("\"coordinateSystem\"", StringComparison.OrdinalIgnoreCase);
    }

    private static string? FirstNullableText(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static string OneLine(string value, int maxLength)
    {
        var text = value
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Trim();

        return text.Length <= maxLength ? text : text[..maxLength] + "...";
    }
}
