using System.Text.RegularExpressions;

namespace AiTutor.Maui.Controls;

public partial class ChatBubbleView : ContentView
{
    public static readonly BindableProperty SpeakerTextProperty = BindableProperty.Create(nameof(SpeakerText), typeof(string), typeof(ChatBubbleView), string.Empty);
    public static readonly BindableProperty MessageTextProperty = BindableProperty.Create(nameof(MessageText), typeof(string), typeof(ChatBubbleView), string.Empty, propertyChanged: OnMarkdownChanged);
    public static readonly BindableProperty BubbleColorProperty = BindableProperty.Create(nameof(BubbleColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#F4F7F5"));
    public static readonly BindableProperty MessageColorProperty = BindableProperty.Create(nameof(MessageColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#1E293B"), propertyChanged: OnMarkdownChanged);
    public static readonly BindableProperty SpeakerColorProperty = BindableProperty.Create(nameof(SpeakerColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#94A3B8"));
    public static readonly BindableProperty BubbleHorizontalOptionsProperty = BindableProperty.Create(nameof(BubbleHorizontalOptions), typeof(LayoutOptions), typeof(ChatBubbleView), LayoutOptions.Start);

    public ChatBubbleView()
    {
        InitializeComponent();
        UpdateFormattedMessage();
    }

    /// <summary>
    /// 气泡说话人。
    /// </summary>
    public string SpeakerText
    {
        get => (string)GetValue(SpeakerTextProperty);
        set => SetValue(SpeakerTextProperty, value);
    }

    /// <summary>
    /// 气泡正文。
    /// </summary>
    public string MessageText
    {
        get => (string)GetValue(MessageTextProperty);
        set => SetValue(MessageTextProperty, value);
    }

    /// <summary>
    /// 气泡背景色。
    /// </summary>
    public Color BubbleColor
    {
        get => (Color)GetValue(BubbleColorProperty);
        set => SetValue(BubbleColorProperty, value);
    }

    public Color MessageColor
    {
        get => (Color)GetValue(MessageColorProperty);
        set => SetValue(MessageColorProperty, value);
    }

    public Color SpeakerColor
    {
        get => (Color)GetValue(SpeakerColorProperty);
        set => SetValue(SpeakerColorProperty, value);
    }

    /// <summary>
    /// 气泡水平位置。
    /// </summary>
    public LayoutOptions BubbleHorizontalOptions
    {
        get => (LayoutOptions)GetValue(BubbleHorizontalOptionsProperty);
        set => SetValue(BubbleHorizontalOptionsProperty, value);
    }

    /// <summary>
    /// 当模型文本或文字颜色变化时，重新解析 Markdown/LaTeX 并刷新气泡内容。
    /// </summary>
    private static void OnMarkdownChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ChatBubbleView)bindable).UpdateFormattedMessage();
    }

    /// <summary>
    /// 把模型返回的 Markdown/LaTeX 原文渲染为 MAUI Label 可显示的富文本。
    /// </summary>
    private void UpdateFormattedMessage()
    {
        MessageLabel.FormattedText = BuildFormattedText(MessageText ?? string.Empty, MessageColor);
    }

    /// <summary>
    /// 支持当前聊天常见格式：Markdown 加粗、标题、列表、分隔线和基础 LaTeX 数学文本。
    /// </summary>
    public static FormattedString BuildFormattedText(string markdown, Color textColor)
    {
        var normalized = NormalizeLatex(markdown)
            .Replace("\\n", Environment.NewLine, StringComparison.Ordinal)
            .Replace("\\r", string.Empty, StringComparison.Ordinal)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

        normalized = Regex.Replace(normalized, @"(?m)^\s*[-—]{3,}\s*$", string.Empty);
        normalized = Regex.Replace(normalized, @"(?m)^(\s*)-\s+", "$1• ");
        normalized = Regex.Replace(normalized, @"\n{3,}", "\n\n").Trim();

        var formatted = new FormattedString();
        var lines = normalized.Split('\n');
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            AppendMarkdownLine(formatted, lines[lineIndex], textColor);
            if (lineIndex < lines.Length - 1)
            {
                formatted.Spans.Add(new Span { Text = Environment.NewLine, TextColor = textColor });
            }
        }

        return formatted;
    }

    /// <summary>
    /// 解析一行内的 Markdown 标题和粗体标记，并映射为 Span.FontAttributes。
    /// </summary>
    private static void AppendMarkdownLine(FormattedString formatted, string line, Color textColor)
    {
        var fontAttributes = FontAttributes.None;
        var headingMatch = Regex.Match(line, @"^\s{0,3}#{1,6}\s+(.+)$");
        if (headingMatch.Success)
        {
            line = headingMatch.Groups[1].Value;
            fontAttributes = FontAttributes.Bold;
        }

        var cursor = 0;
        foreach (Match match in Regex.Matches(line, @"(\*\*|__)(.+?)\1"))
        {
            if (match.Index > cursor)
            {
                formatted.Spans.Add(CreateSpan(line[cursor..match.Index], textColor, fontAttributes));
            }

            formatted.Spans.Add(CreateSpan(match.Groups[2].Value, textColor, FontAttributes.Bold));
            cursor = match.Index + match.Length;
        }

        if (cursor < line.Length)
        {
            formatted.Spans.Add(CreateSpan(line[cursor..], textColor, fontAttributes));
        }
    }

    private static Span CreateSpan(string text, Color textColor, FontAttributes fontAttributes)
    {
        return new Span
        {
            Text = text.Replace("`", string.Empty, StringComparison.Ordinal),
            TextColor = textColor,
            FontAttributes = fontAttributes
        };
    }

    /// <summary>
    /// 将常见 LaTeX 数学文本转为小学生更容易读、MAUI Label 也能稳定显示的普通文本。
    /// </summary>
    private static string NormalizeLatex(string text)
    {
        var normalized = text;

        normalized = Regex.Replace(normalized, @"\\+(?:dfrac|tfrac|frac)\{([^{}]+)\}\{([^{}]+)\}", "$1/$2");
        normalized = Regex.Replace(normalized, @"\\+(?:text|mathrm|operatorname)\{([^{}]*)\}", "$1");
        normalized = Regex.Replace(normalized, @"\\+(?:left|right)", string.Empty);
        normalized = Regex.Replace(normalized, @"\\+(?:quad|qquad|,|;|:|!)", " ");
        normalized = Regex.Replace(normalized, @"\\+\(", string.Empty);
        normalized = Regex.Replace(normalized, @"\\+\)", string.Empty);
        normalized = Regex.Replace(normalized, @"\\+\[", string.Empty);
        normalized = Regex.Replace(normalized, @"\\+\]", string.Empty);
        normalized = Regex.Replace(normalized, @"\${1,2}([^$]+?)\${1,2}", "$1");
        normalized = Regex.Replace(normalized, @"\^\{?2\}?", "²");
        normalized = Regex.Replace(normalized, @"\^\{?3\}?", "³");
        normalized = Regex.Replace(normalized, @"\^\{([^{}]+)\}", "^$1");
        normalized = Regex.Replace(normalized, @"_\{([^{}]+)\}", "_$1");

        return normalized
            .Replace(@"\times", "×", StringComparison.Ordinal)
            .Replace(@"\div", "÷", StringComparison.Ordinal)
            .Replace(@"\cdot", "·", StringComparison.Ordinal)
            .Replace(@"\leq", "≤", StringComparison.Ordinal)
            .Replace(@"\le", "≤", StringComparison.Ordinal)
            .Replace(@"\geq", "≥", StringComparison.Ordinal)
            .Replace(@"\ge", "≥", StringComparison.Ordinal)
            .Replace(@"\neq", "≠", StringComparison.Ordinal)
            .Replace(@"\approx", "≈", StringComparison.Ordinal)
            .Replace(@"\pi", "π", StringComparison.Ordinal)
            .Replace("\\", string.Empty, StringComparison.Ordinal);
    }
}
