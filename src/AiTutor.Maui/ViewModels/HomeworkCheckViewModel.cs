using System.Windows.Input;
using AiTutor.Maui.Services;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.ViewModels;

public class HomeworkCheckViewModel : ImageAskViewModelBase
{
    private bool _showWrongOnly;
    private bool _isThinkingComplete;
    private bool _showThinkingDetails;
    private string _thinkingContent = string.Empty;
    private string _resultSummaryText = string.Empty;
    private int _scrollToBottomTrigger;

    public HomeworkCheckViewModel(
        IApiClientService apiClientService,
        ITabletMediaPickerService mediaPickerService,
        IAppSettingsService settingsService,
        IOcrService ocrService,
        IQuestionRegionBuilder questionRegionBuilder,
        IImageCropService imageCropService,
        ILogger<ImageAskViewModelBase> logger)
        : base(apiClientService, mediaPickerService, settingsService, ocrService, questionRegionBuilder, imageCropService, logger)
    {
        ResultText = "拍一页作业，AI老师会逐题检查。";
        ToggleWrongOnlyCommand = new AsyncCommand(() =>
        {
            ShowWrongOnly = !ShowWrongOnly;
            RefreshHomeworkItems();
            return Task.CompletedTask;
        });
        ToggleThinkingCommand = new AsyncCommand(() =>
        {
            ShowThinkingDetails = !ShowThinkingDetails;
            return Task.CompletedTask;
        });
        SelectHomeworkItemCommand = new Command<HomeworkResultItemViewModel>(item =>
        {
            SelectedHomeworkItem = item;
        });
    }

    public bool ShowWrongOnly
    {
        get => _showWrongOnly;
        set => SetProperty(ref _showWrongOnly, value);
    }

    public bool IsThinkingComplete
    {
        get => _isThinkingComplete;
        set
        {
            if (SetProperty(ref _isThinkingComplete, value))
            {
                OnPropertyChanged(nameof(ThinkingFontSize));
                OnPropertyChanged(nameof(ThinkingTextColor));
                OnPropertyChanged(nameof(ThinkingMaxHeight));
                TriggerScrollToBottom();
            }
        }
    }

    public double ThinkingFontSize => _isThinkingComplete ? 14d : 16d;
    public Color ThinkingTextColor => _isThinkingComplete ? Color.FromArgb("#94A3B8") : Color.FromArgb("#334155");
    public double ThinkingMaxHeight => _isThinkingComplete ? 250d : double.PositiveInfinity;

    public bool ShowThinkingDetails
    {
        get => _showThinkingDetails;
        set
        {
            if (SetProperty(ref _showThinkingDetails, value))
            {
                ResultText = value ? _thinkingContent : _resultSummaryText;
                OnPropertyChanged(nameof(ThinkingToggleText));
                TriggerScrollToBottom();
            }
        }
    }

    public string ThinkingToggleText => _showThinkingDetails ? "收起思考" : "展开思考";

    public string ResultSummaryText
    {
        get => _resultSummaryText;
        set => SetProperty(ref _resultSummaryText, value);
    }

    public bool HasResults => AllHomeworkItems.Count > 0;

    public int ScrollToBottomTrigger => _scrollToBottomTrigger;

    public ICommand ToggleWrongOnlyCommand { get; }
    public ICommand ToggleThinkingCommand { get; }
    public ICommand SelectHomeworkItemCommand { get; }

    protected override string Mode => "check_homework";
    protected override string ResourceType => "homework_photo";
    protected override bool EnableQuestionRegionSelection => false; // 选图时不自动框选，检查完后再框选
    protected override bool CropBeforeUpload => false;
    protected override string QuestionText => "请逐题检查这张作业图片，说明每道题是否正确，并给出适合小学生理解的讲解。";

    protected override bool ShouldShowHomeworkItem(HomeworkResultItemViewModel item) => !ShowWrongOnly || item.IsWrong;

    protected override async Task OnHomeworkItemsLoadedAsync()
    {
        // 保存流式思考文本
        _thinkingContent = ResultText;
        SelectedHomeworkItem = null;

        // AI 检查完毕后再执行 OCR，框选错题区域
        if (SelectedImagePath is not null)
        {
            try
            {
                await ProcessSelectedImageAsync(SelectedImagePath);
            }
            catch
            {
                // OCR 失败不影响结果展示
            }

            // 只保留错题对应的 OCR 区域（按索引匹配），正确题不框选
            var allRegions = QuestionRegions.ToList();
            var wrongRegions = new List<Models.QuestionRegion>();
            var regionResults = new Dictionary<string, bool?>();

            for (var i = 0; i < allRegions.Count && i < AllHomeworkItems.Count; i++)
            {
                if (AllHomeworkItems[i].IsWrong)
                {
                    wrongRegions.Add(allRegions[i]);
                    regionResults[allRegions[i].Id] = false;
                }
            }

            QuestionRegions.Clear();
            foreach (var r in wrongRegions)
            {
                QuestionRegions.Add(r);
            }

            RegionResults = regionResults;
        }

        // 生成检查结果摘要
        _resultSummaryText = BuildResultSummary();
        ResultText = _resultSummaryText;
        ResultSummaryText = _resultSummaryText;
        _showThinkingDetails = false;
        OnPropertyChanged(nameof(ThinkingToggleText));

        IsThinkingComplete = true;
    }

    protected override void OnRegionSelected(string regionId)
    {
        var regions = QuestionRegions.ToList();
        // 因为 QuestionRegions 只保留了错题区域，需要映射回 AllHomeworkItems
        var allRegions = QuestionRegions.ToList();
        for (var i = 0; i < allRegions.Count; i++)
        {
            if (string.Equals(allRegions[i].Id, regionId, StringComparison.Ordinal))
            {
                // 在全部结果中找到对应的错题
                var wrongIndex = 0;
                for (var j = 0; j < AllHomeworkItems.Count; j++)
                {
                    if (AllHomeworkItems[j].IsWrong)
                    {
                        if (wrongIndex == i)
                        {
                            SelectedHomeworkItem = AllHomeworkItems[j];
                            TriggerScrollToBottom();
                            return;
                        }
                        wrongIndex++;
                    }
                }
            }
        }

        SelectedHomeworkItem = null;
    }

    public void NotifyContentChanged()
    {
        if (!_isThinkingComplete)
        {
            TriggerScrollToBottom();
        }
    }

    private void TriggerScrollToBottom()
    {
        _scrollToBottomTrigger++;
        OnPropertyChanged(nameof(ScrollToBottomTrigger));
    }

    private string BuildResultSummary()
    {
        var lines = new System.Text.StringBuilder();
        lines.AppendLine("**检查结果**");
        lines.AppendLine();

        for (var i = 0; i < AllHomeworkItems.Count; i++)
        {
            var item = AllHomeworkItems[i];
            var icon = item.IsWrong ? "✗" : "✓";
            var status = item.IsWrong ? "需订正" : "正确";
            lines.AppendLine($"**{item.QuestionNo}**  {icon} {status}");
            lines.AppendLine($"正确答案：{item.CorrectAnswer}");
            if (item.IsWrong)
            {
                lines.AppendLine($"你的答案：{item.StudentAnswer}  ");
                lines.AppendLine($"错因：{item.ErrorReason}");
            }
            lines.AppendLine();
        }

        var correctCount = AllHomeworkItems.Count(x => !x.IsWrong);
        var wrongCount = AllHomeworkItems.Count(x => x.IsWrong);
        lines.AppendLine($"共 {AllHomeworkItems.Count} 题，{correctCount} 题正确，{wrongCount} 题需订正。");
        return lines.ToString();
    }
}
