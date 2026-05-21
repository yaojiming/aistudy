using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AiTutor.Maui.Models;
using AiTutor.Maui.Services;
using AiTutor.Shared.Agent;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.ViewModels;

/// <summary>
/// 图片类学习页面的通用 ViewModel，负责选图、拍照、上传图片并流式展示 AI 讲解。
/// </summary>
public abstract class ImageAskViewModelBase : ViewModelBase
{
    private readonly IApiClientService _apiClientService;
    private readonly ITabletMediaPickerService _mediaPickerService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOcrService? _ocrService;
    private readonly IQuestionRegionBuilder? _questionRegionBuilder;
    private readonly IImageCropService? _imageCropService;
    private readonly ILogger<ImageAskViewModelBase>? _logger;
    private readonly IAppSettingsService? _settingsService;
    private readonly List<HomeworkResultItemViewModel> _allHomeworkItems = [];
    private FileResult? _selectedFile;
    private string? _selectedImagePath;
    private ImageSource? _previewImage;
    private string? _selectedGrade = "三年级";
    private string? _selectedSubject = "数学";
    private string? _selectedModelName;
    private bool _isThinkingModeEnabled;
    private string _resultText = "选择或拍摄一张清晰图片，AI老师会边看边讲。";
    private double _uploadProgress;
    private float _imagePixelWidth;
    private float _imagePixelHeight;
    private string? _selectedQuestionRegionId;
    private string _regionHintText = string.Empty;
    private bool _hasShownThinkingProgress;
    private bool _isBlockingLoading;
    private string _answerStreamText = string.Empty;
    private readonly StringBuilder _answerBuilder = new();
    private readonly object _answerBuilderLock = new();
    private bool _uiFlushScheduled;
    private bool _isStreamingAnswer;
    private CancellationTokenSource? _flushCts;

    protected ImageAskViewModelBase(
        IApiClientService apiClientService,
        ITabletMediaPickerService mediaPickerService,
        ICurrentUserService currentUserService,
        IAppSettingsService? settingsService = null,
        IOcrService? ocrService = null,
        IQuestionRegionBuilder? questionRegionBuilder = null,
        IImageCropService? imageCropService = null,
        ILogger<ImageAskViewModelBase>? logger = null)
    {
        _apiClientService = apiClientService;
        _mediaPickerService = mediaPickerService;
        _currentUserService = currentUserService;
        _settingsService = settingsService;
        _selectedGrade = _settingsService?.GetCurrentGrade() ?? _selectedGrade;
        _ocrService = ocrService;
        _questionRegionBuilder = questionRegionBuilder;
        _imageCropService = imageCropService;
        _logger = logger;

        PickImageCommand = new AsyncCommand(PickImageAsync);
        CapturePhotoCommand = new AsyncCommand(CapturePhotoAsync);
        SubmitCommand = new AsyncCommand(SubmitAsync);
        BackCommand = new AsyncCommand(() => Shell.Current.GoToAsync("//home", false));
        SelectQuestionRegionCommand = new AsyncCommand<string>(SelectQuestionRegionAsync);
        _ = LoadModelOptionsAsync();
    }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedModelName { get => _selectedModelName; set => SetProperty(ref _selectedModelName, value); }
    public bool IsThinkingModeEnabled { get => _isThinkingModeEnabled; set => SetProperty(ref _isThinkingModeEnabled, value); }
    public ImageSource? PreviewImage { get => _previewImage; set => SetProperty(ref _previewImage, value); }
    public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }
    public double UploadProgress { get => _uploadProgress; set => SetProperty(ref _uploadProgress, value); }
    public float ImagePixelWidth { get => _imagePixelWidth; set => SetProperty(ref _imagePixelWidth, value); }
    public float ImagePixelHeight { get => _imagePixelHeight; set => SetProperty(ref _imagePixelHeight, value); }
    public string? SelectedQuestionRegionId { get => _selectedQuestionRegionId; set => SetProperty(ref _selectedQuestionRegionId, value); }

    /// <summary>
    /// 只控制“准备图片/上传图片”阶段的遮罩；模型开始流式输出后会关闭遮罩，让答案区域实时可见。
    /// </summary>
    public bool IsBlockingLoading { get => _isBlockingLoading; set => SetProperty(ref _isBlockingLoading, value); }

    /// <summary>
    /// AI 回答是否处于流式输出阶段。流式阶段前端只做纯文本轻量刷新，结束后再统一渲染 Markdown/LaTeX。
    /// </summary>
    public bool IsStreamingAnswer { get => _isStreamingAnswer; set => SetProperty(ref _isStreamingAnswer, value); }

    public string RegionHintText
    {
        get => _regionHintText;
        set
        {
            if (SetProperty(ref _regionHintText, value))
            {
                OnPropertyChanged(nameof(HasRegionHint));
            }
        }
    }

    public bool HasPreview => PreviewImage is not null;
    public bool HasRegionHint => !string.IsNullOrWhiteSpace(RegionHintText);

    public ObservableCollection<HomeworkResultItemViewModel> HomeworkItems { get; } = [];
    public ObservableCollection<QuestionRegion> QuestionRegions { get; } = [];
    public ObservableCollection<string> ModelNames { get; } = [];

    public ICommand PickImageCommand { get; }
    public ICommand CapturePhotoCommand { get; }
    public ICommand SubmitCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand SelectQuestionRegionCommand { get; }
    public ICommand ExplainCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("讲解", "讲解内容已在右侧展示。", "知道了"));
    public ICommand AddWrongCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("错题本", "已加入错题本。", "知道了"));
    public ICommand PracticeCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("同类题", "同类题生成功能已预留。", "知道了"));

    protected abstract string Mode { get; }
    protected abstract string ResourceType { get; }
    protected virtual string QuestionText => "请识别图片中的题目，并给出适合小学生理解的分步骤讲解。";
    protected virtual bool EnableQuestionRegionSelection => false;

    private async Task LoadModelOptionsAsync()
    {
        if (_settingsService is null)
        {
            return;
        }

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

                SelectedModelName = options.PhotoQuestionDefaultModel;
            });
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Load photo question model options failed.");
        }
    }

    private async Task PickImageAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.PickImageAsync());
    }

    private async Task CapturePhotoAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.CapturePhotoAsync());
    }

    /// <summary>
    /// 处理用户选择或拍摄的图片，并在拍照讲题模式下自动执行本地 OCR 框选。
    /// </summary>
    private async Task SetSelectedFileAsync(FileResult? file)
    {
        if (file is null)
        {
            return;
        }

        ResetImageAnalysisState();

        _selectedFile = file;
        _selectedImagePath = await CopyFileToCacheAsync(file);
        _logger?.LogInformation("选择图片路径：{Path}", _selectedImagePath);

        await using var stream = File.OpenRead(_selectedImagePath);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        var bytes = memory.ToArray();
        PreviewImage = ImageSource.FromStream(() => new MemoryStream(bytes));
        OnPropertyChanged(nameof(HasPreview));
        ResultText = "图片已准备好，可以上传给 AI 老师。";
        _allHomeworkItems.Clear();
        HomeworkItems.Clear();

        if (EnableQuestionRegionSelection)
        {
            await ProcessSelectedImageAsync(_selectedImagePath);
        }
    }

    private Task SubmitAsync()
    {
        if (IsBusy)
        {
            return Task.CompletedTask;
        }

        return SubmitImageQuestionAsync();
    }

    /// <summary>
    /// 上传题目图片并读取 SSE 流。上传阶段显示遮罩，收到响应头后关闭遮罩并逐段追加模型输出。
    /// </summary>
    private async Task SubmitImageQuestionAsync()
    {
        try
        {
            ErrorMessage = null;
            IsBusy = true;
            IsBlockingLoading = true;

            if (_selectedFile is null)
            {
                ErrorMessage = "请先选择或拍摄图片。";
                return;
            }

            UploadProgress = 0.2;
            ResultText = "正在准备题目图片...";
            var upload = EnableQuestionRegionSelection
                ? await UploadSelectedQuestionImageAsync()
                : await _apiClientService.UploadImageAsync(_selectedFile, ResourceType, _currentUserService.UserId);

            UploadProgress = 0.65;
            ResultText = "AI老师正在读题...\n\n";
            _hasShownThinkingProgress = false;
            ResetAnswerBuffer();
            IsStreamingAnswer = false;
            _allHomeworkItems.Clear();
            HomeworkItems.Clear();
            var hasReceivedDelta = false;

            var request = new AgentRequest
            {
                UserId = _currentUserService.UserId,
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "image",
                Mode = Mode,
                EnableThinking = IsThinkingModeEnabled,
                ModelName = SelectedModelName,
                QuestionText = QuestionText,
                ImageUrl = upload.FilePath
            };

            var finalResponse = await Task.Run(() => _apiClientService.AskStreamAsync(request, async delta =>
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!hasReceivedDelta)
                    {
                        ResultText = string.Empty;
                        IsBlockingLoading = false;
                        IsStreamingAnswer = true;
                        hasReceivedDelta = true;
                    }
                });

                AppendResultDelta(delta);
            }), CancellationToken.None);

            await FlushAnswerToUiAsync();

            var finalVisibleAnswer = string.IsNullOrWhiteSpace(finalResponse?.AnswerText)
                ? null
                : ExtractVisibleAnswerText(finalResponse.AnswerText);

            _logger?.LogInformation(
                "Photo question stream completed. HasDelta={HasDelta}, FinalAnswerLength={FinalAnswerLength}, CurrentTextLength={CurrentTextLength}",
                hasReceivedDelta,
                finalResponse?.AnswerText?.Length ?? 0,
                ResultText?.Length ?? 0);

            if (!string.IsNullOrWhiteSpace(finalVisibleAnswer))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    IsStreamingAnswer = false;
                    ResultText = finalVisibleAnswer;
                });

                UploadProgress = 1;
                if (finalResponse?.HomeworkCheckResult is not null)
                {
                    LoadHomeworkItems(finalResponse.HomeworkCheckResult);
                }

                return;
            }

            if (!hasReceivedDelta && !string.IsNullOrWhiteSpace(finalVisibleAnswer))
            {
                await MainThread.InvokeOnMainThreadAsync(() => ResultText = finalVisibleAnswer);
            }
            else if (!string.IsNullOrWhiteSpace(finalVisibleAnswer) && IsOnlyProgressText(ResultText))
            {
                await MainThread.InvokeOnMainThreadAsync(() => ResultText = finalVisibleAnswer);
            }
            else if (!string.IsNullOrWhiteSpace(finalVisibleAnswer) && FindFirstAnswerMarkerIndex(finalResponse!.AnswerText) > 0)
            {
                await MainThread.InvokeOnMainThreadAsync(() => ResultText = finalVisibleAnswer);
            }
            else if (string.IsNullOrWhiteSpace(ResultText))
            {
                await MainThread.InvokeOnMainThreadAsync(() => ResultText = "AI老师已经完成分析，但没有返回可展示内容，请再试一次。");
            }

            await MainThread.InvokeOnMainThreadAsync(() => IsStreamingAnswer = false);

            UploadProgress = 1;
            if (finalResponse?.HomeworkCheckResult is not null)
            {
                LoadHomeworkItems(finalResponse.HomeworkCheckResult);
            }
        }
        catch (OperationCanceledException)
        {
            ErrorMessage = "已取消发送图片。";
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "图片问答流式调用失败。");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsStreamingAnswer = false;
            IsBlockingLoading = false;
            IsBusy = false;
        }
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

    private Task SelectQuestionRegionAsync(string? regionId)
    {
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return Task.CompletedTask;
        }

        SelectedQuestionRegionId = regionId;
        var region = QuestionRegions.FirstOrDefault(x => x.Id == regionId);
        _logger?.LogInformation("用户选中题目框：{RegionId}, Bounds={Bounds}", regionId, region?.Bounds);
        RegionHintText = region is null ? string.Empty : $"已选择：{region.Title}";
        return Task.CompletedTask;
    }

    /// <summary>
    /// 使用端侧 OCR 识别文字行，并生成可点击的题目区域框。
    /// </summary>
    private async Task ProcessSelectedImageAsync(string imagePath)
    {
        if (_ocrService is null || _questionRegionBuilder is null || _imageCropService is null)
        {
            RegionHintText = "OCR 服务未配置，无法自动识别题目区域。";
            return;
        }

        try
        {
            RegionHintText = "正在本地识别题目区域...";
            var imageSize = await _imageCropService.GetImageSizeAsync(imagePath);
            ImagePixelWidth = imageSize.Width;
            ImagePixelHeight = imageSize.Height;
            _logger?.LogInformation("原图尺寸：{Width}x{Height}", ImagePixelWidth, ImagePixelHeight);

            var lines = await _ocrService.RecognizeLinesAsync(imagePath);
            var regions = _questionRegionBuilder.Build(lines, imageSize);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                QuestionRegions.Clear();
                foreach (var region in regions)
                {
                    QuestionRegions.Add(region);
                }

                SelectedQuestionRegionId = null;
                RegionHintText = QuestionRegions.Count switch
                {
                    0 => "未自动识别到题目区域，可重新拍摄或手动发送整图。",
                    1 => "已识别到 1 道题，请点击题目框后再上传讲解。",
                    _ => $"已识别到 {QuestionRegions.Count} 道题，请点击要讲解的题目。"
                };
                OnPropertyChanged(nameof(QuestionRegions));
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "图片 OCR 或题目框生成失败。");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                QuestionRegions.Clear();
                SelectedQuestionRegionId = null;
                RegionHintText = "题目区域识别失败，可重新拍摄或手动发送整图。";
                OnPropertyChanged(nameof(QuestionRegions));
            });
        }
    }

    /// <summary>
    /// 根据用户选中的题目框裁剪原图，并只上传裁剪后的题目图片。
    /// </summary>
    private async Task<MediaUploadResultDto> UploadSelectedQuestionImageAsync()
    {
        if (_selectedImagePath is null || _imageCropService is null)
        {
            throw new InvalidOperationException("图片尚未准备好，请重新选择图片。");
        }

        RectF cropRect;
        if (QuestionRegions.Count > 0)
        {
            var selected = QuestionRegions.FirstOrDefault(x => x.Id == SelectedQuestionRegionId);
            if (selected is null)
            {
                ErrorMessage = "请先选择要讲解的题目。";
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.DisplayAlert("请选择题目", "请先点击左侧图片中的题目框，再上传讲解。", "知道了"));
                throw new InvalidOperationException("请先选择要讲解的题目。");
            }

            cropRect = selected.Bounds;
        }
        else
        {
            var shouldSendWholeImage = await Shell.Current.DisplayAlert(
                "确认发送整图",
                "未检测到题目区域，是否发送整张图片？",
                "发送整图",
                "取消");

            if (!shouldSendWholeImage)
            {
                throw new OperationCanceledException("用户取消发送整图。");
            }

            if (ImagePixelWidth <= 0 || ImagePixelHeight <= 0)
            {
                var imageSize = await _imageCropService.GetImageSizeAsync(_selectedImagePath);
                ImagePixelWidth = imageSize.Width;
                ImagePixelHeight = imageSize.Height;
            }

            cropRect = new RectF(0, 0, ImagePixelWidth, ImagePixelHeight);
        }

        _logger?.LogInformation("裁剪区域：{CropRect}", cropRect);
        var jpegBytes = await _imageCropService.CropToJpegBytesAsync(_selectedImagePath, cropRect);
        _logger?.LogInformation("实际发送给 AI 的图片大小：{Size} bytes", jpegBytes.Length);

        UploadProgress = 0.45;
        ResultText = "正在上传裁剪后的题目图片...";
        return await _apiClientService.UploadImageBytesAsync(
            jpegBytes,
            $"question-crop-{DateTime.UtcNow:yyyyMMddHHmmss}.jpg",
            ResourceType,
            _currentUserService.UserId);
    }

    private void ResetImageAnalysisState()
    {
        SelectedQuestionRegionId = null;
        ImagePixelWidth = 0;
        ImagePixelHeight = 0;
        RegionHintText = string.Empty;
        IsBlockingLoading = false;
        QuestionRegions.Clear();
        OnPropertyChanged(nameof(QuestionRegions));
    }

    /// <summary>
    /// 追加模型流式输出，并用短节流合并 UI 刷新，避免 WebView 每个 delta 都重新渲染。
    /// </summary>
    private void AppendResultDelta(string delta)
    {
        if (string.IsNullOrEmpty(delta))
        {
            return;
        }

        if (delta.Trim() == "AI老师正在整理思路...")
        {
            if (_hasShownThinkingProgress)
            {
                return;
            }

            _hasShownThinkingProgress = true;
        }

        lock (_answerBuilderLock)
        {
            _answerBuilder.Append(delta);
        }

        ScheduleAnswerUiFlush();
    }

    /// <summary>
    /// 清空本轮回答缓存，确保新一次讲题不会混入上一次的模型输出。
    /// </summary>
    private void ResetAnswerBuffer()
    {
        CancelPendingFlush();

        lock (_answerBuilderLock)
        {
            _answerBuilder.Clear();
        }

        _answerStreamText = string.Empty;
        _uiFlushScheduled = false;
    }

    private void CancelPendingFlush()
    {
        _flushCts?.Cancel();
        _flushCts?.Dispose();
        _flushCts = null;
    }

    /// <summary>
    /// 按 40ms 合并多次 delta，再把可见文本推到 UI，降低 Android WebView 重排频率。
    /// </summary>
    private void ScheduleAnswerUiFlush()
    {
        lock (_answerBuilderLock)
        {
            if (_uiFlushScheduled)
            {
                return;
            }

            _uiFlushScheduled = true;
        }

        CancelPendingFlush();
        _flushCts = new CancellationTokenSource();
        var token = _flushCts.Token;

        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await Task.Delay(40, token);
                FlushAnswerToUi();
            }
            catch (OperationCanceledException)
            {
                // Flush was cancelled — ViewModel is no longer active.
            }
        });
    }

    /// <summary>
    /// 立即把当前缓存刷新到 UI，用于流式结束前补齐最后一小段未刷新的文本。
    /// </summary>
    private Task FlushAnswerToUiAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(FlushAnswerToUi);
    }

    private void FlushAnswerToUi()
    {
        lock (_answerBuilderLock)
        {
            _answerStreamText = _answerBuilder.ToString();
            _uiFlushScheduled = false;
        }

        ResultText = ExtractVisibleAnswerText(_answerStreamText);
    }

    /// <summary>
    /// 从完整流式文本中提取前端应显示的内容。检测到正式答案标记后，会丢弃此前的思考过程。
    /// </summary>
    private static string ExtractVisibleAnswerText(string streamText)
    {
        if (string.IsNullOrWhiteSpace(streamText))
        {
            return string.Empty;
        }

        const string reasoningMarker = "\u3010\u601d\u8def\u5206\u6790\u3011";
        const string answerMarker = "\u3010\u6b63\u5f0f\u8bb2\u89e3\u3011";
        var reasoningStart = streamText.IndexOf(reasoningMarker, StringComparison.Ordinal);
        var formalAnswerStart = streamText.IndexOf(answerMarker, StringComparison.Ordinal);
        if (reasoningStart >= 0 && formalAnswerStart > reasoningStart)
        {
            var thinkingText = streamText[(reasoningStart + reasoningMarker.Length)..formalAnswerStart].Trim();
            var answerText = streamText[formalAnswerStart..].TrimStart();
            if (!string.IsNullOrWhiteSpace(thinkingText))
            {
                return $":::thinking\n{thinkingText}\n:::\n\n{answerText}";
            }

            return answerText;
        }

        var answerStart = FindFirstAnswerMarkerIndex(streamText);
        if (answerStart >= 0)
        {
            var thinkingText = streamText[..answerStart].Trim();
            var answerText = streamText[answerStart..].TrimStart();
            if (string.IsNullOrWhiteSpace(thinkingText))
            {
                return answerText;
            }

            return $":::thinking\n{thinkingText}\n:::\n\n{answerText}";
        }

        return streamText;
    }

    private static int FindFirstAnswerMarkerIndex(string text)
    {
        var markers = new[]
        {
            "【识别结果】",
            "【题目识别】",
            "【考查知识点】",
            "【题目理解】",
            "【解题步骤】",
            "【最终答案】",
            "【答案】",
            "【解答】",
            "### 识别结果",
            "### 题目理解",
            "### 解题步骤",
            "## 识别结果",
            "## 题目理解",
            "## 解题步骤"
        };

        var firstIndex = -1;
        foreach (var marker in markers)
        {
            var index = text.IndexOf(marker, StringComparison.Ordinal);
            if (index >= 0 && (firstIndex < 0 || index < firstIndex))
            {
                firstIndex = index;
            }
        }

        return firstIndex;
    }

    /// <summary>
    /// 判断当前展示内容是否仍停留在进度提示，避免 final 已有答案时前端被“正在读题”卡住。
    /// </summary>
    private static bool IsOnlyProgressText(string text)
    {
        var normalized = (text ?? string.Empty)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return true;
        }

        var progressTexts = new[]
        {
            "AI老师正在读题...",
            "AI老师正在分析题目...",
            "AI老师正在整理思路..."
        };

        foreach (var progress in progressTexts)
        {
            normalized = normalized.Replace(progress.Replace(" ", string.Empty, StringComparison.Ordinal), string.Empty, StringComparison.Ordinal);
        }

        return string.IsNullOrWhiteSpace(normalized);
    }

    private static async Task<string> CopyFileToCacheAsync(FileResult file)
    {
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var destination = Path.Combine(FileSystem.CacheDirectory, $"aitutor-selected-{Guid.NewGuid():N}{extension}");
        await using var source = await file.OpenReadAsync();
        await using var target = File.Create(destination);
        await source.CopyToAsync(target);
        return destination;
    }
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
