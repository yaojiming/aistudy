using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Markdig;

namespace AiTutor.Maui.Controls;

/// <summary>
/// 鐢ㄤ簬鎷嶇収璁查銆佷綔涓氭鏌ョ瓑椤甸潰鐨?Markdown/LaTeX 绛旀灞曠ず鎺т欢銆?/// WebView 鍙垵濮嬪寲涓€娆★紝娴佸紡杈撳嚭鏃堕€氳繃 JavaScript 鏇存柊姝ｆ枃锛岄伩鍏嶆粴鍔ㄦ潯涓婁笅璺冲姩銆?/// </summary>
public sealed partial class FormattedAnswerView : ContentView
{
#if WINDOWS
    private const bool PreferNativeTextRenderer = true;
#else
    private const bool PreferNativeTextRenderer = false;
#endif

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

    private readonly Grid _rootLayout = new();
    private readonly Label _nativeTextLabel = new()
    {
        FontSize = 16,
        LineHeight = 1.45,
        TextColor = Color.FromArgb("#334155"),
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Start
    };

    private readonly ScrollView _nativeTextScrollView = new()
    {
        Orientation = ScrollOrientation.Vertical,
        HorizontalOptions = LayoutOptions.Fill,
        VerticalOptions = LayoutOptions.Fill
    };

    private bool _isWebViewInitialized;
    private bool _isDocumentLoaded;
    private int _renderVersion;
    private string _pendingContentHtml = "<p></p>";
    private bool _pendingRunMathJax;

    public FormattedAnswerView()
    {
        _webView.Navigated += OnWebViewNavigated;
        _webView.HandlerChanged += OnWebViewHandlerChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        _nativeTextScrollView.Content = _nativeTextLabel;
        _rootLayout.Children.Add(_nativeTextScrollView);
        _rootLayout.Children.Add(_webView);
        Content = _rootLayout;

        UpdateNativeTextFallback(NormalizeNewLines(Text ?? string.Empty));
        UpdateRendererVisibility();

        _pendingContentHtml = MarkdownToHtml(NormalizeNewLines(Text ?? string.Empty));
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _webView.Navigated -= OnWebViewNavigated;
        _webView.HandlerChanged -= OnWebViewHandlerChanged;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
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

    private async void UpdateFormattedText()
    {
        var normalizedText = NormalizeNewLines(Text ?? string.Empty);
        UpdateNativeTextFallback(normalizedText);

        if (IsStreaming)
        {
            // Lightweight: escape HTML + <br> for line breaks. LaTeX delimiters ($...$) stay visible as raw text.
            _pendingContentHtml = EscapeHtmlWithBreaks(normalizedText);
            _pendingRunMathJax = false;
        }
        else
        {
            // Final render: full Markdig parse + MathJax typesetting.
            _pendingContentHtml = MarkdownToHtml(normalizedText);
            _pendingRunMathJax = true;
        }

        EnsureWebViewDocumentLoaded();

        if (!_isDocumentLoaded)
            return;

        var version = ++_renderVersion;
        await RenderContentAsync(version);
    }

    private static string EscapeHtmlWithBreaks(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "<p></p>";

        var escaped = text
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal)
            .Replace("\n", "<br>", StringComparison.Ordinal);

        return $"<p style=\"white-space:pre-wrap\">{escaped}</p>";
    }

    /// <summary>
    /// WebView 棣栨鍔犺浇瀹屾垚鍚庢覆鏌撳綋鍓嶇紦瀛樺唴瀹广€?    /// </summary>
    private async void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _isDocumentLoaded = true;
        UpdateRendererVisibility();

        var normalized = NormalizeNewLines(Text ?? string.Empty);
        if (IsStreaming)
            _pendingContentHtml = EscapeHtmlWithBreaks(normalized);
        else
            _pendingContentHtml = MarkdownToHtml(normalized);
        _pendingRunMathJax = !IsStreaming;

        var version = ++_renderVersion;
        await RenderContentAsync(version);
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

    private async Task RenderContentAsync(int version)
    {
        try
        {
            await Task.Delay(_pendingRunMathJax ? 20 : 35);
            if (version != _renderVersion)
                return;

            var htmlJson = JsonSerializer.Serialize(_pendingContentHtml);
            var runMathJaxJson = JsonSerializer.Serialize(_pendingRunMathJax);
            var colorJson = JsonSerializer.Serialize(ToCssColor(AnswerTextColor));
            var fontSizeJson = JsonSerializer.Serialize(AnswerFontSize.ToString(CultureInfo.InvariantCulture) + "px");

            await _webView.EvaluateJavaScriptAsync($$"""
                (function(){
                    if(window.setAnswerHtml){
                        return window.setAnswerHtml({{htmlJson}},{{runMathJaxJson}},{{colorJson}},{{fontSizeJson}});
                    }
                    return 'missing-setAnswerHtml';
                })();
                """);
        }
        catch
        {
            ShowNativeTextFallback();
        }
    }

    private void UpdateNativeTextFallback(string normalizedText)
    {
        _nativeTextLabel.Text = MarkdownToReadableText(normalizedText);
        _nativeTextLabel.TextColor = AnswerTextColor;
        _nativeTextLabel.FontSize = AnswerFontSize;
        UpdateRendererVisibility();
    }

    private void UpdateRendererVisibility()
    {
        var useNativeText = PreferNativeTextRenderer || !_isDocumentLoaded;
        _nativeTextScrollView.IsVisible = useNativeText;
        _webView.IsVisible = !useNativeText;
    }

    private void ShowNativeTextFallback()
    {
        _nativeTextScrollView.IsVisible = true;
        _webView.IsVisible = false;
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
                  const answerEl = document.getElementById('answer');

                  const scrollToBottom = function () {
                    // Try multiple approaches for WebView compatibility.
                    var h = Math.max(
                      document.body.scrollHeight || 0,
                      document.documentElement.scrollHeight || 0,
                      document.body.offsetHeight || 0,
                      document.documentElement.offsetHeight || 0
                    );
                    window.scrollTo({ top: h, behavior: 'instant' });
                    document.documentElement.scrollTop = h;
                    document.body.scrollTop = h;
                    // Fallback: scroll last element into view.
                    if (answerEl && answerEl.lastElementChild) {
                      answerEl.lastElementChild.scrollIntoView(false);
                    }
                  };

                  let pending = null;
                  let pendingFrame = 0;

                  window.setAnswerHtml = function (html, runMathJax, color, fontSize) {
                    pending = { html: html || '', runMathJax: !!runMathJax, color: color, fontSize: fontSize };
                    if (pendingFrame) return 'queued';

                    pendingFrame = requestAnimationFrame(function () {
                      pendingFrame = 0;
                      const p = pending;
                      if (!p || !answerEl) return;

                      document.documentElement.style.setProperty('--answer-color', p.color);
                      document.documentElement.style.setProperty('--answer-font-size', p.fontSize);

                      if (p.runMathJax) {
                        answerEl.classList.remove('streaming');
                        answerEl.classList.add('final');
                      } else {
                        answerEl.classList.add('streaming');
                        answerEl.classList.remove('final');
                      }

                      answerEl.innerHTML = p.html;

                      // Defer scroll until after the browser finishes layout (double-rAF).
                      requestAnimationFrame(function () {
                        if (p.runMathJax && window.MathJax && MathJax.typesetPromise) {
                          if (MathJax.typesetClear) MathJax.typesetClear([answerEl]);
                          MathJax.typesetPromise([answerEl]).then(scrollToBottom).catch(scrollToBottom);
                        } else {
                          scrollToBottom();
                        }
                      });
                    });

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

    private static string MarkdownToReadableText(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var text = NormalizeNewLines(markdown)
            .Replace(":::thinking", "思考过程：", StringComparison.Ordinal)
            .Replace(":::", string.Empty, StringComparison.Ordinal);

        text = Regex.Replace(text, @"(?m)^\s{0,3}#{1,6}\s*", string.Empty);
        text = Regex.Replace(text, @"(\*\*|__)(.+?)\1", "$2");
        text = Regex.Replace(text, @"`(.+?)`", "$1");
        text = Regex.Replace(text, @"\\+(?:text|mathrm|operatorname)\{([^{}]*)\}", "$1");
        text = Regex.Replace(text, @"\\+(?:dfrac|tfrac|frac)\{([^{}]+)\}\{([^{}]+)\}", "$1/$2");
        text = Regex.Replace(text, @"\\+(?:left|right)", string.Empty);
        text = Regex.Replace(text, @"\\+(?:quad|qquad|,|;|:|!)", " ");
        text = Regex.Replace(text, @"\\+\(|\\+\)|\\+\[|\\+\]", string.Empty);
        text = Regex.Replace(text, @"\${1,2}([^$]+?)\${1,2}", "$1");

        return text
            .Replace("\\n", Environment.NewLine, StringComparison.Ordinal)
            .Replace("\\", string.Empty, StringComparison.Ordinal)
            .Trim();
    }

    /// <summary>
    /// 杞婚噺瑙ｆ瀽妯″瀷甯哥敤 Markdown銆侺aTeX 鍘熸牱淇濈暀缁?MathJax 澶勭悊銆?    /// </summary>
    private static string MarkdownToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return "<p></p>";

        var thinkingBlocks = new List<string>();
        var preprocessed = ThinkingBlockRegex().Replace(markdown, match =>
        {
            var i = thinkingBlocks.Count;
            thinkingBlocks.Add(match.Groups[1].Value.Trim());
            return $"<!--thinking-{i}-->";
        });

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        var html = Markdig.Markdown.ToHtml(preprocessed, pipeline);

        for (var i = 0; i < thinkingBlocks.Count; i++)
        {
            var innerHtml = Markdig.Markdown.ToHtml(thinkingBlocks[i], pipeline);
            var details = $"<details class=\"thinking-panel\"><summary>&#26597;&#30475;&#24605;&#32771;&#36807;&#31243;</summary><div class=\"thinking-body\">{innerHtml}</div></details>";
            html = html.Replace($"<!--thinking-{i}-->", details);
        }

        html = SectionTitleRegex().Replace(html, "<span class=\"section-title\">$1</span>");

        return html;
    }

    private static string ToCssColor(Color color)
    {
        var red = (int)Math.Round(color.Red * 255);
        var green = (int)Math.Round(color.Green * 255);
        var blue = (int)Math.Round(color.Blue * 255);
        return FormattableString.Invariant($"rgb({red},{green},{blue})");
    }

    [GeneratedRegex(@":::thinking\s*\n(.*?)\n\s*:::", RegexOptions.Singleline)]
    private static partial Regex ThinkingBlockRegex();

    [GeneratedRegex(@"【([^】]+)】")]
    private static partial Regex SectionTitleRegex();
}
