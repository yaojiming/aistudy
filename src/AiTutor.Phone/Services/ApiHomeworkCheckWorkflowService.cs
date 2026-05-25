using AiTutor.Maui.Models;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using System.Text.RegularExpressions;

namespace AiTutor.Maui.Services;

/// <summary>
/// 真实作业检查服务：整页图片上传到 AiTutor.Api，非流式一次性返回题目区域和逐题检查结果。
/// </summary>
public sealed class ApiHomeworkCheckWorkflowService : IHomeworkCheckWorkflowService
{
    private readonly IApiClientService _apiClientService;
    private readonly IAppSettingsService _settingsService;
    private readonly IImageCropService _imageCropService;
    private readonly IOcrService _ocrService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ApiHomeworkCheckWorkflowService> _logger;

    public ApiHomeworkCheckWorkflowService(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        IImageCropService imageCropService,
        IOcrService ocrService,
        ICurrentUserService currentUserService,
        ILogger<ApiHomeworkCheckWorkflowService> logger)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _imageCropService = imageCropService;
        _ocrService = ocrService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// 一次性提交整页作业图片并返回完整结果列表；题目框坐标会统一转换为当前预览图的原图像素坐标。
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

        var imageSize = await _imageCropService.GetImageSizeAsync(imagePath);
        imageBytes ??= await File.ReadAllBytesAsync(imagePath, cancellationToken);

        _logger.LogInformation(
            "Homework check started. ImagePath={ImagePath}, ImageSize={ImageWidth}x{ImageHeight}, ImageBytes={ImageBytes}",
            imagePath,
            imageSize.Width,
            imageSize.Height,
            imageBytes.Length);

        var upload = await _apiClientService.UploadImageBytesAsync(
            imageBytes,
            $"homework-page-{DateTime.UtcNow:yyyyMMddHHmmss}.jpg",
            "homework_photo",
            _currentUserService.UserId,
            cancellationToken);

        _logger.LogInformation(
            "Homework image uploaded. LocalPath={ImagePath}, UploadedPath={UploadedPath}, FileSize={FileSize}",
            imagePath,
            upload.FilePath,
            upload.FileSize);

        var response = await _apiClientService.AskAsync(new AgentRequest
        {
            UserId = _currentUserService.UserId,
            Grade = _settingsService.GetCurrentGrade(),
            Subject = _settingsService.GetCurrentSubject(),
            InputType = "image",
            Mode = "check_homework",
            EnableThinking = enableThinking,
            ModelName = modelName,
            ImageUrl = upload.FilePath,
            QuestionText = """
                请直接检查整页作业，并在每道题结果中返回 normalized_1000 的 bbox 坐标。
                bbox 坐标必须以“整张上传图片”的左上角为原点，包含照片里的墙面、桌面、手指、空白边缘等所有背景区域。
                不要使用纸张裁剪区域、题目区域或你想象中的页面区域作为坐标系。
                x1、y1、x2、y2 范围是 0 到 1000，分别对应整张上传图片的宽和高。
                即使学生未作答，也要返回正确答案、题号、题目文本、学生答案、是否正确、错因、讲解和 bbox。
                """
        }, cancellationToken);

        var ocrLines = await RecognizeQuestionAnchorLinesAsync(imagePath);
        var results = BuildQuestionResults(response, imagePath, imageSize, ocrLines, _logger);
        _logger.LogInformation(
            "Homework check API response mapped. DtoItemCount={DtoItemCount}, ResultCount={ResultCount}, QuestionNos={QuestionNos}",
            response.HomeworkCheckResult?.Items.Count ?? 0,
            results.Count,
            string.Join(",", results.Select(result => result.QuestionNo)));

        foreach (var result in results)
        {
            _logger.LogInformation(
                "Homework check mapped result. QuestionNo={QuestionNo}, QuestionId={QuestionId}, IsCorrect={IsCorrect}, FinalBBox={BBox}, StudentAnswerLength={StudentAnswerLength}, CorrectAnswerLength={CorrectAnswerLength}, ExplanationLength={ExplanationLength}",
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

    private async Task<IReadOnlyList<OcrLineInfo>> RecognizeQuestionAnchorLinesAsync(string imagePath)
    {
        try
        {
            _logger.LogInformation("Homework bbox OCR anchor recognition started. ImagePath={ImagePath}", imagePath);
            var lines = await _ocrService.RecognizeLinesAsync(imagePath);
            _logger.LogInformation("Homework bbox OCR anchor recognition finished. LineCount={LineCount}", lines.Count);

            foreach (var line in lines)
            {
                if (LooksLikeAnyQuestionNo(line.Text))
                {
                    _logger.LogInformation(
                        "Homework bbox OCR candidate line. Text={Text}, Bounds={Bounds}",
                        line.Text,
                        line.Bounds);
                }
            }

            return lines;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Homework bbox OCR anchor recognition failed. ImagePath={ImagePath}", imagePath);
            return [];
        }
    }

    private static IReadOnlyList<HomeworkQuestionCheckResult> BuildQuestionResults(
        AgentResponse response,
        string imagePath,
        SizeF imageSize,
        IReadOnlyList<OcrLineInfo> ocrLines,
        ILogger logger)
    {
        var items = response.HomeworkCheckResult?.Items ?? [];
        var mappedBoxes = new List<RectF>(items.Count);

        for (var index = 0; index < items.Count; index++)
        {
            mappedBoxes.Add(ConvertBBox(items[index].BBox, imageSize, index, items.Count));
        }

        var calibratedBoxes = CalibrateBoxesIfNeeded(imagePath, imageSize, items, mappedBoxes, ocrLines, logger);
        var results = new List<HomeworkQuestionCheckResult>(items.Count);

        for (var index = 0; index < items.Count; index++)
        {
            var dto = items[index];
            var questionNo = FirstText(dto.QuestionNo, (index + 1).ToString());
            var explanation = FirstNonJsonText(dto.Explanation, "AI 已返回结果，但未提供这道题的详细解析。");
            var shortResult = BuildShortResult(dto);

            logger.LogInformation(
                "Homework bbox final. Index={Index}, QuestionNo={QuestionNo}, RawBBox={RawBBox}, ConvertedBBox={ConvertedBBox}, FinalBBox={FinalBBox}",
                index,
                questionNo,
                FormatRawBBox(dto.BBox),
                mappedBoxes[index],
                calibratedBoxes[index]);

            results.Add(new HomeworkQuestionCheckResult
            {
                QuestionId = $"ai-q{questionNo}",
                QuestionNo = questionNo,
                BBox = calibratedBoxes[index],
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

    /// <summary>
    /// 部分视觉模型会把 bbox 返回成“纸面/内容区内部坐标”，而不是整张照片坐标。
    /// 如果检测到所有框整体落在照片上方空白区域，就根据图片中的文字墨迹区域做一次纵向校准。
    /// </summary>
    private static IReadOnlyList<RectF> CalibrateBoxesIfNeeded(
        string imagePath,
        SizeF imageSize,
        IReadOnlyList<HomeworkCheckItemDto> items,
        IReadOnlyList<RectF> boxes,
        IReadOnlyList<OcrLineInfo> ocrLines,
        ILogger logger)
    {
        if (boxes.Count == 0)
        {
            return boxes;
        }

        var ocrAnchoredBoxes = AnchorBoxesToOcrQuestionRowsIfPossible(imageSize, items, boxes, ocrLines, logger);
        if (ocrAnchoredBoxes is not null)
        {
            return ocrAnchoredBoxes;
        }

        var contentBounds = EstimateInkContentBounds(imagePath, logger);
        if (contentBounds is null)
        {
            logger.LogInformation("Homework bbox calibration skipped. Reason=NoContentBounds");
            return boxes;
        }

        var orderedBoxes = boxes.OrderBy(x => x.Top).ToList();
        var boxesTop = orderedBoxes.Min(x => x.Top);
        var boxesBottom = orderedBoxes.Max(x => x.Bottom);
        var contentTop = contentBounds.Value.Top;
        var contentBottom = contentBounds.Value.Bottom;
        var imageHeight = imageSize.Height;
        var shouldCalibrate =
            boxesTop < contentTop - imageHeight * 0.08f
            && boxesBottom < contentBottom
            && boxesBottom - boxesTop > 20;

        logger.LogInformation(
            "Homework bbox calibration check. ImageSize={ImageWidth}x{ImageHeight}, ContentBounds={ContentBounds}, BoxesTop={BoxesTop}, BoxesBottom={BoxesBottom}, ShouldCalibrate={ShouldCalibrate}",
            imageSize.Width,
            imageSize.Height,
            contentBounds.Value,
            boxesTop,
            boxesBottom,
            shouldCalibrate);

        if (!shouldCalibrate)
        {
            return boxes;
        }

        var targetTop = contentTop - MathF.Max(8, imageHeight * 0.01f);
        var targetBottom = contentBottom + MathF.Max(12, imageHeight * 0.015f);
        var sourceHeight = MathF.Max(1, boxesBottom - boxesTop);
        var scaleY = Math.Clamp((targetBottom - targetTop) / sourceHeight, 1.0f, 2.4f);
        var calibrated = new List<RectF>(boxes.Count);

        for (var index = 0; index < boxes.Count; index++)
        {
            var box = boxes[index];
            var top = targetTop + (box.Top - boxesTop) * scaleY;
            var height = box.Height * scaleY;
            var safe = ClampToImage(new RectF(box.Left, top, box.Width, height), imageSize);
            calibrated.Add(safe);

            logger.LogInformation(
                "Homework bbox calibrated. Index={Index}, QuestionNo={QuestionNo}, RawBBox={RawBBox}, Before={Before}, After={After}, ScaleY={ScaleY}",
                index,
                FirstText(items[index].QuestionNo, (index + 1).ToString()),
                FormatRawBBox(items[index].BBox),
                box,
                safe,
                scaleY);
        }

        return calibrated;
    }

    private static IReadOnlyList<RectF>? AnchorBoxesToOcrQuestionRowsIfPossible(
        SizeF imageSize,
        IReadOnlyList<HomeworkCheckItemDto> items,
        IReadOnlyList<RectF> boxes,
        IReadOnlyList<OcrLineInfo> ocrLines,
        ILogger logger)
    {
        if (items.Count == 0 || boxes.Count == 0 || ocrLines.Count == 0)
        {
            logger.LogInformation("Homework bbox OCR anchor skipped. Reason=NoItemsOrNoOcrLines, ItemCount={ItemCount}, BoxCount={BoxCount}, OcrLineCount={OcrLineCount}", items.Count, boxes.Count, ocrLines.Count);
            return null;
        }

        var sortedLines = ocrLines
            .Where(line => !string.IsNullOrWhiteSpace(line.Text) && line.Bounds.Width > 1 && line.Bounds.Height > 1)
            .OrderBy(line => line.Bounds.Top)
            .ThenBy(line => line.Bounds.Left)
            .ToList();

        var anchors = new Dictionary<int, OcrLineInfo>();
        for (var index = 0; index < items.Count; index++)
        {
            var questionNo = FirstText(items[index].QuestionNo, (index + 1).ToString()).Trim();
            if (string.IsNullOrWhiteSpace(questionNo))
            {
                continue;
            }

            var matchedLine = sortedLines.FirstOrDefault(line => IsQuestionNoLine(line.Text, questionNo));
            if (matchedLine is null)
            {
                continue;
            }

            anchors[index] = matchedLine;
            logger.LogInformation(
                "Homework bbox OCR anchor matched. Index={Index}, QuestionNo={QuestionNo}, OcrText={OcrText}, OcrBounds={OcrBounds}, Before={Before}",
                index,
                questionNo,
                matchedLine.Text,
                matchedLine.Bounds,
                boxes[index]);
        }

        if (anchors.Count == 0)
        {
            logger.LogInformation("Homework bbox OCR anchor skipped. Reason=NoQuestionNoMatched");
            return null;
        }

        var anchoredBoxes = boxes.ToList();
        var orderedAnchors = anchors.OrderBy(pair => pair.Key).ToList();
        var topPadding = MathF.Max(14, imageSize.Height * 0.012f);
        var splitGap = MathF.Max(10, imageSize.Height * 0.008f);
        var minQuestionHeight = MathF.Max(80, imageSize.Height * 0.08f);

        for (var orderIndex = 0; orderIndex < orderedAnchors.Count; orderIndex++)
        {
            var itemIndex = orderedAnchors[orderIndex].Key;
            var anchorLine = orderedAnchors[orderIndex].Value;
            var before = anchoredBoxes[itemIndex];
            OcrLineInfo? nextAnchorLine = null;
            for (var nextIndex = orderIndex + 1; nextIndex < orderedAnchors.Count; nextIndex++)
            {
                if (orderedAnchors[nextIndex].Key > itemIndex)
                {
                    nextAnchorLine = orderedAnchors[nextIndex].Value;
                    break;
                }
            }

            var top = MathF.Max(0, anchorLine.Bounds.Top - topPadding);
            float bottom;

            if (nextAnchorLine is not null)
            {
                bottom = MathF.Max(top + minQuestionHeight, nextAnchorLine.Bounds.Top - splitGap);
            }
            else
            {
                var previousHeight = itemIndex > 0 ? anchoredBoxes[itemIndex - 1].Height : before.Height;
                var inferredHeight = MathF.Max(before.Height, previousHeight);
                inferredHeight = MathF.Max(inferredHeight, imageSize.Height * 0.23f);
                bottom = top + inferredHeight;
            }

            var anchored = ClampToImage(new RectF(before.Left, top, before.Width, bottom - top), imageSize);
            anchoredBoxes[itemIndex] = anchored;

            logger.LogInformation(
                "Homework bbox OCR anchored. Index={Index}, QuestionNo={QuestionNo}, AnchorText={AnchorText}, AnchorBounds={AnchorBounds}, Before={Before}, After={After}",
                itemIndex,
                FirstText(items[itemIndex].QuestionNo, (itemIndex + 1).ToString()),
                anchorLine.Text,
                anchorLine.Bounds,
                before,
                anchored);
        }

        return anchoredBoxes;
    }

    private static bool LooksLikeAnyQuestionNo(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalized = NormalizeQuestionNoText(text);
        return Regex.IsMatch(normalized, @"^\s*(?:第\s*)?[\d一二三四五六七八九十]{1,3}\s*(?:题|[\.。．、\)\）])");
    }

    private static bool IsQuestionNoLine(string? text, string questionNo)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(questionNo))
        {
            return false;
        }

        var normalized = NormalizeQuestionNoText(text);
        var escapedNo = Regex.Escape(questionNo.Trim());
        return Regex.IsMatch(
            normalized,
            $@"^\s*(?:第\s*)?{escapedNo}\s*(?:题|[\.。．、\)\）])",
            RegexOptions.CultureInvariant);
    }

    private static string NormalizeQuestionNoText(string text)
    {
        return text
            .Replace('：', ':')
            .Replace('．', '.')
            .Replace('。', '.')
            .Replace('）', ')')
            .Trim();
    }

    private static RectF? EstimateInkContentBounds(string imagePath, ILogger logger)
    {
        try
        {
            using var stream = File.OpenRead(imagePath);
            using var bitmap = SKBitmap.Decode(stream);
            if (bitmap is null || bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return null;
            }

            const int step = 4;
            var widthSamples = Math.Max(1, bitmap.Width / step);
            var rowThreshold = Math.Max(6, (int)(widthSamples * 0.018f));
            var colCounts = new int[Math.Max(1, bitmap.Width / step)];
            var minY = bitmap.Height;
            var maxY = 0;

            for (var y = 0; y < bitmap.Height; y += step)
            {
                var rowDark = 0;
                for (var x = 0; x < bitmap.Width; x += step)
                {
                    var color = bitmap.GetPixel(x, y);
                    var brightness = (color.Red * 0.299f + color.Green * 0.587f + color.Blue * 0.114f);
                    var max = Math.Max(color.Red, Math.Max(color.Green, color.Blue));
                    var min = Math.Min(color.Red, Math.Min(color.Green, color.Blue));
                    var saturation = max <= 0 ? 0 : (max - min) / (float)max;

                    if (brightness < 105 || (brightness < 145 && saturation > 0.18f))
                    {
                        rowDark++;
                        colCounts[Math.Min(colCounts.Length - 1, x / step)]++;
                    }
                }

                if (rowDark >= rowThreshold)
                {
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }
            }

            if (minY >= maxY)
            {
                return null;
            }

            var colThreshold = Math.Max(5, (int)((maxY - minY) / (float)step * 0.018f));
            var minX = bitmap.Width;
            var maxX = 0;
            for (var i = 0; i < colCounts.Length; i++)
            {
                if (colCounts[i] >= colThreshold)
                {
                    var x = i * step;
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                }
            }

            if (minX >= maxX)
            {
                minX = 0;
                maxX = bitmap.Width;
            }

            var paddingX = bitmap.Width * 0.03f;
            var paddingY = bitmap.Height * 0.025f;
            var bounds = ClampToImage(
                new RectF(minX - paddingX, minY - paddingY, (maxX - minX) + paddingX * 2, (maxY - minY) + paddingY * 2),
                new SizeF(bitmap.Width, bitmap.Height));

            logger.LogInformation(
                "Homework image content bounds estimated. ImagePath={ImagePath}, ImageSize={Width}x{Height}, InkBounds={Bounds}, RowThreshold={RowThreshold}, ColThreshold={ColThreshold}",
                imagePath,
                bitmap.Width,
                bitmap.Height,
                bounds,
                rowThreshold,
                colThreshold);

            return bounds;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Estimate homework image content bounds failed. ImagePath={ImagePath}", imagePath);
            return null;
        }
    }

    private static RectF ClampToImage(RectF rect, SizeF imageSize)
    {
        var left = Math.Clamp(rect.Left, 0, Math.Max(0, imageSize.Width - 1));
        var top = Math.Clamp(rect.Top, 0, Math.Max(0, imageSize.Height - 1));
        var right = Math.Clamp(rect.Right, left + 1, imageSize.Width);
        var bottom = Math.Clamp(rect.Bottom, top + 1, imageSize.Height);
        return new RectF(left, top, right - left, bottom - top);
    }

    private static string FormatRawBBox(HomeworkCheckBBoxDto? bbox)
    {
        return bbox is null
            ? "<null>"
            : $"{bbox.CoordinateSystem}:({bbox.X1},{bbox.Y1})-({bbox.X2},{bbox.Y2})";
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
