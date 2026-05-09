namespace AiTutor.Maui.Controls;

public partial class LoadingOverlay : ContentView
{
    public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlay), false);
    public static readonly BindableProperty LoadingTextProperty = BindableProperty.Create(nameof(LoadingText), typeof(string), typeof(LoadingOverlay), "AI老师正在思考...");

    public LoadingOverlay()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 是否显示加载遮罩。
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// 加载提示文本。
    /// </summary>
    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }
}
