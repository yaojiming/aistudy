using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Services;
using AiTutor.Shared.Agent;

namespace AiTutor.Maui.ViewModels;

public abstract class ImageAskViewModelBase : ViewModelBase
{
    private readonly IApiClientService _apiClientService;
    private readonly ITabletMediaPickerService _mediaPickerService;
    private readonly List<HomeworkResultItemViewModel> _allHomeworkItems = [];
    private FileResult? _selectedFile;
    private ImageSource? _previewImage;
    private string? _selectedGrade = "三年级";
    private string? _selectedSubject = "数学";
    private string? _selectedThinkingMode = "standard";
    private string _resultText = "选择或拍摄一张清晰图片，AI老师会边看边讲。";
    private double _uploadProgress;

    protected ImageAskViewModelBase(IApiClientService apiClientService, ITabletMediaPickerService mediaPickerService)
    {
        _apiClientService = apiClientService;
        _mediaPickerService = mediaPickerService;
        PickImageCommand = new AsyncCommand(PickImageAsync);
        CapturePhotoCommand = new AsyncCommand(CapturePhotoAsync);
        SubmitCommand = new AsyncCommand(SubmitAsync);
    }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedThinkingMode { get => _selectedThinkingMode; set => SetProperty(ref _selectedThinkingMode, value); }
    public ImageSource? PreviewImage { get => _previewImage; set => SetProperty(ref _previewImage, value); }
    public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }
    public double UploadProgress { get => _uploadProgress; set => SetProperty(ref _uploadProgress, value); }
    public bool HasPreview => PreviewImage is not null;

    public ObservableCollection<HomeworkResultItemViewModel> HomeworkItems { get; } = [];

    public ICommand PickImageCommand { get; }
    public ICommand CapturePhotoCommand { get; }
    public ICommand SubmitCommand { get; }
    public ICommand ExplainCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("讲解", "讲解内容已在右侧展示。", "知道了"));
    public ICommand AddWrongCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("错题本", "已加入错题本。", "知道了"));
    public ICommand PracticeCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("同类题", "同类题生成功能已预留。", "知道了"));

    protected abstract string Mode { get; }
    protected abstract string ResourceType { get; }
    protected virtual string QuestionText => "请识别图片中的题目，并给出适合小学生理解的分步骤讲解。";

    private async Task PickImageAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.PickImageAsync());
    }

    private async Task CapturePhotoAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.CapturePhotoAsync());
    }

    private async Task SetSelectedFileAsync(FileResult? file)
    {
        if (file is null)
        {
            return;
        }

        _selectedFile = file;
        await using var stream = await file.OpenReadAsync();
        var bytes = new byte[stream.Length];
        _ = await stream.ReadAsync(bytes);
        PreviewImage = ImageSource.FromStream(() => new MemoryStream(bytes));
        OnPropertyChanged(nameof(HasPreview));
        ResultText = "图片已准备好，可以上传给 AI 老师。";
        _allHomeworkItems.Clear();
        HomeworkItems.Clear();
    }

    private Task SubmitAsync()
    {
        return RunBusyAsync(async () =>
        {
            if (_selectedFile is null)
            {
                ErrorMessage = "请先选择或拍摄图片。";
                return;
            }

            UploadProgress = 0.2;
            ResultText = "正在上传图片...";
            var upload = await _apiClientService.UploadImageAsync(_selectedFile, ResourceType, "test-user");
            UploadProgress = 0.65;
            ResultText = string.Empty;
            _allHomeworkItems.Clear();
            HomeworkItems.Clear();

            var finalResponse = await _apiClientService.AskStreamAsync(new AgentRequest
            {
                UserId = "test-user",
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "image",
                Mode = Mode,
                ThinkingMode = SelectedThinkingMode,
                QuestionText = QuestionText,
                ImageUrl = upload.FilePath
            }, async delta =>
            {
                await MainThread.InvokeOnMainThreadAsync(() => ResultText += delta);
            });

            UploadProgress = 1;
            if (finalResponse?.HomeworkCheckResult is not null)
            {
                LoadHomeworkItems(finalResponse.HomeworkCheckResult);
            }
        });
    }

    protected void LoadHomeworkItems(HomeworkCheckResultDto result)
    {
        _allHomeworkItems.Clear();
        foreach (var item in result.Items)
        {
            _allHomeworkItems.Add(new HomeworkResultItemViewModel(item));
        }

        RefreshHomeworkItems();
    }

    protected void RefreshHomeworkItems()
    {
        HomeworkItems.Clear();
        foreach (var item in _allHomeworkItems.Where(ShouldShowHomeworkItem))
        {
            HomeworkItems.Add(item);
        }
    }

    protected virtual bool ShouldShowHomeworkItem(HomeworkResultItemViewModel item) => true;
}

public class HomeworkResultItemViewModel
{
    public HomeworkResultItemViewModel(HomeworkCheckItemDto item)
    {
        QuestionNo = item.QuestionNo;
        StatusText = item.IsCorrect == true ? "正确" : "需订正";
        QuestionText = item.QuestionText;
        DetailText = item.IsCorrect == true
            ? item.Explanation
            : $"你的答案：{item.StudentAnswer}  正确答案：{item.CorrectAnswer}\n错因：{item.ErrorReason}\n{item.Explanation}";
        IsWrong = item.IsCorrect != true;
    }

    public string QuestionNo { get; }
    public string StatusText { get; }
    public string QuestionText { get; }
    public string DetailText { get; }
    public bool IsWrong { get; }
    public Color CardColor => IsWrong ? Color.FromArgb("#FFF6E8") : Color.FromArgb("#FFFFFF");
}
