using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using AiTutor.Shared.Agent;
using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Services;

namespace AiTutor.Wpf.ViewModels;

public sealed class PhotoQuestionViewModel : ViewModelBase
{
    private const string UserId = "wpf-demo-user";
    private readonly IApiClientService _apiClient;
    private readonly IAppSettingsService _settings;
    private readonly IFileDialogService _fileDialog;
    private readonly StringBuilder _answerBuilder = new();
    private CancellationTokenSource? _explainCancellationTokenSource;
    private string _selectedGrade;
    private string _selectedSubject;
    private string _selectedModel;
    private bool _enableThinking;
    private string? _selectedImagePath;
    private BitmapImage? _previewImage;
    private string _answerText = "请选择或拍摄题目图片，AI 老师会按步骤讲解。";
    private string _statusText = "等待图片";
    private bool _isExplaining;

    public PhotoQuestionViewModel(IApiClientService apiClient, IAppSettingsService settings, IFileDialogService fileDialog)
    {
        _apiClient = apiClient;
        _settings = settings;
        _fileDialog = fileDialog;
        _selectedGrade = settings.Current.CurrentGrade;
        _selectedSubject = settings.Current.CurrentSubject;
        _selectedModel = settings.Current.Models.PhotoQuestionDefaultModel;

        ModelNames = new ObservableCollection<string>(settings.Current.Models.VisionModelNames);
        SelectImageCommand = new RelayCommand(SelectImage);
        CapturePhotoCommand = new RelayCommand(() => StatusText = "WPF 版本暂未接入相机，请先选择图片。");
        SubmitCommand = new ReentrantAsyncCommand(SubmitOrStopAsync);
        GenerateSimilarCommand = new RelayCommand(() => StatusText = "生成类似题功能待接入。");
        AddWrongBookCommand = new RelayCommand(() => StatusText = "已加入错题本（WPF 占位实现）。");

        _settings.SettingsChanged += (_, _) => ReloadModelSettings();
    }

    public ObservableCollection<string> Grades { get; } =
        ["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"];

    public ObservableCollection<string> Subjects { get; } =
        ["语文", "数学", "英语", "科学"];

    public ObservableCollection<string> ModelNames { get; }

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
        set => SetProperty(ref _selectedImagePath, value);
    }

    public BitmapImage? PreviewImage
    {
        get => _previewImage;
        set => SetProperty(ref _previewImage, value);
    }

    public string AnswerText
    {
        get => _answerText;
        set => SetProperty(ref _answerText, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public bool IsExplaining
    {
        get => _isExplaining;
        private set
        {
            if (SetProperty(ref _isExplaining, value))
            {
                OnPropertyChanged(nameof(SubmitButtonText));
            }
        }
    }

    public string SubmitButtonText => IsExplaining ? "停止讲解" : "上传并讲解";

    public RelayCommand SelectImageCommand { get; }

    public RelayCommand CapturePhotoCommand { get; }

    public ReentrantAsyncCommand SubmitCommand { get; }

    public RelayCommand GenerateSimilarCommand { get; }

    public RelayCommand AddWrongBookCommand { get; }

    private void SelectImage()
    {
        var path = _fileDialog.PickImageFile();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        SelectedImagePath = path;
        PreviewImage = LoadBitmap(path);
        AnswerText = "图片已选择，点击“上传并讲解”开始。";
        StatusText = Path.GetFileName(path);
    }

    private Task SubmitOrStopAsync()
    {
        if (IsExplaining)
        {
            _explainCancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        return SubmitAsync();
    }

    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedImagePath))
        {
            StatusText = "请先选择一张题目图片。";
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();
        _explainCancellationTokenSource = cancellationTokenSource;
        IsExplaining = true;
        StatusText = "正在上传图片...";
        _answerBuilder.Clear();
        AnswerText = "AI 老师正在读题...";

        try
        {
            var media = await _apiClient.UploadImageAsync(SelectedImagePath, "question_photo", UserId, cancellationTokenSource.Token);
            StatusText = "AI 老师正在讲解...";
            var request = new AgentRequest
            {
                UserId = UserId,
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "image",
                Mode = "explain",
                ImageUrl = media.FilePath,
                ModelName = SelectedModel,
                EnableThinking = EnableThinking,
                QuestionText = "请只讲解图片中的这道题，面向小学生，按步骤讲解，不要只给答案。"
            };

            var hasDelta = false;
            var finalResponse = await _apiClient.AskStreamAsync(request, async delta =>
            {
                _answerBuilder.Append(delta);
                hasDelta = true;
                await Application.Current.Dispatcher.InvokeAsync(() => AnswerText = _answerBuilder.ToString());
            }, cancellationTokenSource.Token);

            if (!hasDelta && !string.IsNullOrWhiteSpace(finalResponse?.AnswerText))
            {
                AnswerText = finalResponse.AnswerText;
            }

            StatusText = "讲解完成";
        }
        catch (OperationCanceledException)
        {
            AnswerText += $"{Environment.NewLine}{Environment.NewLine}已停止讲解。";
            StatusText = "已停止";
        }
        catch (Exception ex)
        {
            AnswerText = $"讲解失败：{ex.Message}";
            StatusText = "请求失败";
        }
        finally
        {
            if (ReferenceEquals(_explainCancellationTokenSource, cancellationTokenSource))
            {
                _explainCancellationTokenSource = null;
            }

            cancellationTokenSource.Dispose();
            IsExplaining = false;
        }
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

            SelectedModel = _settings.Current.Models.PhotoQuestionDefaultModel;
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
