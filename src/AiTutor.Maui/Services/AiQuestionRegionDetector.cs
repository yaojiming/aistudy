using System.Text.Json;
using System.Text.RegularExpressions;
using AiTutor.Maui.Models;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// AI 题目区域识别器。MAUI 端只调用 AiTutor.Api，不直接访问任何模型或保存 API Key。
/// </summary>
public sealed class AiQuestionRegionDetector : IAiQuestionRegionDetector
{
    private static readonly JsonDocumentOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private readonly IApiClientService _apiClientService;
    private readonly IAppSettingsService _settingsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AiQuestionRegionDetector> _logger;

    public AiQuestionRegionDetector(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        ICurrentUserService currentUserService,
        ILogger<AiQuestionRegionDetector> logger)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DetectedQuestionRegion>> DetectAsync(
        string imagePath,
        float imageWidth,
        float imageHeight,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("作业图片不存在，无法进行 AI 题目区域识别。", imagePath);
        }

        if (imageWidth <= 0 || imageHeight <= 0)
        {
            throw new InvalidOperationException("图片尺寸无效，无法转换 AI 返回坐标。");
        }

        try
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
            var upload = await _apiClientService.UploadImageBytesAsync(
                imageBytes,
                $"homework-region-detect-{DateTime.UtcNow:yyyyMMddHHmmss}.jpg",
                "homework_region_detect",
                _currentUserService.UserId,
                cancellationToken);

            var response = await _apiClientService.AskAsync(new AgentRequest
            {
                UserId = _currentUserService.UserId,
                Grade = _settingsService.GetCurrentGrade(),
                Subject = "数学",
                InputType = "image",
                Mode = "ask",
                EnableThinking = false,
                ImageUrl = upload.FilePath,
                QuestionText = BuildPrompt()
            }, cancellationToken);

            var regions = ParseRegions(response.AnswerText, imageWidth, imageHeight);
            _logger.LogInformation("AI 题目区域识别完成。Count={Count}", regions.Count);
            return regions;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI 题目区域识别失败。");
            return [];
        }
    }

    private static string BuildPrompt()
    {
        return """
            请识别这张整页作业图片中每一道题的大致区域，只返回合法 JSON，不要 Markdown，不要代码块，不要解释。
            坐标必须使用 normalized_1000 坐标系：x1、y1、x2、y2 都是 0 到 1000 的整数，分别表示相对于整张图片宽高的归一化坐标。
            题目区域应尽量覆盖完整题干、图形、学生作答和必要空白，但不要覆盖相邻题。

            JSON 格式：
            {
              "coordinateSystem": "normalized_1000",
              "questions": [
                {
                  "index": 1,
                  "bbox": { "x1": 100, "y1": 120, "x2": 900, "y2": 260 },
                  "confidence": 0.9,
                  "note": ""
                }
              ]
            }

            要求：
            1. questions 按图片从上到下、从左到右排序。
            2. 如果不确定，也可以返回低 confidence，但不要编造很多不存在的题。
            3. coordinateSystem 必须是 normalized_1000。
            """;
    }

    private static IReadOnlyList<DetectedQuestionRegion> ParseRegions(string answerText, float imageWidth, float imageHeight)
    {
        var json = ExtractJson(answerText);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            using var document = JsonDocument.Parse(json, JsonOptions);
            var root = document.RootElement;
            var coordinateSystem = GetString(root, "coordinateSystem", "coordinate_system") ?? string.Empty;
            if (!string.Equals(coordinateSystem, "normalized_1000", StringComparison.OrdinalIgnoreCase))
            {
                return [];
            }

            if (!TryGetProperty(root, out var questions, "questions", "Questions") || questions.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var result = new List<DetectedQuestionRegion>();
            foreach (var question in questions.EnumerateArray())
            {
                if (!TryGetProperty(question, out var bbox, "bbox", "box", "region")
                    || bbox.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                var index = GetInt(question, "index", "questionNo", "no") ?? result.Count + 1;
                var x1 = GetFloat(bbox, "x1", "left") ?? 0;
                var y1 = GetFloat(bbox, "y1", "top") ?? 0;
                var x2 = GetFloat(bbox, "x2", "right") ?? 0;
                var y2 = GetFloat(bbox, "y2", "bottom") ?? 0;
                var rect = ConvertNormalizedToImageRect(x1, y1, x2, y2, imageWidth, imageHeight);
                if (rect.Width <= 4 || rect.Height <= 4)
                {
                    continue;
                }

                result.Add(new DetectedQuestionRegion(
                    $"ai-q{index}",
                    index,
                    rect,
                    GetDouble(question, "confidence") ?? 0.5,
                    GetString(question, "note") ?? string.Empty));
            }

            return result
                .OrderBy(item => item.Bounds.Top)
                .ThenBy(item => item.Bounds.Left)
                .ToList();
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static RectF ConvertNormalizedToImageRect(float x1, float y1, float x2, float y2, float imageWidth, float imageHeight)
    {
        var leftNorm = Math.Clamp(Math.Min(x1, x2), 0, 1000);
        var rightNorm = Math.Clamp(Math.Max(x1, x2), 0, 1000);
        var topNorm = Math.Clamp(Math.Min(y1, y2), 0, 1000);
        var bottomNorm = Math.Clamp(Math.Max(y1, y2), 0, 1000);

        var left = leftNorm / 1000f * imageWidth;
        var top = topNorm / 1000f * imageHeight;
        var right = rightNorm / 1000f * imageWidth;
        var bottom = bottomNorm / 1000f * imageHeight;
        return new RectF(left, top, right - left, bottom - top);
    }

    private static string? ExtractJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var cleaned = Regex.Replace(text, "```(?:json)?|```", string.Empty, RegexOptions.IgnoreCase).Trim();
        var start = cleaned.IndexOf('{');
        var end = cleaned.LastIndexOf('}');
        return start >= 0 && end > start ? cleaned[start..(end + 1)] : null;
    }

    private static bool TryGetProperty(JsonElement element, out JsonElement value, params string[] names)
    {
        foreach (var name in names)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out value))
            {
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string? GetString(JsonElement element, params string[] names)
    {
        return TryGetProperty(element, out var value, names) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static int? GetInt(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
        {
            return number;
        }

        return value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out number)
            ? number
            : null;
    }

    private static float? GetFloat(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetSingle(out var number))
        {
            return number;
        }

        return value.ValueKind == JsonValueKind.String && float.TryParse(value.GetString(), out number)
            ? number
            : null;
    }

    private static double? GetDouble(JsonElement element, params string[] names)
    {
        if (!TryGetProperty(element, out var value, names))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number))
        {
            return number;
        }

        return value.ValueKind == JsonValueKind.String && double.TryParse(value.GetString(), out number)
            ? number
            : null;
    }
}
