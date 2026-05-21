using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Services;

namespace AiTutor.Maui.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly IAppSettingsService _settingsService;
    private string _currentGrade;

    public HomeViewModel(IAppSettingsService settingsService)
    {
        _settingsService = settingsService;
        _currentGrade = _settingsService.GetCurrentGrade();
        _settingsService.SettingsChanged += OnSettingsChanged;

        TodayTasks =
        [
            new("数学口算", "还剩 8 道"),
            new("错题复习", "2 道待复习"),
            new("英语单词", "今日已完成 60%")
        ];

        RecentQuestions =
        [
            "有余数除法怎么验算？",
            "怎样区分比喻句和拟人句？",
            "play 为什么变成 plays？"
        ];

        OpenHomeCommand = new AsyncCommand(() => Shell.Current.GoToAsync("//home", false));
        OpenChatCommand = new AsyncCommand(() => Shell.Current.GoToAsync("chat", false));
        OpenPhotoQuestionCommand = new AsyncCommand(() => Shell.Current.GoToAsync("photo-question", false));
        OpenHomeworkCheckCommand = new AsyncCommand(() => Shell.Current.GoToAsync("homework-check", false));
        OpenWrongBookCommand = new AsyncCommand(() => Shell.Current.GoToAsync("wrong-book", false));
        OpenTextbookCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("教材", "教材学习入口已预留。", "知道了"));
        OpenStudyPlanCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("学习计划", "分阶段学习计划入口已预留。", "知道了"));
        OpenSettingsCommand = new AsyncCommand(() => Shell.Current.GoToAsync("settings", false));
    }

    /// <summary>
    /// 顶部显示的当前年级，来自设置页保存的本地配置。
    /// </summary>
    public string CurrentGrade
    {
        get => _currentGrade;
        private set => SetProperty(ref _currentGrade, value);
    }

    public string CurrentSubject { get; } = "数学";
    public ObservableCollection<HomeStatusItem> TodayTasks { get; }
    public ObservableCollection<string> RecentQuestions { get; }
    public int WrongQuestionCount { get; } = 6;
    public string RecommendedPractice { get; } = "表内乘除法巩固 10 分钟";

    public ICommand OpenHomeCommand { get; }
    public ICommand OpenChatCommand { get; }
    public ICommand OpenPhotoQuestionCommand { get; }
    public ICommand OpenHomeworkCheckCommand { get; }
    public ICommand OpenWrongBookCommand { get; }
    public ICommand OpenTextbookCommand { get; }
    public ICommand OpenStudyPlanCommand { get; }
    public ICommand OpenSettingsCommand { get; }

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        CurrentGrade = _settingsService.GetCurrentGrade();
    }
}

public record HomeStatusItem(string Title, string Description);
