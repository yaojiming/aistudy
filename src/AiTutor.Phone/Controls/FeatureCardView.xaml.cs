using System.Windows.Input;

namespace AiTutor.Maui.Controls;

public partial class FeatureCardView : ContentView
{
    public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(nameof(TitleText), typeof(string), typeof(FeatureCardView), string.Empty);
    public static readonly BindableProperty DescriptionTextProperty = BindableProperty.Create(nameof(DescriptionText), typeof(string), typeof(FeatureCardView), string.Empty);
    public static readonly BindableProperty IconTextProperty = BindableProperty.Create(nameof(IconText), typeof(string), typeof(FeatureCardView), string.Empty);
    public static readonly BindableProperty CardColorProperty = BindableProperty.Create(nameof(CardColor), typeof(Color), typeof(FeatureCardView), Color.FromArgb("#F7FAF8"));
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FeatureCardView));

    public FeatureCardView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 功能卡片标题。
    /// </summary>
    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    /// <summary>
    /// 功能卡片说明。
    /// </summary>
    public string DescriptionText
    {
        get => (string)GetValue(DescriptionTextProperty);
        set => SetValue(DescriptionTextProperty, value);
    }

    /// <summary>
    /// 卡片左上角图标文字。
    /// </summary>
    public string IconText
    {
        get => (string)GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }

    /// <summary>
    /// 卡片背景色。
    /// </summary>
    public Color CardColor
    {
        get => (Color)GetValue(CardColorProperty);
        set => SetValue(CardColorProperty, value);
    }

    /// <summary>
    /// 点击卡片时执行的命令。
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
