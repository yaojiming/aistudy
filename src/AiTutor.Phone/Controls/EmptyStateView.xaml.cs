namespace AiTutor.Maui.Controls;

public partial class EmptyStateView : ContentView
{
    public static readonly BindableProperty IconTextProperty = BindableProperty.Create(nameof(IconText), typeof(string), typeof(EmptyStateView), "◇");
    public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(nameof(TitleText), typeof(string), typeof(EmptyStateView), string.Empty);
    public static readonly BindableProperty DescriptionTextProperty = BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(EmptyStateView), string.Empty);

    public EmptyStateView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 空状态图标文字。
    /// </summary>
    public string IconText
    {
        get => (string)GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }

    /// <summary>
    /// 空状态标题。
    /// </summary>
    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    /// <summary>
    /// 空状态说明。
    /// </summary>
    public string DescriptionText
    {
        get => (string)GetValue(DescriptionTextProperty);
        set => SetValue(DescriptionTextProperty, value);
    }
}
