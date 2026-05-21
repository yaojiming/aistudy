using System.Windows.Input;

namespace AiTutor.Maui.Controls;

public partial class WrongQuestionCard : ContentView
{
    public static readonly BindableProperty KnowledgePointProperty = BindableProperty.Create(nameof(KnowledgePoint), typeof(string), typeof(WrongQuestionCard), string.Empty);
    public static readonly BindableProperty ErrorReasonProperty = BindableProperty.Create(nameof(ErrorReason), typeof(string), typeof(WrongQuestionCard), string.Empty);
    public static readonly BindableProperty MasteryStatusProperty = BindableProperty.Create(nameof(MasteryStatus), typeof(string), typeof(WrongQuestionCard), string.Empty);
    public static readonly BindableProperty ReviewTimeProperty = BindableProperty.Create(nameof(ReviewTime), typeof(string), typeof(WrongQuestionCard), string.Empty);
    public static readonly BindableProperty RedoCommandProperty = BindableProperty.Create(nameof(RedoCommand), typeof(ICommand), typeof(WrongQuestionCard));
    public static readonly BindableProperty ExplainCommandProperty = BindableProperty.Create(nameof(ExplainCommand), typeof(ICommand), typeof(WrongQuestionCard));
    public static readonly BindableProperty PracticeCommandProperty = BindableProperty.Create(nameof(PracticeCommand), typeof(ICommand), typeof(WrongQuestionCard));

    public WrongQuestionCard()
    {
        InitializeComponent();
    }

    public string KnowledgePoint { get => (string)GetValue(KnowledgePointProperty); set => SetValue(KnowledgePointProperty, value); }
    public string ErrorReason { get => (string)GetValue(ErrorReasonProperty); set => SetValue(ErrorReasonProperty, value); }
    public string MasteryStatus { get => (string)GetValue(MasteryStatusProperty); set => SetValue(MasteryStatusProperty, value); }
    public string ReviewTime { get => (string)GetValue(ReviewTimeProperty); set => SetValue(ReviewTimeProperty, value); }
    public ICommand? RedoCommand { get => (ICommand?)GetValue(RedoCommandProperty); set => SetValue(RedoCommandProperty, value); }
    public ICommand? ExplainCommand { get => (ICommand?)GetValue(ExplainCommandProperty); set => SetValue(ExplainCommandProperty, value); }
    public ICommand? PracticeCommand { get => (ICommand?)GetValue(PracticeCommandProperty); set => SetValue(PracticeCommandProperty, value); }
}
