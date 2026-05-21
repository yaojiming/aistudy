using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Services;

namespace AiTutor.Maui.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IAppSettingsService _settingsService;
    private string? _selectedGrade;
    private string? _selectedSubject;
    private string _apiBaseUrl = string.Empty;
    private string _visionModelNamesText = string.Empty;
    private string? _selectedPhotoQuestionDefaultModel;
    private string? _selectedHomeworkCheckDefaultModel;
    private string? _statusMessage;

    public SettingsViewModel(IAppSettingsService settingsService)
    {
        _settingsService = settingsService;
        GradeOptions = new ObservableCollection<string>(["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"]);
        SaveCommand = new AsyncCommand(SaveAsync);
        BackCommand = new AsyncCommand(() => Shell.Current.GoToAsync("..", false));
        _ = LoadAsync();
    }

    public ObservableCollection<string> GradeOptions { get; }
    public ObservableCollection<string> VisionModelOptions { get; } = [];

    public string? SelectedGrade
    {
        get => _selectedGrade;
        set => SetProperty(ref _selectedGrade, value);
    }

    public string? SelectedSubject
    {
        get => _selectedSubject;
        set => SetProperty(ref _selectedSubject, value);
    }

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => SetProperty(ref _apiBaseUrl, value);
    }

    public string VisionModelNamesText
    {
        get => _visionModelNamesText;
        set
        {
            if (SetProperty(ref _visionModelNamesText, value))
            {
                RefreshVisionModelOptions();
            }
        }
    }

    public string? SelectedPhotoQuestionDefaultModel
    {
        get => _selectedPhotoQuestionDefaultModel;
        set => SetProperty(ref _selectedPhotoQuestionDefaultModel, value);
    }

    public string? SelectedHomeworkCheckDefaultModel
    {
        get => _selectedHomeworkCheckDefaultModel;
        set => SetProperty(ref _selectedHomeworkCheckDefaultModel, value);
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }

    /// <summary>
    /// 打开设置页时读取当前年级和后端地址。
    /// </summary>
    private async Task LoadAsync()
    {
        var options = await _settingsService.GetApiOptionsAsync();
        var modelOptions = await _settingsService.GetModelSelectionOptionsAsync();
        SelectedGrade = _settingsService.GetCurrentGrade();
        SelectedSubject = _settingsService.GetCurrentSubject();
        ApiBaseUrl = options.BaseUrl;
        VisionModelNamesText = string.Join(Environment.NewLine, modelOptions.VisionModelNames);
        SelectedPhotoQuestionDefaultModel = modelOptions.PhotoQuestionDefaultModel;
        SelectedHomeworkCheckDefaultModel = modelOptions.HomeworkCheckDefaultModel;
    }

    /// <summary>
    /// 保存年级和后端 HTTP 地址到本地 Preferences。
    /// </summary>
    private async Task SaveAsync()
    {
        await RunBusyAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(SelectedGrade))
            {
                throw new InvalidOperationException("请先选择当前年级。");
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                throw new InvalidOperationException("请填写后端 HTTP 地址。");
            }

            _settingsService.SaveCurrentGrade(SelectedGrade);
            _settingsService.SaveCurrentSubject(SelectedSubject ?? "数学");
            await _settingsService.SaveApiBaseUrlAsync(ApiBaseUrl);
            await _settingsService.SaveModelSelectionOptionsAsync(new ModelSelectionOptions
            {
                VisionModelNames = ParseVisionModelNames().ToList(),
                PhotoQuestionDefaultModel = SelectedPhotoQuestionDefaultModel ?? string.Empty,
                HomeworkCheckDefaultModel = SelectedHomeworkCheckDefaultModel ?? string.Empty
            });
            StatusMessage = "设置已保存，新的后端地址会在下一次请求时生效。";
        });
    }

    private void RefreshVisionModelOptions()
    {
        var currentPhoto = SelectedPhotoQuestionDefaultModel;
        var currentHomework = SelectedHomeworkCheckDefaultModel;

        VisionModelOptions.Clear();
        foreach (var model in ParseVisionModelNames())
        {
            VisionModelOptions.Add(model);
        }

        SelectedPhotoQuestionDefaultModel = VisionModelOptions.Contains(currentPhoto)
            ? currentPhoto
            : VisionModelOptions.FirstOrDefault();
        SelectedHomeworkCheckDefaultModel = VisionModelOptions.Contains(currentHomework)
            ? currentHomework
            : VisionModelOptions.FirstOrDefault();
    }

    private IEnumerable<string> ParseVisionModelNames()
    {
        return VisionModelNamesText
            .Split(['\r', '\n', ',', '，', ';', '；'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(model => !string.IsNullOrWhiteSpace(model))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
