using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using AiTutor.Shared.Agent;
using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Models;
using AiTutor.Wpf.Services;

namespace AiTutor.Wpf.ViewModels;

public sealed class HomeworkCheckViewModel : ViewModelBase
{
    private const string UserId = "wpf-demo-user";
    private const double OverlayWidth = 520;
    private const double OverlayHeight = 720;
    private readonly IApiClientService _apiClient;
    private readonly IAppSettingsService _settings;
    private readonly IFileDialogService _fileDialog;
    private string _selectedGrade;
    private string _selectedSubject;
    private string _selectedModel;
    private bool _enableThinking;
    private string? _selectedImagePath;
    private BitmapImage? _previewImage;
    private string _summaryText = "请选择一张作业图片，点击“开始检查”。";
    private string _statusText = "未开始";
    private bool _isChecking;
    private HomeworkResultItem? _selectedResult;

    public HomeworkCheckViewModel(IApiClientService apiClient, IAppSettingsService settings, IFileDialogService fileDialog)
    {
        _apiClient = apiClient;
        _settings = settings;
        _fileDialog = fileDialog;
        _selectedGrade = settings.Current.CurrentGrade;
        _selectedSubject = settings.Current.CurrentSubject;
        _selectedModel = settings.Current.Models.HomeworkCheckDefaultModel;

        ModelNames = new ObservableCollection<string>(settings.Current.Models.VisionModelNames);
        SelectImageCommand = new RelayCommand(SelectImage);
        CapturePhotoCommand = new RelayCommand(() => StatusText = "WPF 版本暂未接入相机，请先选择图片。");
        StartCheckCommand = new AsyncRelayCommand(StartCheckAsync, () => !IsChecking && !string.IsNullOrWhiteSpace(SelectedImagePath));
        SelectRegionCommand = new RelayCommand(parameter =>
        {
            if (parameter is HomeworkRegionBox box && box.Result is not null)
            {
                SelectedResult = box.Result;
            }
        });
        AddWrongBookCommand = new RelayCommand(() => StatusText = "已加入错题本（WPF 占位实现）。", () => SelectedResult?.IsCorrect == false);
        GenerateSimilarCommand = new RelayCommand(() => StatusText = "生成同类题功能待接入。");

        _settings.SettingsChanged += (_, _) => ReloadModelSettings();
    }

    public ObservableCollection<string> Grades { get; } =
        ["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"];

    public ObservableCollection<string> Subjects { get; } =
        ["语文", "数学", "英语", "科学"];

    public ObservableCollection<string> ModelNames { get; }

    public ObservableCollection<HomeworkResultItem> Results { get; } = [];

    public ObservableCollection<HomeworkRegionBox> RegionBoxes { get; } = [];

    public string SelectedGrade
    {
        get => _selectedGrade;
        set => SetProperty(ref _selectedGrade, value);
    }

    public string SelectedSubject
    {
        get => _selectedSubject;
        set => SetProperty(ref _selectedSubject, value);
    }

    public string SelectedModel
    {
        get => _selectedModel;
        set => SetProperty(ref _selectedModel, value);
    }

    public bool EnableThinking
    {
        get => _enableThinking;
        set => SetProperty(ref _enableThinking, value);
    }

    public string? SelectedImagePath
    {
        get => _selectedImagePath;
        set
        {
            if (SetProperty(ref _selectedImagePath, value))
            {
                StartCheckCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public BitmapImage? PreviewImage
    {
        get => _previewImage;
        set => SetProperty(ref _previewImage, value);
    }

    public string SummaryText
    {
        get => _summaryText;
        set => SetProperty(ref _summaryText, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public bool IsChecking
    {
        get => _isChecking;
        set
        {
            if (SetProperty(ref _isChecking, value))
            {
                OnPropertyChanged(nameof(StartCheckButtonText));
                StartCheckCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StartCheckButtonText => IsChecking
        ? "检查中..."
        : Results.Count > 0 ? "重新检查" : "开始检查";

    public HomeworkResultItem? SelectedResult
    {
        get => _selectedResult;
        set
        {
            if (SetProperty(ref _selectedResult, value))
            {
                AddWrongBookCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand SelectImageCommand { get; }

    public RelayCommand CapturePhotoCommand { get; }

    public AsyncRelayCommand StartCheckCommand { get; }

    public RelayCommand SelectRegionCommand { get; }

    public RelayCommand AddWrongBookCommand { get; }

    public RelayCommand GenerateSimilarCommand { get; }

    private void SelectImage()
    {
        var path = _fileDialog.PickImageFile();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        SelectedImagePath = path;
        PreviewImage = LoadBitmap(path);
        Results.Clear();
        RegionBoxes.Clear();
        SelectedResult = null;
        SummaryText = "图片已选择。点击“开始检查”后，会把整张作业图片发给后端，由 AI 一次性返回每道题的检查结果和坐标。";
        StatusText = Path.GetFileName(path);
        OnPropertyChanged(nameof(StartCheckButtonText));
    }

    private async Task StartCheckAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedImagePath))
        {
            return;
        }

        IsChecking = true;
        Results.Clear();
        RegionBoxes.Clear();
        SelectedResult = null;
        SummaryText = "正在提交 AI 检查，请稍等。";
        StatusText = "上传作业图片...";
        OnPropertyChanged(nameof(StartCheckButtonText));

        try
        {
            var media = await _apiClient.UploadImageAsync(SelectedImagePath, "homework_photo", UserId);
            StatusText = "AI 正在检查整页作业...";
            var response = await _apiClient.AskAsync(new AgentRequest
            {
                UserId = UserId,
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "image",
                Mode = "check_homework",
                ImageUrl = media.FilePath,
                ModelName = SelectedModel,
                EnableThinking = EnableThinking,
                QuestionText = "请检查整页作业，返回每道题的结构化检查结果和 normalized_1000 坐标。"
            });

            ApplyHomeworkResult(response.HomeworkCheckResult);
            StatusText = $"检查完成：返回 {Results.Count} 道题";
        }
        catch (Exception ex)
        {
            SummaryText = $"作业检查失败：{ex.Message}";
            StatusText = "请求失败";
        }
        finally
        {
            IsChecking = false;
            OnPropertyChanged(nameof(StartCheckButtonText));
        }
    }

    private void ApplyHomeworkResult(HomeworkCheckResultDto? result)
    {
        if (result is null || result.Items.Count == 0)
        {
            SummaryText = "后端没有返回作业检查结果。";
            return;
        }

        SummaryText = string.IsNullOrWhiteSpace(result.Summary)
            ? $"共检查 {result.Items.Count} 道题。"
            : result.Summary;

        foreach (var item in result.Items)
        {
            var viewItem = HomeworkResultItem.FromDto(item);
            Results.Add(viewItem);
            if (item.BBox is not null)
            {
                RegionBoxes.Add(ToRegionBox(viewItem));
            }
        }

        SelectedResult = Results.FirstOrDefault();
        OnPropertyChanged(nameof(StartCheckButtonText));
    }

    private static HomeworkRegionBox ToRegionBox(HomeworkResultItem item)
    {
        var bbox = item.BBox!;
        var x1 = Math.Clamp(bbox.X1, 0, 1000);
        var y1 = Math.Clamp(bbox.Y1, 0, 1000);
        var x2 = Math.Clamp(bbox.X2, 0, 1000);
        var y2 = Math.Clamp(bbox.Y2, 0, 1000);

        return new HomeworkRegionBox
        {
            QuestionNo = item.QuestionNo,
            IsCorrect = item.IsCorrect,
            Result = item,
            Left = x1 / 1000d * OverlayWidth,
            Top = y1 / 1000d * OverlayHeight,
            Width = Math.Max(24, (x2 - x1) / 1000d * OverlayWidth),
            Height = Math.Max(24, (y2 - y1) / 1000d * OverlayHeight)
        };
    }

    private void ReloadModelSettings()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ModelNames.Clear();
            foreach (var model in _settings.Current.Models.VisionModelNames)
            {
                ModelNames.Add(model);
            }

            SelectedModel = _settings.Current.Models.HomeworkCheckDefaultModel;
        });
    }

    private static BitmapImage LoadBitmap(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path);
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}
