using System.Text.RegularExpressions;
using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// 根据 OCR 行推算题目区域。OCR 只负责文字和坐标，区域合并在端侧完成。
/// </summary>
public sealed partial class QuestionRegionBuilder : IQuestionRegionBuilder
{
    private const float LeftPadding = 30;
    private const float TopPadding = 20;
    private const float RightPadding = 30;
    private const float BottomPadding = 80;

    private readonly ILogger<QuestionRegionBuilder> _logger;

    public QuestionRegionBuilder(ILogger<QuestionRegionBuilder> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 按题号或纵向间距从 OCR 行生成题目框，返回的 Bounds 始终是原图像素坐标。
    /// </summary>
    public IReadOnlyList<QuestionRegion> Build(IReadOnlyList<OcrLineInfo> lines, SizeF imageSize)
    {
        var orderedLines = lines
            .Where(x => !string.IsNullOrWhiteSpace(x.Text) && x.Bounds.Width > 0 && x.Bounds.Height > 0)
            .OrderBy(x => x.Bounds.Y)
            .ThenBy(x => x.Bounds.X)
            .ToList();

        if (orderedLines.Count == 0)
        {
            return [];
        }

        var grouped = BuildByQuestionNumbers(orderedLines);
        if (grouped.Count == 0)
        {
            grouped = BuildByVerticalGaps(orderedLines);
        }

        var regions = grouped
            .Select((group, index) => CreateRegion(group, index + 1, imageSize))
            .Where(x => x.Bounds.Width > 8 && x.Bounds.Height > 8)
            .ToList();

        _logger.LogInformation("生成题目框数量：{Count}", regions.Count);
        foreach (var region in regions)
        {
            _logger.LogInformation("题目框 {Title}: {Bounds}", region.Title, region.Bounds);
        }

        return regions;
    }

    /// <summary>
    /// 遇到新的题号行就开启一个题目分组，直到下一个题号出现。
    /// </summary>
    private static List<List<OcrLineInfo>> BuildByQuestionNumbers(IReadOnlyList<OcrLineInfo> lines)
    {
        var groups = new List<List<OcrLineInfo>>();
        List<OcrLineInfo>? current = null;

        foreach (var line in lines)
        {
            if (QuestionStartRegex().IsMatch(line.Text.Trim()))
            {
                current = [];
                groups.Add(current);
            }

            current?.Add(line);
        }

        return groups.Where(x => x.Count > 0).ToList();
    }

    /// <summary>
    /// 未识别出题号时，按行间距兜底切分题目区域。
    /// </summary>
    private static List<List<OcrLineInfo>> BuildByVerticalGaps(IReadOnlyList<OcrLineInfo> lines)
    {
        var groups = new List<List<OcrLineInfo>>();
        var current = new List<OcrLineInfo>();
        var medianHeight = lines.Select(x => x.Bounds.Height).OrderBy(x => x).ElementAt(lines.Count / 2);
        var gapThreshold = MathF.Max(48, medianHeight * 2.6f);
        float? previousBottom = null;

        foreach (var line in lines)
        {
            if (previousBottom is not null && line.Bounds.Top - previousBottom.Value > gapThreshold && current.Count > 0)
            {
                groups.Add(current);
                current = [];
            }

            current.Add(line);
            previousBottom = line.Bounds.Bottom;
        }

        if (current.Count > 0)
        {
            groups.Add(current);
        }

        return groups.Count > 1 ? groups : [];
    }

    /// <summary>
    /// 合并题目内所有文字行坐标，并向外扩展，尽量覆盖图形、表格和手写答案。
    /// </summary>
    private static QuestionRegion CreateRegion(IReadOnlyList<OcrLineInfo> lines, int index, SizeF imageSize)
    {
        var left = lines.Min(x => x.Bounds.Left);
        var top = lines.Min(x => x.Bounds.Top);
        var right = lines.Max(x => x.Bounds.Right);
        var bottom = lines.Max(x => x.Bounds.Bottom);

        var expandedLeft = MathF.Max(0, left - LeftPadding);
        var expandedTop = MathF.Max(0, top - TopPadding);
        var expandedRight = MathF.Min(imageSize.Width, right + RightPadding);
        var expandedBottom = MathF.Min(imageSize.Height, bottom + BottomPadding);
        var expanded = new RectF(
            expandedLeft,
            expandedTop,
            expandedRight - expandedLeft,
            expandedBottom - expandedTop);

        var preview = string.Join(" ", lines.Select(x => x.Text.Trim()));
        if (preview.Length > 80)
        {
            preview = preview[..80] + "...";
        }

        return new QuestionRegion($"q{index}", $"第 {index} 题", expanded, preview);
    }

    [GeneratedRegex(@"^\s*((\u7B2C\s*\d+\s*\u9898)|(\d+\s*(?:[.]|\u3001|[)]|\uFF0E))|([\u4E00\u4E8C\u4E09\u56DB\u4E94\u516D\u4E03\u516B\u4E5D\u5341]+\u3001)|([\(\uFF08]\s*\d+\s*[\)\uFF09]))")]
    private static partial Regex QuestionStartRegex();
}
