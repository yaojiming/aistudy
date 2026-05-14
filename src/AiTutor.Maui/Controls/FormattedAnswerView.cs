using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AiTutor.Maui.Controls;

/// <summary>
/// 鐢ㄤ簬鎷嶇収璁查銆佷綔涓氭鏌ョ瓑椤甸潰鐨?Markdown/LaTeX 绛旀灞曠ず鎺т欢銆?/// WebView 鍙垵濮嬪寲涓€娆★紝娴佸紡杈撳嚭鏃堕€氳繃 JavaScript 鏇存柊姝ｆ枃锛岄伩鍏嶆粴鍔ㄦ潯涓婁笅璺冲姩銆?/// </summary>
public sealed class FormattedAnswerView : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(FormattedAnswerView),
        string.Empty,
        propertyChanged: OnTextChanged);

    public static readonly BindableProperty AnswerTextColorProperty = BindableProperty.Create(
        nameof(AnswerTextColor),
        typeof(Color),
        typeof(FormattedAnswerView),
        Color.FromArgb("#334155"),
        propertyChanged: OnTextChanged);

    public static readonly BindableProperty AnswerFontSizeProperty = BindableProperty.Create(
        nameof(AnswerFontSize),
        typeof(double),
        typeof(FormattedAnswerView),
        16d,
        propertyChanged: OnTextChanged);

    public static readonly BindableProperty IsStreamingProperty = BindableProperty.Create(
        nameof(IsStreaming),
        typeof(bool),
        typeof(FormattedAnswerView),
        false,
        propertyChanged: OnTextChanged);

    private readonly WebView _webView = new()
    {
        BackgroundColor = Colors.Transparent,
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill
    };

    private bool _isWebViewInitialized;
    private bool _isDocumentLoaded;
    private int _renderVersion;
    private string _pendingBodyHtml = "<p></p>";
    private string _pendingPlainText = string.Empty;

    public FormattedAnswerView()
    {
        _webView.Navigated += OnWebViewNavigated;
        _webView.HandlerChanged += OnWebViewHandlerChanged;
        Loaded += OnLoaded;
        Content = _webView;

        _pendingBodyHtml = MarkdownToHtml(NormalizeNewLines(Text ?? string.Empty));
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Color AnswerTextColor
    {
        get => (Color)GetValue(AnswerTextColorProperty);
        set => SetValue(AnswerTextColorProperty, value);
    }

    public double AnswerFontSize
    {
        get => (double)GetValue(AnswerFontSizeProperty);
        set => SetValue(AnswerFontSizeProperty, value);
    }

    public bool IsStreaming
    {
        get => (bool)GetValue(IsStreamingProperty);
        set => SetValue(IsStreamingProperty, value);
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FormattedAnswerView)bindable).UpdateFormattedText();
    }

    /// <summary>
    /// 灏嗘柊鍐呭鍐欏叆宸叉湁 WebView 鏂囨。锛岄伩鍏嶆瘡涓祦寮?delta 閮介噸鏂板姞杞芥暣椤点€?    /// </summary>
    private async void UpdateFormattedText()
    {
        var normalizedText = NormalizeNewLines(Text ?? string.Empty);
        if (IsStreaming)
        {
            _pendingPlainText = normalizedText;
        }
        else
        {
            _pendingBodyHtml = MarkdownToHtml(normalizedText);
        }

        EnsureWebViewDocumentLoaded();

        if (!_isDocumentLoaded)
        {
            return;
        }

        var version = ++_renderVersion;
        if (IsStreaming)
        {
            await RenderPlainTextAsync(version);
        }
        else
        {
            await RenderPendingContentAsync(version);
        }
    }

    /// <summary>
    /// WebView 棣栨鍔犺浇瀹屾垚鍚庢覆鏌撳綋鍓嶇紦瀛樺唴瀹广€?    /// </summary>
    private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _isDocumentLoaded = true;
        var version = ++_renderVersion;
        if (IsStreaming)
        {
            _pendingPlainText = NormalizeNewLines(Text ?? string.Empty);
            await RenderPlainTextAsync(version);
        }
        else
        {
            _pendingBodyHtml = MarkdownToHtml(NormalizeNewLines(Text ?? string.Empty));
            await RenderPendingContentAsync(version);
        }
    }

    /// <summary>
    /// 页面真正加载后再初始化 WebView 文档，避免 Android 在控件构造阶段创建原生代理时抛出 JavaProxyThrowable。
    /// </summary>
    private void OnLoaded(object? sender, EventArgs e)
    {
        EnsureWebViewDocumentLoaded();
    }

    /// <summary>
    /// Android 鍘熺敓 WebView 鎵撳紑婊氬姩鏉★紝骞朵娇鐢ㄩ€忔槑鑳屾櫙铻嶅叆绛旀鍗＄墖銆?    /// </summary>
    private void OnWebViewHandlerChanged(object? sender, EventArgs e)
    {
#if ANDROID
        if (_webView.Handler?.PlatformView is Android.Webkit.WebView androidWebView)
        {
            androidWebView.VerticalScrollBarEnabled = true;
            androidWebView.HorizontalScrollBarEnabled = true;
            androidWebView.ScrollBarStyle = Android.Views.ScrollbarStyles.InsideOverlay;
            androidWebView.Settings.JavaScriptEnabled = true;
            androidWebView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
#endif
        EnsureWebViewDocumentLoaded();
    }

    /// <summary>
    /// 只给 WebView 注入一次 HTML 外壳，后续流式内容通过 JavaScript 更新正文。
    /// </summary>
    private void EnsureWebViewDocumentLoaded()
    {
        if (_isWebViewInitialized || _webView.Handler is null)
        {
            return;
        }

        _isWebViewInitialized = true;
        _webView.Source = new HtmlWebViewSource
        {
            Html = BuildHtmlDocument(AnswerTextColor, AnswerFontSize)
        };
    }

    private async Task RenderPendingContentAsync(int version)
    {
        try
        {
            await Task.Delay(20);
            if (version != _renderVersion)
            {
                return;
            }

            var bodyJson = JsonSerializer.Serialize(_pendingBodyHtml);
            var colorJson = JsonSerializer.Serialize(ToCssColor(AnswerTextColor));
            var fontSizeJson = JsonSerializer.Serialize(AnswerFontSize.ToString(CultureInfo.InvariantCulture) + "px");

            await _webView.EvaluateJavaScriptAsync($$"""
                (function () {
                    if (window.setAnswerHtml) {
                        return window.setAnswerHtml({{bodyJson}}, {{colorJson}}, {{fontSizeJson}});
                    }
                    return 'missing-setAnswerHtml';
                })();
                """);
        }
        catch
        {
            // 渲染失败时保持 WebView 当前内容，避免打断流式回答。
        }
    }

    private async Task RenderPlainTextAsync(int version)
    {
        try
        {
            await Task.Delay(35);
            if (version != _renderVersion)
            {
                return;
            }

            var textJson = JsonSerializer.Serialize(_pendingPlainText);

            await _webView.EvaluateJavaScriptAsync($$"""
                (function () {
                    if (window.setAnswerText) {
                        return window.setAnswerText({{textJson}});
                    }
                    return 'missing-setAnswerText';
                })();
                """);
        }
        catch
        {
            // 流式渲染失败不要中断回答，下一次 delta 或最终渲染会继续尝试。
        }
    }

    private static string BuildHtmlDocument(Color textColor, double fontSize)
    {
        var color = ToCssColor(textColor);
        var lineHeight = 1.55d;

        return $$"""
            <!doctype html>
            <html>
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0">
              <style>
                :root {
                  --answer-color: {{color}};
                  --answer-font-size: {{fontSize.ToString(CultureInfo.InvariantCulture)}}px;
                }

                html, body {
                  margin: 0;
                  padding: 0;
                  min-height: 100%;
                  background: transparent;
                  color: var(--answer-color);
                  font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
                  font-size: var(--answer-font-size);
                  line-height: {{lineHeight.ToString(CultureInfo.InvariantCulture)}};
                  overflow-x: hidden;
                  overflow-y: auto;
                  overflow-wrap: anywhere;
                }

                body {
                  padding: 0 6px 20px 2px;
                  box-sizing: border-box;
                }

                p {
                  margin: 0 0 14px 0;
                }

                h1, h2, h3, h4 {
                  margin: 18px 0 10px 0;
                  color: #1E293B;
                  font-weight: 800;
                  line-height: 1.25;
                }

                h1 { font-size: 1.34em; }
                h2 { font-size: 1.24em; }
                h3 { font-size: 1.14em; }
                h4 { font-size: 1.05em; }

                ul, ol {
                  margin: 0 0 14px 1.2em;
                  padding: 0;
                }

                li {
                  margin: 4px 0;
                  padding-left: 0.12em;
                }

                strong {
                  color: #1E293B;
                  font-weight: 800;
                }

                code {
                  padding: 1px 5px;
                  border-radius: 6px;
                  background: rgba(15, 23, 42, 0.07);
                  font-family: ui-monospace, SFMono-Regular, Consolas, monospace;
                }

                .section-title {
                  display: block;
                  margin: 18px 0 8px 0;
                  color: #1E293B;
                  font-weight: 800;
                }

                details.thinking-panel {
                  margin: 0 0 16px 0;
                  padding: 12px 14px;
                  border: 1px solid #CBD5E1;
                  border-radius: 14px;
                  background: rgba(255, 255, 255, 0.62);
                  color: #64748B;
                }

                details.thinking-panel summary {
                  color: #475569;
                  cursor: pointer;
                  font-weight: 800;
                  user-select: none;
                }

                details.thinking-panel .thinking-body {
                  margin-top: 10px;
                  font-size: 0.92em;
                }

                mjx-container {
                  color: #1E293B;
                  overflow-x: auto;
                  overflow-y: hidden;
                  max-width: 100%;
                }

                #answer.streaming {
                  white-space: pre-wrap;
                }

                #answer.final {
                  white-space: normal;
                }
              </style>
              <script>
                window.MathJax = {
                  tex: {
                    inlineMath: [['$', '$'], ['\\(', '\\)']],
                    displayMath: [['$$', '$$'], ['\\[', '\\]']],
                    processEscapes: true,
                    processEnvironments: true
                  },
                  svg: {
                    fontCache: 'none'
                  },
                  options: {
                    skipHtmlTags: ['script', 'noscript', 'style', 'textarea', 'pre', 'code']
                  }
                };
              </script>
              <script async src="https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-svg.js"></script>
              <script>
                (function () {
                  const scrollToBottom = function () {
                    window.scrollTo(0, Math.max(document.body.scrollHeight, document.documentElement.scrollHeight));
                  };

                  let pendingText = '';
                  let pendingFrame = 0;

                  window.setAnswerText = function (text) {
                    pendingText = text || '';
                    if (pendingFrame) {
                      return 'queued';
                    }

                    pendingFrame = requestAnimationFrame(function () {
                      pendingFrame = 0;
                      const answer = document.getElementById('answer');
                      if (!answer) {
                        return;
                      }

                      answer.classList.remove('final');
                      answer.classList.add('streaming');
                      answer.textContent = pendingText;
                      scrollToBottom();
                    });

                    return 'ok';
                  };

                  window.setAnswerHtml = function (html, color, fontSize) {
                    const answer = document.getElementById('answer');
                    if (!answer) {
                      return 'missing-answer';
                    }

                    document.documentElement.style.setProperty('--answer-color', color);
                    document.documentElement.style.setProperty('--answer-font-size', fontSize);
                    answer.classList.remove('streaming');
                    answer.classList.add('final');
                    answer.innerHTML = html || '';

                    if (window.MathJax && MathJax.typesetPromise) {
                      if (MathJax.typesetClear) {
                        MathJax.typesetClear([answer]);
                      }

                      MathJax.typesetPromise([answer]).then(scrollToBottom).catch(scrollToBottom);
                    } else {
                      scrollToBottom();
                    }

                    return 'ok';
                  };
                })();
              </script>
            </head>
            <body>
              <main id="answer" class="final"></main>
            </body>
            </html>
            """;
    }

    private static string NormalizeNewLines(string text)
    {
        return text
            .Replace("\\n", Environment.NewLine, StringComparison.Ordinal)
            .Replace("\\r", string.Empty, StringComparison.Ordinal)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Trim();
    }

    /// <summary>
    /// 杞婚噺瑙ｆ瀽妯″瀷甯哥敤 Markdown銆侺aTeX 鍘熸牱淇濈暀缁?MathJax 澶勭悊銆?    /// </summary>
    private static string MarkdownToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return "<p></p>";
        }

        var html = new StringBuilder();
        var lines = markdown.Split('\n');
        var listMode = ListMode.None;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var rawLine = lines[lineIndex];
            var line = rawLine.TrimEnd();
            if (line.Trim() == ":::thinking")
            {
                CloseListIfNeeded(html, ref listMode);
                var thinkingMarkdown = new StringBuilder();
                lineIndex++;
                while (lineIndex < lines.Length && lines[lineIndex].Trim() != ":::")
                {
                    thinkingMarkdown.AppendLine(lines[lineIndex]);
                    lineIndex++;
                }

                html.Append(BuildThinkingDetailsHtml(thinkingMarkdown.ToString()));
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                CloseListIfNeeded(html, ref listMode);
                continue;
            }

            var heading = Regex.Match(line, @"^\s{0,3}(#{1,4})\s+(.+)$");
            if (heading.Success)
            {
                CloseListIfNeeded(html, ref listMode);
                var level = Math.Min(heading.Groups[1].Value.Length, 4);
                html.Append(CultureInfo.InvariantCulture, $"<h{level}>{InlineMarkdownToHtml(heading.Groups[2].Value)}</h{level}>");
                continue;
            }

            var bullet = Regex.Match(line, @"^\s*[-*+]\s+(.+)$");
            if (bullet.Success)
            {
                OpenListIfNeeded(html, ref listMode, ListMode.Unordered);
                html.Append(CultureInfo.InvariantCulture, $"<li>{InlineMarkdownToHtml(bullet.Groups[1].Value)}</li>");
                continue;
            }

            var ordered = Regex.Match(line, @"^\s*\d+(?:[.)]|\u3001|\uFF0E)\s+(.+)$");
            if (ordered.Success)
            {
                OpenListIfNeeded(html, ref listMode, ListMode.Ordered);
                html.Append(CultureInfo.InvariantCulture, $"<li>{InlineMarkdownToHtml(ordered.Groups[1].Value)}</li>");
                continue;
            }

            CloseListIfNeeded(html, ref listMode);
            html.Append(CultureInfo.InvariantCulture, $"<p>{InlineMarkdownToHtml(line)}</p>");
        }

        CloseListIfNeeded(html, ref listMode);
        return html.ToString();
    }

    /// <summary>
    /// 灏嗘€濊€冭繃绋嬫覆鏌撲负榛樿鎶樺彔鐨?details 鍖哄潡锛屾寮忕瓟妗堜繚鎸佺洿鎺ュ彲瑙併€?    /// </summary>
    private static string BuildThinkingDetailsHtml(string thinkingMarkdown)
    {
        var bodyHtml = MarkdownToHtml(thinkingMarkdown);
        return
            $"<details class=\"thinking-panel\"><summary>&#26597;&#30475;&#24605;&#32771;&#36807;&#31243;</summary><div class=\"thinking-body\">{bodyHtml}</div></details>";
    }

    private static string InlineMarkdownToHtml(string text)
    {
        var encoded = WebUtility.HtmlEncode(text);

        encoded = Regex.Replace(encoded, @"(\*\*|__)(.+?)\1", "<strong>$2</strong>");
        encoded = Regex.Replace(encoded, @"`(.+?)`", "<code>$1</code>");
        encoded = Regex.Replace(encoded, @"^【([^】]+)】", "<span class=\"section-title\">【$1】</span>");

        return encoded;
    }

    private static void OpenListIfNeeded(StringBuilder html, ref ListMode currentMode, ListMode nextMode)
    {
        if (currentMode == nextMode)
        {
            return;
        }

        CloseListIfNeeded(html, ref currentMode);
        html.Append(nextMode == ListMode.Ordered ? "<ol>" : "<ul>");
        currentMode = nextMode;
    }

    private static void CloseListIfNeeded(StringBuilder html, ref ListMode currentMode)
    {
        if (currentMode == ListMode.None)
        {
            return;
        }

        html.Append(currentMode == ListMode.Ordered ? "</ol>" : "</ul>");
        currentMode = ListMode.None;
    }

    private static string ToCssColor(Color color)
    {
        var red = (int)Math.Round(color.Red * 255);
        var green = (int)Math.Round(color.Green * 255);
        var blue = (int)Math.Round(color.Blue * 255);
        return FormattableString.Invariant($"rgb({red},{green},{blue})");
    }

    private enum ListMode
    {
        None,
        Unordered,
        Ordered
    }
}
