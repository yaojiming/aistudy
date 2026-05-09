namespace AiTutor.Maui.Controls;

public partial class StudyStageCard : ContentView
{
    public static readonly BindableProperty StageTitleProperty = BindableProperty.Create(nameof(StageTitle), typeof(string), typeof(StudyStageCard), string.Empty);
    public static readonly BindableProperty StageGoalProperty = BindableProperty.Create(nameof(StageGoal), typeof(string), typeof(StudyStageCard), string.Empty);
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(double), typeof(StudyStageCard), 0.0);

    public StudyStageCard()
    {
        InitializeComponent();
    }

    public string StageTitle { get => (string)GetValue(StageTitleProperty); set => SetValue(StageTitleProperty, value); }
    public string StageGoal { get => (string)GetValue(StageGoalProperty); set => SetValue(StageGoalProperty, value); }
    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }
}
