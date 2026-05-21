using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AiTutor.Maui.Services;
using AiTutor.Shared.Agent;

namespace AiTutor.Maui.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiClientService _apiClientService;
    private readonly IAppSettingsService _settingsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISpeechInteractionService _speechInteractionService;
    private string? _selectedGrade = "五年级";
    private string? _selectedSubject = "数学";
    private bool _isThinkingModeEnabled;
    private string _questionText = string.Empty;
    private string? _toastMessage;
    private string? _sessionId;
    private bool _isSending;
    private bool _isListening;
    private CancellationTokenSource? _sendCancellationTokenSource;
    private CancellationTokenSource? _listenCancellationTokenSource;
    private readonly List<ConversationTurn> _conversationTurns = [];

    public ChatViewModel(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        ICurrentUserService currentUserService,
        ISpeechInteractionService speechInteractionService)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _currentUserService = currentUserService;
        _speechInteractionService = speechInteractionService;
        ApplyLearningSettings();
        _settingsService.SettingsChanged += OnSettingsChanged;

        Messages =
        [
            new ChatMessageViewModel(
                "AI TUTOR",
                "你好啊，小状元！我是你的 AI 学习小助手。不管是数学难题、古诗词解析，还是英语语法，你都可以问我哦！",
                false)
        ];

        QuickQuestions = ["再讲简单点", "给我例子", "出一道类似题", "加入错题本"];
        SubmitCommand = new ReentrantAsyncCommand(SubmitOrStopAsync);
        NewSessionCommand = new AsyncCommand(NewSessionAsync);
        BackCommand = new AsyncCommand(() => Shell.Current.GoToAsync("//home", false));
        PhotoPlaceholderCommand = new AsyncCommand(() => Shell.Current.GoToAsync("photo-question", false));
        VoicePlaceholderCommand = new ReentrantAsyncCommand(StartVoiceAskAsync);
        QuickActionCommand = new AsyncCommand<string>(HandleQuickActionAsync);
    }

    public ObservableCollection<ChatMessageViewModel> Messages { get; }
    public ObservableCollection<string> QuickQuestions { get; }

    public string? SelectedGrade
    {
        get => _selectedGrade;
        set
        {
            if (SetProperty(ref _selectedGrade, value))
            {
                OnPropertyChanged(nameof(LearningCaption));
            }
        }
    }

    public string? SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            if (SetProperty(ref _selectedSubject, value))
            {
                OnPropertyChanged(nameof(LearningCaption));
            }
        }
    }

    public string LearningCaption => $"{SelectedGrade ?? "三年级"} · {SelectedSubject ?? "数学"}";
    public bool IsThinkingModeEnabled { get => _isThinkingModeEnabled; set => SetProperty(ref _isThinkingModeEnabled, value); }
    public string QuestionText { get => _questionText; set => SetProperty(ref _questionText, value); }

    /// <summary>
    /// 聊天页顶部轻提示，替代平台 Toast 依赖。
    /// </summary>
    public string? ToastMessage
    {
        get => _toastMessage;
        set
        {
            if (SetProperty(ref _toastMessage, value))
            {
                OnPropertyChanged(nameof(HasToast));
            }
        }
    }

    public bool HasToast => !string.IsNullOrWhiteSpace(ToastMessage);

    /// <summary>
    /// 当前是否正在等待 AI 流式回答。
    /// </summary>
    public bool IsSending
    {
        get => _isSending;
        private set
        {
            if (SetProperty(ref _isSending, value))
            {
                OnPropertyChanged(nameof(SendButtonText));
            }
        }
    }

    /// <summary>
    /// 当前是否正在进行语音识别。
    /// </summary>
    public bool IsListening
    {
        get => _isListening;
        private set
        {
            if (SetProperty(ref _isListening, value))
            {
                OnPropertyChanged(nameof(VoiceButtonOpacity));
            }
        }
    }

    public double VoiceButtonOpacity => IsListening ? 0.55 : 1.0;

    /// <summary>
    /// 发送按钮文本。回答中显示停止符号，方便学生随时打断。
    /// </summary>
    public string SendButtonText => IsSending ? "■" : "➤";

    public ICommand SubmitCommand { get; }
    public ICommand NewSessionCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand PhotoPlaceholderCommand { get; }
    public ICommand VoicePlaceholderCommand { get; }
    public ICommand QuickActionCommand { get; }

    /// <summary>
    /// 从设置页同步当前年级和学科，确保聊天请求使用统一学习信息。
    /// </summary>
    private void ApplyLearningSettings()
    {
        SelectedGrade = _settingsService.GetCurrentGrade();
        var subject = _settingsService.GetCurrentSubject();
        SelectedSubject = Subjects.Contains(subject) ? subject : "数学";
    }

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(ApplyLearningSettings);
    }

    private Task SubmitOrStopAsync()
    {
        if (IsSending)
        {
            StopCurrentAnswer();
            return Task.CompletedTask;
        }

        return SubmitMessageAsync(QuestionText, speakAnswer: true);
    }

    /// <summary>
    /// 语音提问：调用系统语音识别拿到文字，然后走原有文本问答链路。
    /// </summary>
    private async Task StartVoiceAskAsync()
    {
        if (IsSending)
        {
            StopCurrentAnswer();
            return;
        }

        if (IsListening)
        {
            StopListening();
            await ShowToastAsync("已停止听写。");
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();
        _listenCancellationTokenSource = cancellationTokenSource;
        IsListening = true;

        try
        {
            await ShowToastAsync("请说出你想问 AI 老师的问题。");
            var recognizedText = await _speechInteractionService.ListenOnceAsync(cancellationTokenSource.Token);
            if (string.IsNullOrWhiteSpace(recognizedText))
            {
                await ShowToastAsync("没有听清楚，请再试一次。");
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() => QuestionText = recognizedText.Trim());
            await SubmitMessageAsync(recognizedText, speakAnswer: true);
        }
        catch (OperationCanceledException)
        {
            await ShowToastAsync("已停止听写。");
        }
        finally
        {
            if (ReferenceEquals(_listenCancellationTokenSource, cancellationTokenSource))
            {
                _listenCancellationTokenSource = null;
            }

            cancellationTokenSource.Dispose();
            IsListening = false;
        }
    }

    private async Task SubmitMessageAsync(string? message, bool speakAnswer)
    {
        if (IsSending)
        {
            await ShowToastAsync("AI 老师正在回答，稍等一下。");
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            await ShowToastAsync("先写下想问的问题。");
            return;
        }

        _speechInteractionService.Stop();

        var userText = message.Trim();
        var answer = new ChatMessageViewModel("AI TUTOR", "AI 老师正在思考...", false);
        var hasReceivedDelta = false;
        var conversationContext = BuildConversationContext();
        var cancellationTokenSource = new CancellationTokenSource();
        _sendCancellationTokenSource = cancellationTokenSource;
        IsSending = true;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            QuestionText = string.Empty;
            Messages.Add(new ChatMessageViewModel("ME", userText, true));
            Messages.Add(answer);
        });

        try
        {
            var finalResponse = await Task.Run(() => _apiClientService.AskStreamAsync(new AgentRequest
            {
                UserId = _currentUserService.UserId,
                SessionId = _sessionId,
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "text",
                Mode = "ask",
                EnableThinking = IsThinkingModeEnabled,
                QuestionText = userText,
                ConversationContext = conversationContext
            }, async delta =>
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!hasReceivedDelta)
                    {
                        answer.MessageText = string.Empty;
                        hasReceivedDelta = true;
                    }

                    answer.AppendMessageText(delta);
                });
            }, cancellationTokenSource.Token), cancellationTokenSource.Token);

            if (!string.IsNullOrWhiteSpace(finalResponse?.SessionId))
            {
                _sessionId = finalResponse.SessionId;
            }

            if (!hasReceivedDelta && !string.IsNullOrWhiteSpace(finalResponse?.AnswerText))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    answer.MessageText = finalResponse.AnswerText;
                    hasReceivedDelta = true;
                });
            }

            if (!hasReceivedDelta)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    answer.MessageText = "AI 老师暂时没有返回内容，请稍后再试。";
                });
            }

            RememberTurn(userText, answer.RawMessageText);

            if (speakAnswer && !cancellationTokenSource.IsCancellationRequested)
            {
                await _speechInteractionService.SpeakAsync(answer.RawMessageText, cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!hasReceivedDelta)
                {
                    answer.MessageText = "已停止回答。";
                    return;
                }

                answer.AppendMessageText("\n\n已停止回答。");
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                answer.MessageText = $"连接 AI 老师时遇到问题：{ex.Message}";
            });
        }
        finally
        {
            if (ReferenceEquals(_sendCancellationTokenSource, cancellationTokenSource))
            {
                _sendCancellationTokenSource = null;
            }

            cancellationTokenSource.Dispose();
            IsSending = false;
        }
    }

    private async Task NewSessionAsync()
    {
        if (IsSending)
        {
            await ShowToastAsync("AI 老师正在回答，稍后再开新会话。");
            return;
        }

        StopListening();
        _speechInteractionService.Stop();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _sessionId = null;
            _conversationTurns.Clear();
            Messages.Clear();
            Messages.Add(new ChatMessageViewModel("AI TUTOR", "新的会话开始了。今天想先解决哪道题？", false));
        });
    }

    private async Task HandleQuickActionAsync(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        if (action == "加入错题本")
        {
            await ShowToastAsync("已加入错题本");
            return;
        }

        await SubmitMessageAsync(action, speakAnswer: true);
    }

    private async Task ShowToastAsync(string message)
    {
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = message);
        await Task.Delay(1400);
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = null);
    }

    /// <summary>
    /// 停止当前流式回答，并停止语音播报。
    /// </summary>
    private void StopCurrentAnswer()
    {
        _sendCancellationTokenSource?.Cancel();
        _speechInteractionService.Stop();
    }

    private void StopListening()
    {
        _listenCancellationTokenSource?.Cancel();
        _speechInteractionService.Stop();
    }

    /// <summary>
    /// 构造同一轮会话的最近上下文，帮助模型理解“3”“不会”“为什么”等短回复。
    /// </summary>
    private string BuildConversationContext()
    {
        if (_conversationTurns.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var turn in _conversationTurns.TakeLast(4))
        {
            builder.Append("学生：").AppendLine(TrimForContext(turn.UserText));
            builder.Append("AI老师：").AppendLine(TrimForContext(turn.AssistantText));
        }

        return builder.ToString();
    }

    /// <summary>
    /// 记录本轮问答，供下一次同会话请求携带上下文；新会话会清空该列表。
    /// </summary>
    private void RememberTurn(string userText, string assistantText)
    {
        _conversationTurns.Add(new ConversationTurn(userText, assistantText));
        if (_conversationTurns.Count > 8)
        {
            _conversationTurns.RemoveRange(0, _conversationTurns.Count - 8);
        }
    }

    /// <summary>
    /// 限制上下文长度，避免把过长回答反复传给后端。
    /// </summary>
    private static string TrimForContext(string text)
    {
        var normalized = text.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal).Trim();
        return normalized.Length > 500 ? normalized[..500] + "..." : normalized;
    }

    private sealed record ConversationTurn(string UserText, string AssistantText);
}

public class ChatMessageViewModel : ViewModelBase
{
    private string _messageText;

    public ChatMessageViewModel(string speakerText, string messageText, bool isUser)
    {
        SpeakerText = speakerText;
        _messageText = messageText;
        IsUser = isUser;
    }

    public string SpeakerText { get; }
    public string VisibleSpeakerText => SpeakerText;
    public bool IsUser { get; }
    public Color BubbleColor => IsUser ? Color.FromArgb("#2563EB") : Color.FromArgb("#FFFFFF");
    public Color MessageColor => IsUser ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#1E293B");
    public Color SpeakerColor => IsUser ? Color.FromArgb("#93C5FD") : Color.FromArgb("#94A3B8");
    public LayoutOptions BubbleHorizontalOptions => IsUser ? LayoutOptions.End : LayoutOptions.Start;

    public string MessageText
    {
        get => _messageText;
        set
        {
            if (SetProperty(ref _messageText, value))
            {
                OnPropertyChanged(nameof(RawMessageText));
                OnPropertyChanged(nameof(VisibleText));
            }
        }
    }

    public string VisibleText => _messageText;

    /// <summary>
    /// 未清洗的原始文本，用于会话上下文。
    /// </summary>
    public string RawMessageText => _messageText;

    /// <summary>
    /// 流式追加模型输出片段，并触发展示文本刷新。
    /// </summary>
    public void AppendMessageText(string delta)
    {
        _messageText += delta;
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(VisibleText));
    }
}

public sealed class AsyncCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;

    public AsyncCommand(Func<T?, Task> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public async void Execute(object? parameter) => await _execute((T?)parameter);
}

public sealed class ReentrantAsyncCommand : ICommand
{
    private readonly Func<Task> _execute;

    public ReentrantAsyncCommand(Func<Task> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;

    /// <summary>
    /// 允许按钮在流式回答期间再次触发，用于把第二次点击转换为停止回答。
    /// </summary>
    public async void Execute(object? parameter)
    {
        await _execute();
    }
}
