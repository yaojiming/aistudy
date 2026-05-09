using System.Windows.Input;
using AiTutor.Maui.Services;

namespace AiTutor.Maui.ViewModels;

public class HomeworkCheckViewModel : ImageAskViewModelBase
{
    private bool _showWrongOnly;

    public HomeworkCheckViewModel(IApiClientService apiClientService, ITabletMediaPickerService mediaPickerService)
        : base(apiClientService, mediaPickerService)
    {
        ResultText = "拍一页作业，AI老师会逐题检查。";
        ToggleWrongOnlyCommand = new AsyncCommand(() =>
        {
            ShowWrongOnly = !ShowWrongOnly;
            RefreshHomeworkItems();
            return Task.CompletedTask;
        });
    }

    public bool ShowWrongOnly
    {
        get => _showWrongOnly;
        set => SetProperty(ref _showWrongOnly, value);
    }

    public ICommand ToggleWrongOnlyCommand { get; }

    protected override string Mode => "check_homework";
    protected override string ResourceType => "homework_photo";
    protected override string QuestionText => "请逐题检查这张作业图片，说明每道题是否正确，并给出适合小学生理解的讲解。";

    protected override bool ShouldShowHomeworkItem(HomeworkResultItemViewModel item) => !ShowWrongOnly || item.IsWrong;
}
