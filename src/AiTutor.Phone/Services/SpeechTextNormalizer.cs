using System.Text.RegularExpressions;

namespace AiTutor.Maui.Services;

/// <summary>
/// 将 Markdown、LaTeX 和少量显示符号清理成适合 TTS 播报的普通中文文本。
/// </summary>
public static class SpeechTextNormalizer
{
    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text
            .Replace("AI TUTOR", "AI老师", StringComparison.OrdinalIgnoreCase)
            .Replace(@"\times", "乘以", StringComparison.OrdinalIgnoreCase)
            .Replace(@"\div", "除以", StringComparison.OrdinalIgnoreCase)
            .Replace(@"\frac", "分数", StringComparison.OrdinalIgnoreCase)
            .Replace("×", "乘以", StringComparison.Ordinal)
            .Replace("÷", "除以", StringComparison.Ordinal);

        normalized = Regex.Replace(normalized, @"\$\$?([^$]+)\$\$?", "$1");
        normalized = Regex.Replace(normalized, @"\\text\s*\{([^}]*)\}", "$1");
        normalized = Regex.Replace(normalized, @"[#*_`>$\\{}]", string.Empty);
        normalized = Regex.Replace(normalized, @"\[(.*?)\]\((.*?)\)", "$1");
        normalized = Regex.Replace(normalized, @"\s+", " ");

        return normalized.Trim();
    }
}
