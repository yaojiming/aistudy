namespace AiTutor.Maui.Controls;

public partial class ChatBubbleView : ContentView
{
    public static readonly BindableProperty SpeakerTextProperty = BindableProperty.Create(nameof(SpeakerText), typeof(string), typeof(ChatBubbleView), string.Empty);
    public static readonly BindableProperty MessageTextProperty = BindableProperty.Create(nameof(MessageText), typeof(string), typeof(ChatBubbleView), string.Empty);
    public static readonly BindableProperty BubbleColorProperty = BindableProperty.Create(nameof(BubbleColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#F4F7F5"));
    public static readonly BindableProperty MessageColorProperty = BindableProperty.Create(nameof(MessageColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#1E293B"));
    public static readonly BindableProperty SpeakerColorProperty = BindableProperty.Create(nameof(SpeakerColor), typeof(Color), typeof(ChatBubbleView), Color.FromArgb("#94A3B8"));
    public static readonly BindableProperty BubbleHorizontalOptionsProperty = BindableProperty.Create(nameof(BubbleHorizontalOptions), typeof(LayoutOptions), typeof(ChatBubbleView), LayoutOptions.Start);

    public ChatBubbleView()
    {
        InitializeComponent();
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
}
