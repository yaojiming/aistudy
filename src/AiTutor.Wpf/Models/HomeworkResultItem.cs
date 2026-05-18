using AiTutor.Shared.Agent;
using System.Windows.Media;

namespace AiTutor.Wpf.Models;

public sealed class HomeworkResultItem
{
    public string QuestionNo { get; set; } = string.Empty;

    public string QuestionText { get; set; } = string.Empty;

    public string StudentAnswer { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public bool? IsCorrect { get; set; }

    public string ErrorReason { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public string KnowledgePointName { get; set; } = string.Empty;

    public HomeworkCheckBBoxDto? BBox { get; set; }

    public string StatusText => IsCorrect switch
    {
        true => "✓ 正确",
        false => "× 错误",
        _ => "不确定"
    };

    public Brush StatusBrush => IsCorrect switch
    {
        true => new SolidColorBrush(Color.FromRgb(34, 197, 94)),
        false => new SolidColorBrush(Color.FromRgb(225, 93, 104)),
        _ => new SolidColorBrush(Color.FromRgb(100, 116, 139))
    };

    public string ShortResult => string.IsNullOrWhiteSpace(ErrorReason)
        ? Truncate(Explanation, 42)
        : ErrorReason;

    public static HomeworkResultItem FromDto(HomeworkCheckItemDto item)
    {
        return new HomeworkResultItem
        {
            QuestionNo = item.QuestionNo,
            QuestionText = item.QuestionText,
            StudentAnswer = item.StudentAnswer ?? string.Empty,
            CorrectAnswer = item.CorrectAnswer ?? string.Empty,
            IsCorrect = item.IsCorrect,
            ErrorReason = item.ErrorReason ?? string.Empty,
            Explanation = item.Explanation,
            KnowledgePointName = item.KnowledgePointName ?? string.Empty,
            BBox = item.BBox
        };
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }
}

public sealed class HomeworkRegionBox
{
    public string QuestionNo { get; set; } = string.Empty;

    public double Left { get; set; }

    public double Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public bool? IsCorrect { get; set; }

    public HomeworkResultItem? Result { get; set; }

    public string Marker => IsCorrect switch
    {
        true => "✓",
        false => "×",
        _ => ""
    };

    public Brush BorderBrush => IsCorrect switch
    {
        true => new SolidColorBrush(Color.FromRgb(34, 197, 94)),
        false => new SolidColorBrush(Color.FromRgb(225, 93, 104)),
        _ => new SolidColorBrush(Color.FromRgb(56, 189, 248))
    };
}
