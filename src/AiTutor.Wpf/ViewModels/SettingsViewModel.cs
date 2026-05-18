using System.Collections.ObjectModel;
using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Services;

namespace AiTutor.Wpf.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly IAppSettingsService _settings;
    private string _apiBaseUrl = string.Empty;
    private string _currentGrade = string.Empty;
    private string _currentSubject = string.Empty;
    private string _modelNamesText = string.Empty;
    private string _photoQuestionDefaultModel = string.Empty;
    private string _homeworkCheckDefaultModel = string.Empty;
    private string _statusText = string.Empty;

    public SettingsViewModel(IAppSettingsService settings)
    {
        _settings = settings;
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        LoadFromSettings();
    }

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => SetProperty(ref _apiBaseUrl, value);
    }

    public string CurrentGrade
    {
        get => _currentGrade;
        set => SetProperty(ref _currentGrade, value);
    }

    public string CurrentSubject
    {
        get => _currentSubject;
        set => SetProperty(ref _currentSubject, value);
    }

    public string ModelNamesText
    {
        get => _modelNamesText;
        set => SetProperty(ref _modelNamesText, value);
    }

    public string PhotoQuestionDefaultModel
    {
        get => _photoQuestionDefaultModel;
        set => SetProperty(ref _photoQuestionDefaultModel, value);
    }

    public string HomeworkCheckDefaultModel
    {
        get => _homeworkCheckDefaultModel;
        set => SetProperty(ref _homeworkCheckDefaultModel, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public AsyncRelayCommand SaveCommand { get; }

    private void LoadFromSettings()
    {
        var current = _settings.Current;
        ApiBaseUrl = current.Api.BaseUrl;
        CurrentGrade = current.CurrentGrade;
        CurrentSubject = current.CurrentSubject;
        ModelNamesText = string.Join(Environment.NewLine, current.Models.VisionModelNames);
        PhotoQuestionDefaultModel = current.Models.PhotoQuestionDefaultModel;
        HomeworkCheckDefaultModel = current.Models.HomeworkCheckDefaultModel;
    }

    private async Task SaveAsync()
    {
        var modelNames = ModelNamesText
            .Split([Environment.NewLine, ",", ";"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (modelNames.Count == 0)
        {
            modelNames.Add("glm-4.5v");
        }

        await _settings.SaveAsync(new WpfAppSettings
        {
            Api = new ApiClientOptions { BaseUrl = ApiBaseUrl, TimeoutSeconds = _settings.Current.Api.TimeoutSeconds },
            CurrentGrade = string.IsNullOrWhiteSpace(CurrentGrade) ? "三年级" : CurrentGrade.Trim(),
            CurrentSubject = string.IsNullOrWhiteSpace(CurrentSubject) ? "数学" : CurrentSubject.Trim(),
            Models = new ModelSelectionOptions
            {
                VisionModelNames = modelNames,
                PhotoQuestionDefaultModel = string.IsNullOrWhiteSpace(PhotoQuestionDefaultModel) ? modelNames[0] : PhotoQuestionDefaultModel.Trim(),
                HomeworkCheckDefaultModel = string.IsNullOrWhiteSpace(HomeworkCheckDefaultModel) ? modelNames[0] : HomeworkCheckDefaultModel.Trim()
            }
        });

        StatusText = $"已保存设置：{DateTime.Now:HH:mm:ss}";
    }
}
