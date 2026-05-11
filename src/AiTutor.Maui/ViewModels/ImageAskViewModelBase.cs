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
    private readonly IOcrService? _ocrService;
    private readonly IQuestionRegionBuilder? _questionRegionBuilder;
    private readonly IImageCropService? _imageCropService;
    private readonly ILogger<ImageAskViewModelBase>? _logger;
    private readonly IAppSettingsService? _settingsService;
    private readonly List<HomeworkResultItemViewModel> _allHomeworkItems = [];
    protected IReadOnlyList<HomeworkResultItemViewModel> AllHomeworkItems => _allHomeworkItems;
    private FileResult? _selectedFile;
    private string? _selectedImagePath;
    private ImageSource? _previewImage;
    private string? _selectedGrade = "三年级";
    private string? _selectedSubject = "数学";
    private string? _selectedThinkingMode = "standard";
    private string _resultText = "选择或拍摄一张清晰图片，AI老师会边看边讲。";
    private double _uploadProgress;
    private float _imagePixelWidth;
    private float _imagePixelHeight;
    private string? _selectedQuestionRegionId;
    private string _regionHintText = string.Empty;
    private bool _hasShownThinkingProgress;
    private bool _isBlockingLoading;
    private string _answerStreamText = string.Empty;
    private HomeworkResultItemViewModel? _selectedHomeworkItem;
    private Dictionary<string, bool?>? _regionResults;

    protected ImageAskViewModelBase(
        IApiClientService apiClientService,
        ITabletMediaPickerService mediaPickerService,
        IAppSettingsService? settingsService = null,
        IOcrService? ocrService = null,
        IQuestionRegionBuilder? questionRegionBuilder = null,
        IImageCropService? imageCropService = null,
        ILogger<ImageAskViewModelBase>? logger = null)
    {
        _apiClientService = apiClientService;
        _mediaPickerService = mediaPickerService;
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
    }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedThinkingMode { get => _selectedThinkingMode; set => SetProperty(ref _selectedThinkingMode, value); }
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

    public HomeworkResultItemViewModel? SelectedHomeworkItem
    {
        get => _selectedHomeworkItem;
        set
        {
            if (SetProperty(ref _selectedHomeworkItem, value))
            {
                OnPropertyChanged(nameof(HasSelectedHomeworkItem));
            }
        }
    }

    public bool HasSelectedHomeworkItem => SelectedHomeworkItem is not null;

    public Dictionary<string, bool?>? RegionResults
    {
        get => _regionResults;
        set => SetProperty(ref _regionResults, value);
    }

    public ICommand PickImageCommand { get; }
    public ICommand CapturePhotoCommand { get; }
    public ICommand SubmitCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand SelectQuestionRegionCommand { get; }
    public ICommand ExplainCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("讲解", "讲解内容已在右侧展示。", "知道了"));
    public ICommand AddWrongCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("错题本", "已加入错题本。", "知道了"));
    public ICommand PracticeCommand { get; } = new AsyncCommand(() => Shell.Current.DisplayAlert("同类题", "同类题生成功能已预留。", "知道了"));

    protected string? SelectedImagePath => _selectedImagePath;

    protected abstract string Mode { get; }
    protected abstract string ResourceType { get; }
    protected virtual string QuestionText => "请识别图片中的题目，并给出适合小学生理解的分步骤讲解。";
    protected virtual bool EnableQuestionRegionSelection => false;
    protected virtual bool CropBeforeUpload => true;

    private async Task PickImageAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.PickImageAsync());
    }

    private async Task CapturePhotoAsync()
    {
        await SetSelectedFileAsync(await _mediaPickerService.CapturePhotoAsync());
    }

    /// <summary>
    /// 处理用户选择或拍摄的图片，自动检测倾斜并修正，再执行本地 OCR 框选。
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

        // 自动检测并修正倾斜（独立于 OCR 框选，只要裁剪服务可用就执行）
        if (_imageCropService is not null)
        {
            try
            {
                var imagePath = _selectedImagePath;
                var skewAngle = await Task.Run(() => _imageCropService.DetectSkewAngleAsync(imagePath));
                if (skewAngle.HasValue && MathF.Abs(skewAngle.Value) > 0.5f)
                {
                    ResultText = $"检测到图片倾斜 {skewAngle.Value:F1}°，正在自动旋转...";
                    var correctedPath = await Task.Run(() => _imageCropService.RotateImageAsync(imagePath, skewAngle.Value));
                    _selectedImagePath = correctedPath;
                    _logger?.LogInformation("图片已旋转修正，新路径：{Path}", _selectedImagePath);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "倾斜检测或旋转失败，使用原图继续。");
            }
        }

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
            var upload = EnableQuestionRegionSelection && CropBeforeUpload
                ? await UploadSelectedQuestionImageAsync()
                : await _apiClientService.UploadImageAsync(_selectedFile, ResourceType, "test-user");

            UploadProgress = 0.65;
            ResultText = "AI老师正在读题...\n\n";
            _hasShownThinkingProgress = false;
            _answerStreamText = string.Empty;
            _allHomeworkItems.Clear();
            HomeworkItems.Clear();
            var hasReceivedDelta = false;

            IsBlockingLoading = false;

            var request = new AgentRequest
            {
                UserId = "test-user",
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "image",
                Mode = Mode,
                ThinkingMode = SelectedThinkingMode,
                QuestionText = QuestionText,
                ImageUrl = upload.FilePath
            };

            var pendingDelta = new StringBuilder();
            var pendingDeltaLock = new object();
            var lastFlushTime = DateTime.UtcNow;

            var finalResponse = await Task.Run(() => _apiClientService.AskStreamAsync(request, async delta =>
            {
                string? textToFlush = null;
                lock (pendingDeltaLock)
                {
                    pendingDelta.Append(delta);
                    var shouldFlush = 
                    !hasReceivedDelta ||
                    delta.Contains('\n') ||
                    DateTime.UtcNow - lastFlushTime >= TimeSpan.FromMilliseconds(0);

                    if (shouldFlush)
                    {
                        textToFlush = pendingDelta.ToString();
                        pendingDelta.Clear();
                        lastFlushTime = DateTime.UtcNow;
                    }
                }

                if (string.IsNullOrEmpty(textToFlush))
                {
                    return;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!hasReceivedDelta)
                    {
                        ResultText = string.Empty;
                        hasReceivedDelta = true;
                    }

                    AppendResultDelta(textToFlush);
                });
            }), CancellationToken.None);

            string? remainingDelta = null;
            lock (pendingDeltaLock)
            {
                if (pendingDelta.Length > 0)
                {
                    remainingDelta = pendingDelta.ToString();
                    pendingDelta.Clear();
                }
            }

            if (!string.IsNullOrEmpty(remainingDelta))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!hasReceivedDelta)
                    {
                        ResultText = string.Empty;
                        hasReceivedDelta = true;
                    }

                    AppendResultDelta(remainingDelta);
                });
            }

            var finalVisibleAnswer = string.IsNullOrWhiteSpace(finalResponse?.AnswerText)
                ? null
                : ExtractVisibleAnswerText(finalResponse.AnswerText);

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
            IsBlockingLoading = false;
            IsBusy = false;
        }
    }

    protected async void LoadHomeworkItems(HomeworkCheckResultDto result)
    {
        _allHomeworkItems.Clear();
        foreach (var item in result.Items)
        {
            _allHomeworkItems.Add(new HomeworkResultItemViewModel(item));
        }

        RefreshHomeworkItems();
        await OnHomeworkItemsLoadedAsync();
    }

    protected virtual Task OnHomeworkItemsLoadedAsync() => Task.CompletedTask;

    protected void RefreshHomeworkItems()
    {
        HomeworkItems.Clear();
        foreach (var item in _allHomeworkItems.Where(ShouldShowHomeworkItem))
        {
            HomeworkItems.Add(item);
        }
    }

    protected virtual bool ShouldShowHomeworkItem(HomeworkResultItemViewModel item) => true;

    protected virtual Task SelectQuestionRegionAsync(string? regionId)
    {
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return Task.CompletedTask;
        }

        SelectedQuestionRegionId = regionId;
        var region = QuestionRegions.FirstOrDefault(x => x.Id == regionId);
        _logger?.LogInformation("用户选中题目框：{RegionId}, Bounds={Bounds}", regionId, region?.Bounds);
        RegionHintText = region is null ? string.Empty : $"已选择：{region.Title}";
        OnRegionSelected(regionId);
        return Task.CompletedTask;
    }

    protected virtual void OnRegionSelected(string regionId) { }

    /// <summary>
    /// 使用端侧 OCR 识别文字行，并生成可点击的题目区域框。
    /// 子类可在适当时机调用（如作业检查在 AI 返回结果后再执行 OCR）。
    /// </summary>
    protected async Task ProcessSelectedImageAsync(string imagePath)
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
            "test-user");
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
    /// 追加模型流式输出，并对重复的思考进度提示去重。
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

        _answerStreamText += delta;
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
        CorrectAnswer = item.CorrectAnswer ?? string.Empty;
        StudentAnswer = item.StudentAnswer ?? string.Empty;
        ErrorReason = item.ErrorReason ?? string.Empty;
        Explanation = item.Explanation;
        DetailText = item.IsCorrect == true
            ? item.Explanation
            : $"你的答案：{item.StudentAnswer}  正确答案：{item.CorrectAnswer}\n错因：{item.ErrorReason}\n{item.Explanation}";
        IsWrong = item.IsCorrect != true;
    }

    public string QuestionNo { get; }
    public string StatusText { get; }
    public string QuestionText { get; }
    public string CorrectAnswer { get; }
    public string StudentAnswer { get; }
    public string ErrorReason { get; }
    public string Explanation { get; }
    public string DetailText { get; }
    public bool IsWrong { get; }
    public Color CardColor => IsWrong ? Color.FromArgb("#FFF6E8") : Color.FromArgb("#FFFFFF");
}
