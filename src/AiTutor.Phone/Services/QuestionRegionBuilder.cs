using System.Text.RegularExpressions;
using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

public sealed partial class QuestionRegionBuilder : IQuestionRegionBuilder
{
    private const float MinAspectRatio = 0.15f;
    private const int MinLineLength = 3;

    private readonly ILogger<QuestionRegionBuilder> _logger;

    public QuestionRegionBuilder(ILogger<QuestionRegionBuilder> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<QuestionRegion> Build(IReadOnlyList<OcrLineInfo> lines, SizeF imageSize)
    {
        var orderedLines = lines
            .Where(x => !string.IsNullOrWhiteSpace(x.Text)
                        && x.Text.Trim().Length >= MinLineLength
                        && x.Bounds.Width > 8
                        && x.Bounds.Height > 6)
            .OrderBy(x => x.Bounds.Y)
            .ThenBy(x => x.Bounds.X)
            .ToList();

        if (orderedLines.Count == 0)
            return [];

        // Determine padding adaptively based on image dimensions.
        // For a typical 1200px-tall homework photo, this gives roughly:
        //   top ~14px, bottom ~35px, left ~25px, right ~25px.
        var topPadding = MathF.Max(8, imageSize.Height * 0.012f);
        var bottomPadding = MathF.Max(12, imageSize.Height * 0.030f);
        var sidePadding = MathF.Max(10, imageSize.Width * 0.020f);

        var grouped = BuildByQuestionNumbers(orderedLines);
        if (grouped.Count == 0)
            grouped = BuildByVerticalGaps(orderedLines);
        else
            grouped = RefineGroups(grouped);

        var regions = grouped
            .Select((group, index) => CreateRegion(group, index + 1, imageSize, topPadding, bottomPadding, sidePadding))
            .Where(x => x.Bounds.Width > 12 && x.Bounds.Height > 10)
            .ToList();

        regions = MergeOverlapping(regions);

        _logger.LogInformation("Question regions built. Count={Count}", regions.Count);
        foreach (var r in regions)
            _logger.LogInformation("Region {Title}: {Bounds}", r.Title, r.Bounds);

        return regions;
    }

    /// <summary>
    /// Group lines by question number patterns like "1.", "第一题", "①", "(1)", "Q1" etc.
    /// </summary>
    private static List<List<OcrLineInfo>> BuildByQuestionNumbers(IReadOnlyList<OcrLineInfo> lines)
    {
        var groups = new List<List<OcrLineInfo>>();
        List<OcrLineInfo>? current = null;

        // Collect preamble lines (before the first question number match)
        // so the top question isn't lost when its number format isn't recognized.
        var preamble = new List<OcrLineInfo>();

        foreach (var line in lines)
        {
            if (QuestionStartRegex().IsMatch(line.Text.Trim()))
            {
                current = [];
                groups.Add(current);
            }

            if (current is not null)
            {
                current.Add(line);
            }
            else
            {
                preamble.Add(line);
            }
        }

        // If we found question numbers AND there is a substantial preamble,
        // prepend it as the first group (it's likely an un-numbered question).
        if (groups.Count > 0 && preamble.Count > 1)
        {
            groups.Insert(0, preamble);
        }

        return groups.Where(g => g.Count > 0).ToList();
    }

    /// <summary>
    /// For groups found by question numbers, split any group whose internal line count
    /// or vertical span suggests it contains multiple questions with missed number patterns.
    /// </summary>
    private static List<List<OcrLineInfo>> RefineGroups(List<List<OcrLineInfo>> groups)
    {
        var refined = new List<List<OcrLineInfo>>();
        foreach (var group in groups)
        {
            if (group.Count <= 2)
            {
                refined.Add(group);
                continue;
            }

            var height = group.Max(l => l.Bounds.Bottom) - group.Min(l => l.Bounds.Top);
            var avgLineHeight = group.Average(l => l.Bounds.Height);

            // Heuristic: if a group spans more than 4 typical line heights, it might
            // contain multiple questions. Try splitting by internal gaps.
            if (height < avgLineHeight * 4)
            {
                refined.Add(group);
                continue;
            }

            var subGroups = BuildByVerticalGaps(group);
            if (subGroups.Count > 1)
            {
                refined.AddRange(subGroups);
            }
            else
            {
                refined.Add(group);
            }
        }

        return refined;
    }

    /// <summary>
    /// Fallback: use statistical gap analysis to find natural question boundaries.
    /// Uses a percentile-based threshold instead of a fixed multiplier.
    /// </summary>
    private static List<List<OcrLineInfo>> BuildByVerticalGaps(IReadOnlyList<OcrLineInfo> lines)
    {
        // Collect all vertical gaps between consecutive lines.
        var gaps = new List<float>();
        float? previousBottom = null;
        foreach (var line in lines)
        {
            if (previousBottom is not null)
            {
                var gap = line.Bounds.Top - previousBottom.Value;
                if (gap > 0) gaps.Add(gap);
            }
            previousBottom = line.Bounds.Bottom;
        }

        if (gaps.Count == 0) return [lines.ToList()];

        gaps.Sort();
        var p25 = gaps[(int)(gaps.Count * 0.25)];
        var p75 = gaps[(int)(gaps.Count * 0.75)];
        var median = gaps[gaps.Count / 2];

        // IQR-based outlier threshold: anything above Q3 + 0.5*IQR is a "large gap".
        // This adapts to the actual gap distribution of each image.
        var iqr = p75 - p25;
        var iqrThreshold = p75 + iqr * 0.5f;

        // Blend with a percentile-based floor for robustness.
        var threshold = MathF.Max(iqrThreshold, median * 2.0f);
        threshold = MathF.Max(threshold, 30f);

        var groups = new List<List<OcrLineInfo>>();
        var current = new List<OcrLineInfo>();
        previousBottom = null;

        foreach (var line in lines)
        {
            if (previousBottom is not null
                && line.Bounds.Top - previousBottom.Value > threshold
                && current.Count > 0)
            {
                groups.Add(current);
                current = [];
            }

            current.Add(line);
            previousBottom = line.Bounds.Bottom;
        }

        if (current.Count > 0) groups.Add(current);

        return groups.Count > 1 ? groups : [];
    }

    /// <summary>
    /// Create a question region from a group of OCR lines with adaptive padding.
    /// </summary>
    private static QuestionRegion CreateRegion(
        IReadOnlyList<OcrLineInfo> lines,
        int index,
        SizeF imageSize,
        float topPadding,
        float bottomPadding,
        float sidePadding)
    {
        var left = lines.Min(x => x.Bounds.Left);
        var top = lines.Min(x => x.Bounds.Top);
        var right = lines.Max(x => x.Bounds.Right);
        var bottom = lines.Max(x => x.Bounds.Bottom);

        // Clamp inside image bounds — don't let padding push us past edges.
        var expandedLeft = MathF.Max(0, left - sidePadding);
        var expandedTop = MathF.Max(0, top - topPadding);
        var expandedRight = MathF.Min(imageSize.Width, right + sidePadding);
        var expandedBottom = MathF.Min(imageSize.Height, bottom + bottomPadding);

        var preview = string.Join(" ", lines.Select(x => x.Text.Trim()));
        if (preview.Length > 80) preview = preview[..80] + "...";

        return new QuestionRegion(
            $"q{index}",
            $"第 {index} 题",
            new RectF(expandedLeft, expandedTop, expandedRight - expandedLeft, expandedBottom - expandedTop),
            preview);
    }

    /// <summary>
    /// Merge question regions that overlap significantly after padding expansion.
    /// This prevents adjacent questions from being merged into one oversized box.
    /// </summary>
    private static List<QuestionRegion> MergeOverlapping(List<QuestionRegion> regions)
    {
        if (regions.Count <= 1) return regions;

        var sorted = regions.OrderBy(r => r.Bounds.Top).ToList();
        var merged = new List<QuestionRegion> { sorted[0] };

        for (var i = 1; i < sorted.Count; i++)
        {
            var prev = merged[^1];
            var curr = sorted[i];

            var overlapTop = MathF.Max(prev.Bounds.Top, curr.Bounds.Top);
            var overlapBottom = MathF.Min(prev.Bounds.Bottom, curr.Bounds.Bottom);
            var overlapHeight = overlapBottom - overlapTop;
            var shorterHeight = MathF.Min(prev.Bounds.Height, curr.Bounds.Height);

            // If overlap exceeds 35% of the shorter region, merge them.
            if (overlapHeight > shorterHeight * 0.35f)
            {
                var mergedRect = new RectF(
                    MathF.Min(prev.Bounds.Left, curr.Bounds.Left),
                    MathF.Min(prev.Bounds.Top, curr.Bounds.Top),
                    MathF.Max(prev.Bounds.Right, curr.Bounds.Right) - MathF.Min(prev.Bounds.Left, curr.Bounds.Left),
                    MathF.Max(prev.Bounds.Bottom, curr.Bounds.Bottom) - MathF.Min(prev.Bounds.Top, curr.Bounds.Top));

                merged[^1] = new QuestionRegion(
                    prev.Id,
                    prev.Title,
                    mergedRect,
                    prev.TextPreview + " | " + curr.TextPreview);
                continue;
            }

            // If regions are very close (small gap), still merge them
            var gap = curr.Bounds.Top - prev.Bounds.Bottom;
            if (gap > 0 && gap < shorterHeight * 0.15f)
            {
                var mergedRect = new RectF(
                    MathF.Min(prev.Bounds.Left, curr.Bounds.Left),
                    prev.Bounds.Top,
                    MathF.Max(prev.Bounds.Right, curr.Bounds.Right) - MathF.Min(prev.Bounds.Left, curr.Bounds.Left),
                    curr.Bounds.Bottom - prev.Bounds.Top);

                merged[^1] = new QuestionRegion(
                    prev.Id,
                    prev.Title,
                    mergedRect,
                    prev.TextPreview + " | " + curr.TextPreview);
                continue;
            }

            merged.Add(curr);
        }

        return merged;
    }

    [GeneratedRegex(
        @"^\s*("
        + @"第\s*\d+\s*题"                     // 第N题
        + @"|[一二三四五六七八九十]+、|十[一二三四五六七八九]、" // 一、十一、
        + @"|\d+\s*[.)、．]"                   // 1. 2) 3、
        + @"|\d+\s{2,}"                        // 1  (number + at least 2 spaces)
        + @"|[\(（]\s*\d+\s*[\)）]"             // (1) （2）
        + @"|[①-⑳]"                            // ①②...⑳
        + @"|[㈠-㈩]"                            // ㈠㈡...
        + @"|Q\d+\s*[.:]"                      // Q1. Q2:
        + @"|Question\s+\d+"                   // Question 1
        + @"|[０-９]+\s*[.．、)]"               // fullwidth １．２．３、
        + @")",
        RegexOptions.IgnoreCase)]
    private static partial Regex QuestionStartRegex();
}
