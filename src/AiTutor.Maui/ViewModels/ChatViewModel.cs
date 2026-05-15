using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AiTutor.Maui.Services;
using AiTutor.Shared.Agent;

namespace AiTutor.Maui.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiClientService _apiClientService;
    private string? _selectedGrade = "五年级";
    private string? _selectedSubject = "数学";
    private bool _isThinkingModeEnabled;
    private string _questionText = string.Empty;
    private string? _toastMessage;
    private string? _sessionId;
    private bool _isSending;
    private CancellationTokenSource? _sendCancellationTokenSource;
    private readonly List<ConversationTurn> _conversationTurns = [];

    public ChatViewModel(IApiClientService apiClientService, IAppSettingsService settingsService)
    {
        _apiClientService = apiClientService;
        _selectedGrade = settingsService.GetCurrentGrade();
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
        VoicePlaceholderCommand = new AsyncCommand(() => ShowToastAsync("语音问答入口已预留。"));
        QuickActionCommand = new AsyncCommand<string>(HandleQuickActionAsync);
    }

    public ObservableCollection<ChatMessageViewModel> Messages { get; }
    public ObservableCollection<string> QuickQuestions { get; }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
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
    /// 发送按钮文本。回答中显示停止符号，方便学生随时打断。
    /// </summary>
    public string SendButtonText => IsSending ? "■" : "➤";

    public ICommand SubmitCommand { get; }
    public ICommand NewSessionCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand PhotoPlaceholderCommand { get; }
    public ICommand VoicePlaceholderCommand { get; }
    public ICommand QuickActionCommand { get; }

    private Task SubmitOrStopAsync()
    {
        if (IsSending)
        {
            StopCurrentAnswer();
            return Task.CompletedTask;
        }

        return SubmitMessageAsync(QuestionText);
    }

    private async Task SubmitMessageAsync(string? message)
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
                UserId = "test-user",
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

            if (!hasReceivedDelta)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    answer.MessageText = "AI 老师暂时没有返回内容，请稍后再试。";
                });
            }

            RememberTurn(userText, answer.RawMessageText);
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

        await SubmitMessageAsync(action);
    }

    private async Task ShowToastAsync(string message)
    {
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = message);
        await Task.Delay(1400);
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = null);
    }

    /// <summary>
    /// 停止当前流式回答。按钮二次点击时调用。
    /// </summary>
    private void StopCurrentAnswer()
    {
        _sendCancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// 构造同一轮会话的最近上下文，帮助模型理解“3”“不会”“为什么”等短回复。
    /// </summary>
    /// <returns>最近几轮用户和 AI 的对话摘要。</returns>
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
            }
        }
    }

    /// <summary>
    /// 未清洗的原始文本，用于会话上下文，不把显示层格式化结果再次喂回模型。
    /// </summary>
    public string RawMessageText => _messageText;

    /// <summary>
    /// 流式追加模型输出片段，并触发展示文本刷新。
    /// </summary>
    /// <param name="delta">模型返回的增量文本。</param>
    public void AppendMessageText(string delta)
    {
        _messageText += delta;
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(RawMessageText));
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
    /// 允许发送按钮在流式回答期间再次触发，用于把第二次点击转换为停止回答。
    /// </summary>
    public async void Execute(object? parameter)
    {
        await _execute();
    }
}
