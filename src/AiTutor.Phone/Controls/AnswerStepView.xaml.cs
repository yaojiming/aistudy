namespace AiTutor.Maui.Controls;

public partial class AnswerStepView : ContentView
{
    public static readonly BindableProperty StepNoProperty = BindableProperty.Create(nameof(StepNo), typeof(string), typeof(AnswerStepView), "1");
    public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(nameof(TitleText), typeof(string), typeof(AnswerStepView), string.Empty);
    public static readonly BindableProperty BodyTextProperty = BindableProperty.Create(nameof(BodyText), typeof(string), typeof(AnswerStepView), string.Empty);

    public AnswerStepView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 步骤序号。
    /// </summary>
    public string StepNo
    {
        get => (string)GetValue(StepNoProperty);
        set => SetValue(StepNoProperty, value);
    }

    /// <summary>
    /// 步骤标题。
    /// </summary>
    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    /// <summary>
    /// 步骤说明。
    /// </summary>
    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }
}
