using System.Windows.Input;

namespace AiTutor.Maui.ViewModels;

public class WrongQuestionDetailViewModel : ViewModelBase
{
    public WrongQuestionDetailViewModel()
    {
        VoiceAskCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("语音问老师", "语音讨论能力已预留。", "知道了"));
        AvatarExplainCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("AI老师视频讲解", "数字人讲解能力已预留。", "知道了"));
        StagePracticeCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("分阶段练习", "学习计划能力已预留。", "知道了"));
        ReviewDoneCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("复习完成", "已记录本次复习。", "知道了"));
    }

    public string Subject { get; } = "数学";
    public string Grade { get; } = "三年级";
    public string QuestionText { get; } = "7 × 8 = ?";
    public string StudentAnswer { get; } = "54";
    public string CorrectAnswer { get; } = "56";
    public string ErrorReason { get; } = "把七八五十六记成了五十四。";
    public string AiExplanation { get; } = "这道题先不要急着写答案。7 × 8 表示 8 个 7 相加，也可以背口诀：七八五十六。所以答案是 56。下次遇到乘法题，可以先背口诀，再用加法反过来检查。";

    public ICommand VoiceAskCommand { get; }
    public ICommand AvatarExplainCommand { get; }
    public ICommand StagePracticeCommand { get; }
    public ICommand ReviewDoneCommand { get; }
}
