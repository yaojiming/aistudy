using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Models;
using AiTutor.Maui.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.ViewModels;

/// <summary>
/// 浣滀笟妫€鏌ラ〉闈?ViewModel銆傞〉闈㈠彧缁戝畾鐘舵€侊紱鍥剧墖澶勭悊銆侀鐩瘑鍒€佹鏌ユ祦绋嬪垎鍒氦缁欐湇鍔″畬鎴愩€?
/// </summary>
public sealed class HomeworkCheckViewModel : ViewModelBase
{
    private readonly ITabletMediaPickerService _mediaPickerService;
    private readonly IAppSettingsService _settingsService;
    private readonly IImageOrientationService _orientationService;
    private readonly IImageCropService _imageCropService;
    private readonly IHomeworkCheckWorkflowService _checkWorkflowService;
    private readonly IWrongBookService _wrongBookService;
    private readonly ILogger<HomeworkCheckViewModel> _logger;

    private string? _selectedGrade;
    private string? _selectedSubject = "数学";
    private string? _selectedModelName;
    private bool _isThinkingModeEnabled;
    private string? _originalImagePath;
    private string? _correctedImagePath;
    private byte[]? _cachedImageBytes;
    private ImageSource? _previewImage;
    private HomeworkQuestionItemViewModel? _selectedQuestion;
    private string? _selectedQuestionRegionId;
    public string ClientStageText { get => _clientStageText; set => SetProperty(ref _clientStageText, value); }
    public string AiStatusText { get => _aiStatusText; set => SetProperty(ref _aiStatusText, value); }
    public string LatestStatusMessage { get => _latestStatusMessage; set => SetProperty(ref _latestStatusMessage, value); }
    private int _returnedResultCount;
    private float _imagePixelWidth;
    private float _imagePixelHeight;
    private bool _isBlockingLoading;
    private bool _isChecking;
    private bool _hasCheckResult;
    private CancellationTokenSource? _checkCancellationTokenSource;

    public HomeworkCheckViewModel(
        ITabletMediaPickerService mediaPickerService,
        IAppSettingsService settingsService,
        IImageOrientationService orientationService,
        IImageCropService imageCropService,
        IHomeworkCheckWorkflowService checkWorkflowService,
        IWrongBookService wrongBookService,
        ILogger<HomeworkCheckViewModel> logger)
    {
        _mediaPickerService = mediaPickerService;
        _settingsService = settingsService;
        _orientationService = orientationService;
        _imageCropService = imageCropService;
        _checkWorkflowService = checkWorkflowService;
        _wrongBookService = wrongBookService;
        _logger = logger;
        _selectedGrade = _settingsService.GetCurrentGrade();

        PickImageCommand = new AsyncCommand(PickImageAsync);
        CapturePhotoCommand = new AsyncCommand(CapturePhotoAsync);
        StartCheckCommand = new AsyncCommand(StartCheckAsync);
        ReDetectCommand = new AsyncCommand(ReDetectAsync);
        AddWrongCommand = new AsyncCommand<HomeworkQuestionItemViewModel>(AddWrongAsync);
        PracticeCommand = new AsyncCommand<HomeworkQuestionItemViewModel>(_ => Shell.Current.DisplayAlert("生成类似题", "同类题生成入口已预留。", "知道了"));
        BackCommand = new AsyncCommand(() => Shell.Current.GoToAsync("//home", false));
        BackToResultListCommand = new AsyncCommand(() =>
        {
            ClearSelection();
            return Task.CompletedTask;
        });
        RenumberSelectedQuestionCommand = new AsyncCommand(RenumberSelectedQuestionAsync);
        NudgeSelectedRegionCommand = new AsyncCommand<string>(NudgeSelectedRegionAsync);
        SelectQuestionRegionCommand = new AsyncCommand<string>(SelectQuestionAsync);
        SelectQuestionCommand = new AsyncCommand<HomeworkQuestionItemViewModel>(SelectQuestionAsync);
        _ = LoadModelOptionsAsync();
    }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedModelName { get => _selectedModelName; set => SetProperty(ref _selectedModelName, value); }
    public bool IsThinkingModeEnabled { get => _isThinkingModeEnabled; set => SetProperty(ref _isThinkingModeEnabled, value); }
    public ImageSource? PreviewImage
    {
        get => _previewImage;
        set
        {
            if (SetProperty(ref _previewImage, value))
            {
                OnPropertyChanged(nameof(HasPreview));
                RefreshComputedState();
            }
        }
    }

    private string _clientStageText = "未开始";
    private string _aiStatusText = "等待选择作业图片";
    private string _latestStatusMessage = "请选择或拍摄一张作业图片。";

    public int ReturnedResultCount
    {
        get => _returnedResultCount;
        set
        {
            if (SetProperty(ref _returnedResultCount, value))
            {
                OnPropertyChanged(nameof(ReturnedResultText));
            }
        }
    }

    public float ImagePixelWidth { get => _imagePixelWidth; set => SetProperty(ref _imagePixelWidth, value); }
    public float ImagePixelHeight { get => _imagePixelHeight; set => SetProperty(ref _imagePixelHeight, value); }
    public bool IsBlockingLoading { get => _isBlockingLoading; set => SetProperty(ref _isBlockingLoading, value); }

    public bool IsChecking
    {
        get => _isChecking;
        set
        {
            if (SetProperty(ref _isChecking, value))
            {
                RefreshComputedState();
            }
        }
    }

    public bool HasCheckResult
    {
        get => _hasCheckResult;
        set
        {
            if (SetProperty(ref _hasCheckResult, value))
            {
                RefreshComputedState();
            }
        }
    }

    public string? SelectedQuestionRegionId { get => _selectedQuestionRegionId; set => SetProperty(ref _selectedQuestionRegionId, value); }

    public HomeworkQuestionItemViewModel? SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            if (SetProperty(ref _selectedQuestion, value))
            {
                SelectedQuestionRegionId = value?.QuestionId;
                foreach (var question in Questions)
                {
                    question.IsSelected = ReferenceEquals(question, value);
                }

                RefreshComputedState();
            }
        }
    }

    public ObservableCollection<HomeworkQuestionItemViewModel> Questions { get; } = [];
    public ObservableCollection<string> ModelNames { get; } = [];
    public ObservableCollection<QuestionRegion> QuestionRegions { get; } = [];
    public bool HasPreview => PreviewImage is not null;
    public bool HasQuestions => Questions.Count > 0;
    public bool HasSelectedQuestion => SelectedQuestion is not null;
    public bool CanStartCheck => HasPreview && !IsChecking;
    public bool CanAddWrongBook => SelectedQuestion?.Result?.CanAddToWrongBook == true && !IsChecking;
    public bool ShowNotStartedPanel => !IsChecking && !HasCheckResult && SelectedQuestion is null;
    public bool ShowCheckingPanel => IsChecking;
    public bool ShowResultOverview => !IsChecking && HasCheckResult && SelectedQuestion is null;
    public bool ShowQuestionDetail => !IsChecking && SelectedQuestion is not null;
    public string StartCheckButtonText => IsChecking ? "检查中..." : "重新检查";
    public string ReturnedResultText => $"已返回结果 {ReturnedResultCount} / {Questions.Count}";

    public ICommand PickImageCommand { get; }
    public ICommand CapturePhotoCommand { get; }
    public ICommand StartCheckCommand { get; }
    public ICommand ReDetectCommand { get; }
    public ICommand AddWrongCommand { get; }
    public ICommand PracticeCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand BackToResultListCommand { get; }
    public ICommand RenumberSelectedQuestionCommand { get; }
    public ICommand NudgeSelectedRegionCommand { get; }
    public ICommand SelectQuestionRegionCommand { get; }
    public ICommand SelectQuestionCommand { get; }

    private async Task LoadModelOptionsAsync()
    {
        try
        {
            var options = await _settingsService.GetModelSelectionOptionsAsync();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ModelNames.Clear();
                foreach (var model in options.VisionModelNames)
                {
                    ModelNames.Add(model);
                }

                SelectedModelName = options.HomeworkCheckDefaultModel;
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Load homework check model options failed.");
        }
    }

    private async Task PickImageAsync()
    {
        await ProcessSelectedFileAsync(await _mediaPickerService.PickImageAsync());
    }

    private async Task CapturePhotoAsync()
    {
        await ProcessSelectedFileAsync(await _mediaPickerService.CapturePhotoAsync());
    }

    /// <summary>
    /// 閫夋嫨鍥剧墖鍚庡彧鍋氬鎴风鍙‘瀹氱殑澶勭悊锛氬鍒躲€佹柟鍚戜慨姝ｅ拰棰勮锛涢殢鍚庣洿鎺ユ妸鏁村浘浜ょ粰鍚庣 AI 涓€娆℃€ф鏌ャ€?
    /// </summary>
    private async Task ProcessSelectedFileAsync(FileResult? file)
    {
        if (file is null)
        {
            return;
        }

        try
        {
            ResetPageState();
            IsBlockingLoading = true;
            ClientStageText = "客户端处理图片";
            AiStatusText = "未提交";
            LatestStatusMessage = "请先选择作业图片。";

            _originalImagePath = await CopyFileToCacheAsync(file);
            var corrected = await _orientationService.CorrectOrientationAsync(_originalImagePath);
            _correctedImagePath = corrected.CorrectedImagePath;

            await LoadPreviewAsync(_correctedImagePath);
            IsBlockingLoading = false;
            await StartCheckAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "作业图片处理失败。");
            ErrorMessage = "图片处理失败，请重新选择一张清晰的作业图片。";
            LatestStatusMessage = ErrorMessage;
        }
        finally
        {
            IsBlockingLoading = false;
            RefreshComputedState();
        }
    }

    private async Task ReDetectAsync()
    {
        if (string.IsNullOrWhiteSpace(_correctedImagePath))
        {
            LatestStatusMessage = "正在读取作业图片...";
            return;
        }

        try
        {
            IsBlockingLoading = true;
            ClearQuestionState();
            await StartCheckAsync();
        }
        finally
        {
            IsBlockingLoading = false;
            RefreshComputedState();
        }
    }

    /// <summary>
    /// 寮€濮嬫鏌ユ椂鏁村浘涓€娆℃€ф彁浜ょ粰 AI锛涢鐩鍜屾鏌ョ粨鏋滈兘鏉ヨ嚜 AI 杩斿洖鐨勬渶缁?JSON銆?
    /// </summary>
    private async Task StartCheckAsync()
    {
        if (IsChecking || string.IsNullOrWhiteSpace(_correctedImagePath))
        {
            return;
        }

        _checkCancellationTokenSource?.Cancel();
        _checkCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _checkCancellationTokenSource.Token;

        try
        {
            ClearQuestionState();
            IsChecking = true;
            HasCheckResult = false;
            ReturnedResultCount = 0;
            ClientStageText = "整图已准备，等待 AI 返回题目区域和检查结果";
            AiStatusText = "正在提交 AI 检查";
            LatestStatusMessage = "正在提交 AI 检查";

            var results = await _checkWorkflowService.CheckAsync(_correctedImagePath, [], SelectedModelName, IsThinkingModeEnabled, _cachedImageBytes, cancellationToken);
            await MainThread.InvokeOnMainThreadAsync(() => ApplyCheckResults(results));
        }
        catch (OperationCanceledException)
        {
            LatestStatusMessage = "检查已取消。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "作业检查流程失败。");
            ErrorMessage = $"作业检查失败：{ex.Message}";
            LatestStatusMessage = ErrorMessage;
        }
        finally
        {
            IsChecking = false;
            _checkCancellationTokenSource?.Dispose();
            _checkCancellationTokenSource = null;
            RebuildQuestionRegions();
            RefreshComputedState();
        }
    }

    private void ApplyCheckResults(IReadOnlyList<HomeworkQuestionCheckResult> results)
    {
        Questions.Clear();

        foreach (var result in results)
        {
            _logger.LogInformation(
                "Homework check VM applying result. QuestionNo={QuestionNo}, QuestionId={QuestionId}, IsCorrect={IsCorrect}",
                result.QuestionNo,
                result.QuestionId,
                result.IsCorrect);

            var question = new HomeworkQuestionItemViewModel(new HomeworkQuestionRegion(
                result.QuestionId,
                result.QuestionNo,
                result.BBox,
                result.QuestionText,
                result.StudentAnswer,
                0.9));
            question.ApplyResult(result);
            Questions.Add(question);
        }

        ReturnedResultCount = Questions.Count(x => x.Result is not null);
        AiStatusText = "AI 已返回整页检查结果";
        LatestStatusMessage = ReturnedResultCount > 0
            ? $"整页作业检查完成，共返回 {ReturnedResultCount} 道题。"
            : "AI 已返回内容，但没有可用的逐题检查结果。";
        HasCheckResult = true;
        ClearSelection();
        RebuildQuestionRegions();
        RefreshComputedState();

        _logger.LogInformation(
            "Homework check VM applied all results. QuestionCount={QuestionCount}, ReturnedResultCount={ReturnedResultCount}",
            Questions.Count,
            ReturnedResultCount);
    }



    private Task SelectQuestionAsync(string? questionId)
    {
        if (!string.IsNullOrWhiteSpace(questionId))
        {
            SelectedQuestion = Questions.FirstOrDefault(x => x.QuestionId == questionId);
        }

        return Task.CompletedTask;
    }

    private Task SelectQuestionAsync(HomeworkQuestionItemViewModel? question)
    {
        SelectedQuestion = question;
        return Task.CompletedTask;
    }

    private async Task AddWrongAsync(HomeworkQuestionItemViewModel? question)
    {
        var target = question ?? SelectedQuestion;
        if (target?.Result is null || target.Result.IsCorrect != false || string.IsNullOrWhiteSpace(_correctedImagePath))
        {
            LatestStatusMessage = "请先选择一道错误题。";
            return;
        }

        await _wrongBookService.AddWrongQuestionAsync(target.Result, _correctedImagePath, target.ToRegion());
        LatestStatusMessage = $"第 {target.QuestionNo} 题已加入错题本。";
        await Shell.Current.DisplayAlert("错题本", "已加入错题本。", "知道了");
    }

    private async Task RenumberSelectedQuestionAsync()
    {
        if (SelectedQuestion is null)
        {
            LatestStatusMessage = "请先选择一道题。";
            return;
        }

        var value = await Shell.Current.DisplayPromptAsync("重新编号", "请输入新的题号：", "确定", "取消", SelectedQuestion.QuestionNo);
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        SelectedQuestion.UpdateQuestionNo(value.Trim());
        RebuildQuestionRegions();
        LatestStatusMessage = $"已将题目框编号改为第 {SelectedQuestion.QuestionNo} 题。";
    }

    private Task NudgeSelectedRegionAsync(string? direction)
    {
        if (SelectedQuestion is null)
        {
            LatestStatusMessage = "正在读取作业图片...";
            return Task.CompletedTask;
        }

        var stepX = Math.Max(4, ImagePixelWidth * 0.01f);
        var stepY = Math.Max(4, ImagePixelHeight * 0.01f);
        var offset = direction switch
        {
            "left" => new PointF(-stepX, 0),
            "right" => new PointF(stepX, 0),
            "up" => new PointF(0, -stepY),
            "down" => new PointF(0, stepY),
            _ => new PointF(0, 0)
        };

        SelectedQuestion.Nudge(offset, ImagePixelWidth, ImagePixelHeight);
        RebuildQuestionRegions();
        LatestStatusMessage = $"已微调第 {SelectedQuestion.QuestionNo} 题题目框。";
        return Task.CompletedTask;
    }

    private async Task LoadPreviewAsync(string imagePath)
    {
        var size = await _imageCropService.GetImageSizeAsync(imagePath);
        ImagePixelWidth = size.Width;
        ImagePixelHeight = size.Height;

        _cachedImageBytes = await File.ReadAllBytesAsync(imagePath);
        PreviewImage = ImageSource.FromStream(() => new MemoryStream(_cachedImageBytes));
    }

    private void ResetPageState()
    {
        ErrorMessage = null;
        _originalImagePath = null;
        _correctedImagePath = null;
        _cachedImageBytes = null;
        PreviewImage = null;
        ImagePixelWidth = 0;
        ImagePixelHeight = 0;
        ClientStageText = "客户端处理图片";
        AiStatusText = "未提交";
        LatestStatusMessage = "正在处理图片...";
        ReturnedResultCount = 0;
        IsChecking = false;
        HasCheckResult = false;
        ClearQuestionState();
    }

    private void ClearQuestionState()
    {
        Questions.Clear();
        QuestionRegions.Clear();
        ClearSelection();
        OnPropertyChanged(nameof(QuestionRegions));
    }

    private void ClearSelection()
    {
        _selectedQuestion = null;
        OnPropertyChanged(nameof(SelectedQuestion));
        SelectedQuestionRegionId = null;
        foreach (var question in Questions)
        {
            question.IsSelected = false;
        }

        RefreshComputedState();
    }

    private void RebuildQuestionRegions()
    {
        QuestionRegions.Clear();
        foreach (var question in Questions)
        {
            QuestionRegions.Add(new QuestionRegion(
                question.QuestionId,
                $"第{question.QuestionNo}题",
                question.BBox,
                question.QuestionText,
                question.Status,
                question.Result?.IsCorrect));
        }

        OnPropertyChanged(nameof(QuestionRegions));
    }

    private void RefreshComputedState()
    {
        OnPropertyChanged(nameof(HasPreview));
        OnPropertyChanged(nameof(HasQuestions));
        OnPropertyChanged(nameof(HasSelectedQuestion));
        OnPropertyChanged(nameof(CanStartCheck));
        OnPropertyChanged(nameof(CanAddWrongBook));
        OnPropertyChanged(nameof(ShowNotStartedPanel));
        OnPropertyChanged(nameof(ShowCheckingPanel));
        OnPropertyChanged(nameof(ShowResultOverview));
        OnPropertyChanged(nameof(ShowQuestionDetail));
        OnPropertyChanged(nameof(StartCheckButtonText));
        OnPropertyChanged(nameof(ReturnedResultText));
    }

    private static async Task<string> CopyFileToCacheAsync(FileResult file)
    {
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var destination = Path.Combine(FileSystem.CacheDirectory, $"aitutor-homework-{Guid.NewGuid():N}{extension}");
        await using var source = await file.OpenReadAsync();
        await using var target = File.Create(destination);
        await source.CopyToAsync(target);
        return destination;
    }
}

/// <summary>
/// 浣滀笟妫€鏌ュ彸渚ч鐩崱鐗?ViewModel锛岃礋璐ｅ崟棰樼姸鎬併€佺揣鍑戞憳瑕佸拰璇︽儏灞曠ず銆?
/// </summary>
public sealed class HomeworkQuestionItemViewModel : ViewModelBase
{
    private HomeworkQuestionStatus _status = HomeworkQuestionStatus.Pending;
    private HomeworkQuestionCheckResult? _result;
    private bool _isSelected;
    private string _questionNo;
    private RectF _bbox;

    public HomeworkQuestionItemViewModel(HomeworkQuestionRegion region)
    {
        Region = region;
        _questionNo = region.QuestionNo;
        _bbox = region.BBox;
    }

    public HomeworkQuestionRegion Region { get; }
    public string QuestionId => Region.QuestionId;
    public string QuestionNo => _questionNo;
    public RectF BBox => _bbox;
    public string QuestionText => Region.QuestionText;
    public string? StudentAnswer => Region.StudentAnswer;

    public HomeworkQuestionStatus Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                NotifyDisplayProperties();
            }
        }
    }

    public HomeworkQuestionCheckResult? Result
    {
        get => _result;
        private set
        {
            if (SetProperty(ref _result, value))
            {
                NotifyDisplayProperties();
            }
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                NotifyDisplayProperties();
            }
        }
    }

    public string StatusIcon => Status switch
    {
        HomeworkQuestionStatus.Correct => "✓",
        HomeworkQuestionStatus.Wrong => "×",
        HomeworkQuestionStatus.Uncertain => "?",
        _ => "•"
    };

    public string StatusText => Status switch
    {
        HomeworkQuestionStatus.Correct => "正确",
        HomeworkQuestionStatus.Wrong => "错误",
        HomeworkQuestionStatus.Uncertain => "不确定",
        _ => "待检查"
    };

    public string ResultSummary => Result?.ShortResult ?? "等待检查结果";
    public string StudentAnswerText => Result?.StudentAnswer ?? StudentAnswer ?? "未识别";
    public string CorrectAnswerText => Result?.CorrectAnswer ?? "待返回";
    public string DetailText => Result?.StreamingCheckText ?? "这道题还没有返回检查结果。";
    public Color StatusColor => Status switch
    {
        HomeworkQuestionStatus.Correct => Color.FromArgb("#22C55E"),
        HomeworkQuestionStatus.Wrong => Color.FromArgb("#D86673"),
        HomeworkQuestionStatus.Uncertain => Color.FromArgb("#A16207"),
        _ => Color.FromArgb("#64748B")
    };

    public Color RowBackgroundColor => IsSelected ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#FFFFFF");
    public Color RowStrokeColor => IsSelected ? Color.FromArgb("#CBD5E1") : Color.FromArgb("#E5E7EB");

    public void ApplyResult(HomeworkQuestionCheckResult result)
    {
        Result = result;
        Status = result.IsCorrect == true
            ? HomeworkQuestionStatus.Correct
            : result.IsCorrect == false
                ? HomeworkQuestionStatus.Wrong
                : HomeworkQuestionStatus.Uncertain;
    }

    public void ClearResult()
    {
        Result = null;
        Status = HomeworkQuestionStatus.Pending;
        IsSelected = false;
    }

    public void UpdateQuestionNo(string questionNo)
    {
        _questionNo = questionNo;
        OnPropertyChanged(nameof(QuestionNo));
        NotifyDisplayProperties();
    }

    public void Nudge(PointF offset, float imageWidth, float imageHeight)
    {
        var left = Math.Clamp(_bbox.Left + offset.X, 0, Math.Max(0, imageWidth - _bbox.Width));
        var top = Math.Clamp(_bbox.Top + offset.Y, 0, Math.Max(0, imageHeight - _bbox.Height));
        _bbox = new RectF(left, top, _bbox.Width, _bbox.Height);
        OnPropertyChanged(nameof(BBox));
    }

    public HomeworkQuestionRegion ToRegion()
    {
        return Region with
        {
            QuestionNo = QuestionNo,
            BBox = BBox
        };
    }

    private void NotifyDisplayProperties()
    {
        OnPropertyChanged(nameof(StatusIcon));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ResultSummary));
        OnPropertyChanged(nameof(StudentAnswerText));
        OnPropertyChanged(nameof(CorrectAnswerText));
        OnPropertyChanged(nameof(DetailText));
        OnPropertyChanged(nameof(StatusColor));
        OnPropertyChanged(nameof(RowBackgroundColor));
        OnPropertyChanged(nameof(RowStrokeColor));
    }
}

