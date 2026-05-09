using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AiTutor.Maui.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public HomeViewModel()
    {
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
    }

    /// <summary>
    /// 顶部展示的当前年级。
    /// </summary>
    public string CurrentGrade { get; } = "三年级";

    /// <summary>
    /// 顶部展示的当前学科。
    /// </summary>
    public string CurrentSubject { get; } = "数学";

    /// <summary>
    /// 首页右侧今日任务。
    /// </summary>
    public ObservableCollection<HomeStatusItem> TodayTasks { get; }

    /// <summary>
    /// 首页右侧最近提问。
    /// </summary>
    public ObservableCollection<string> RecentQuestions { get; }

    /// <summary>
    /// 待复习错题数量。
    /// </summary>
    public int WrongQuestionCount { get; } = 6;

    /// <summary>
    /// 推荐练习说明。
    /// </summary>
    public string RecommendedPractice { get; } = "表内乘除法巩固 10 分钟";

    public ICommand OpenHomeCommand { get; } = new AsyncCommand(() => Shell.Current.GoToAsync("//home", false));
    public ICommand OpenChatCommand { get; } = new AsyncCommand(
        () => Shell.Current.GoToAsync("chat", false)
        );
    public ICommand OpenPhotoQuestionCommand { get; } = new AsyncCommand(() => Shell.Current.GoToAsync("photo-question", false));
    public ICommand OpenHomeworkCheckCommand { get; } = new AsyncCommand(() => Shell.Current.GoToAsync("homework-check", false));
    public ICommand OpenWrongBookCommand { get; } = new AsyncCommand(() => Shell.Current.GoToAsync("wrong-book", false));
    public ICommand OpenTextbookCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("教材", "教材学习入口已预留。", "知道了"));
    public ICommand OpenStudyPlanCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("学习计划", "分阶段学习计划入口已预留。", "知道了"));
    public ICommand OpenSettingsCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("设置", "后端地址在 Resources/Raw/appsettings.json 中配置。", "知道了"));
}

public record HomeStatusItem(string Title, string Description);
