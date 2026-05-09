using System.Windows.Input;

namespace AiTutor.Maui.Controls;

public partial class HomeworkResultCard : ContentView
{
    public static readonly BindableProperty QuestionNoProperty = BindableProperty.Create(nameof(QuestionNo), typeof(string), typeof(HomeworkResultCard), string.Empty);
    public static readonly BindableProperty StatusTextProperty = BindableProperty.Create(nameof(StatusText), typeof(string), typeof(HomeworkResultCard), string.Empty);
    public static readonly BindableProperty QuestionTextProperty = BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(HomeworkResultCard), string.Empty);
    public static readonly BindableProperty DetailTextProperty = BindableProperty.Create(nameof(DetailText), typeof(string), typeof(HomeworkResultCard), string.Empty);
    public static readonly BindableProperty CardColorProperty = BindableProperty.Create(nameof(CardColor), typeof(Color), typeof(HomeworkResultCard), Color.FromArgb("#FFFFFF"));
    public static readonly BindableProperty ExplainCommandProperty = BindableProperty.Create(nameof(ExplainCommand), typeof(ICommand), typeof(HomeworkResultCard));
    public static readonly BindableProperty AddWrongCommandProperty = BindableProperty.Create(nameof(AddWrongCommand), typeof(ICommand), typeof(HomeworkResultCard));
    public static readonly BindableProperty PracticeCommandProperty = BindableProperty.Create(nameof(PracticeCommand), typeof(ICommand), typeof(HomeworkResultCard));

    public HomeworkResultCard()
    {
        InitializeComponent();
    }

    public string QuestionNo { get => (string)GetValue(QuestionNoProperty); set => SetValue(QuestionNoProperty, value); }
    public string StatusText { get => (string)GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }
    public string QuestionText { get => (string)GetValue(QuestionTextProperty); set => SetValue(QuestionTextProperty, value); }
    public string DetailText { get => (string)GetValue(DetailTextProperty); set => SetValue(DetailTextProperty, value); }
    public Color CardColor { get => (Color)GetValue(CardColorProperty); set => SetValue(CardColorProperty, value); }
    public ICommand? ExplainCommand { get => (ICommand?)GetValue(ExplainCommandProperty); set => SetValue(ExplainCommandProperty, value); }
    public ICommand? AddWrongCommand { get => (ICommand?)GetValue(AddWrongCommandProperty); set => SetValue(AddWrongCommandProperty, value); }
    public ICommand? PracticeCommand { get => (ICommand?)GetValue(PracticeCommandProperty); set => SetValue(PracticeCommandProperty, value); }
}
