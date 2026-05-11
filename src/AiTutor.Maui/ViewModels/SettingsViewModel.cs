using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Services;

namespace AiTutor.Maui.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IAppSettingsService _settingsService;
    private string? _selectedGrade;
    private string _apiBaseUrl = string.Empty;
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

    public string? SelectedGrade
    {
        get => _selectedGrade;
        set => SetProperty(ref _selectedGrade, value);
    }

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => SetProperty(ref _apiBaseUrl, value);
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
        SelectedGrade = _settingsService.GetCurrentGrade();
        ApiBaseUrl = options.BaseUrl;
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
            await _settingsService.SaveApiBaseUrlAsync(ApiBaseUrl);
            StatusMessage = "设置已保存，新的后端地址会在下一次请求时生效。";
        });
    }
}
