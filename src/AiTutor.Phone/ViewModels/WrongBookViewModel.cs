using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AiTutor.Maui.ViewModels;

public class WrongBookViewModel : ViewModelBase
{
    private string? _selectedSubject = "全部";
    private string? _selectedMasteryStatus = "全部";
    private DateTime _selectedDate = DateTime.Today;

    public WrongBookViewModel()
    {
        SubjectFilters = ["全部", "数学", "语文", "英语"];
        MasteryFilters = ["全部", "新错题", "复习中", "已掌握"];
        WrongQuestions =
        [
            new WrongQuestionItemViewModel("wq1", "数学", "三年级", "表内乘法", "7 × 8 = ?", "54", "56", "乘法口诀记错，七八应是五十六。", "先背口诀，再用加法检查。", "复习中", "明天 19:00"),
            new WrongQuestionItemViewModel("wq2", "语文", "四年级", "近义词", "“立刻”的近义词是什么？", "慢慢", "马上", "没有理解词语表示很快发生。", "把词语放进句子里读，判断动作快慢。", "新错题", "今天 20:00"),
            new WrongQuestionItemViewModel("wq3", "英语", "五年级", "第三人称单数", "He ____ football every day.", "play", "plays", "主语是 he 时动词要变形。", "he/she/it 后普通动词通常加 s。", "已掌握", "周五 18:30")
        ];

        OpenDetailCommand = new AsyncCommand<WrongQuestionItemViewModel>(OpenDetailAsync);
        RedoCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("重新做", "重新作答入口已预留。", "知道了"));
        ExplainCommand = new AsyncCommand(() => Shell.Current.GoToAsync("wrong-question-detail"));
        PracticeCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("同类题", "同类题生成功能已预留。", "知道了"));
    }

    public ObservableCollection<string> SubjectFilters { get; }
    public ObservableCollection<string> MasteryFilters { get; }
    public ObservableCollection<WrongQuestionItemViewModel> WrongQuestions { get; }

    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedMasteryStatus { get => _selectedMasteryStatus; set => SetProperty(ref _selectedMasteryStatus, value); }
    public DateTime SelectedDate { get => _selectedDate; set => SetProperty(ref _selectedDate, value); }

    public ICommand OpenDetailCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand ExplainCommand { get; }
    public ICommand PracticeCommand { get; }

    private Task OpenDetailAsync(WrongQuestionItemViewModel? item)
    {
        return Shell.Current.GoToAsync("wrong-question-detail");
    }
}

public record WrongQuestionItemViewModel(
    string Id,
    string Subject,
    string Grade,
    string KnowledgePoint,
    string QuestionText,
    string StudentAnswer,
    string CorrectAnswer,
    string ErrorReason,
    string Explanation,
    string MasteryStatus,
    string NextReviewTime);
