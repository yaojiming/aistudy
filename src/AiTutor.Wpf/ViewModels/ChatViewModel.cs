using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using AiTutor.Shared.Agent;
using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Services;

namespace AiTutor.Wpf.ViewModels;

public sealed class ChatViewModel : ViewModelBase
{
    private const string UserId = "wpf-demo-user";
    private readonly IApiClientService _apiClient;
    private readonly List<ConversationTurn> _conversationTurns = [];
    private string? _sessionId;
    private CancellationTokenSource? _sendCancellationTokenSource;
    private string _selectedGrade;
    private string _selectedSubject;
    private bool _isThinkingModeEnabled;
    private string _questionText = string.Empty;
    private string? _toastMessage;
    private bool _isSending;

    public ChatViewModel(IApiClientService apiClient, IAppSettingsService settings)
    {
        _apiClient = apiClient;
        _selectedGrade = settings.Current.CurrentGrade;
        _selectedSubject = settings.Current.CurrentSubject;

        Messages =
        [
            new ChatMessageViewModel(
                "AI TUTOR",
                "你好啊，小状元！我是你的 AI 学习小助手。不管是数学难题、古诗词解析，还是英语语法，你都可以问我哦！",
                false)
        ];

        QuickQuestions = ["再讲简单点", "给我例子", "出一道类似题", "加入错题本"];
        SubmitCommand = new ReentrantAsyncCommand(SubmitOrStopAsync);
        NewSessionCommand = new AsyncRelayCommand(NewSessionAsync, () => !IsSending);
        PhotoPlaceholderCommand = new RelayCommand(() => ShowToast("拍照问答入口已预留，请先使用拍照讲题模块。"));
        VoicePlaceholderCommand = new RelayCommand(() => ShowToast("语音问答入口已预留。"));
        QuickActionCommand = new AsyncRelayCommand<string>(HandleQuickActionAsync);
    }

    public ObservableCollection<ChatMessageViewModel> Messages { get; }

    public ObservableCollection<string> QuickQuestions { get; }

    public ObservableCollection<string> Grades { get; } =
        ["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"];

    public ObservableCollection<string> Subjects { get; } =
        ["语文", "数学", "英语", "科学"];

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

    public bool IsThinkingModeEnabled
    {
        get => _isThinkingModeEnabled;
        set => SetProperty(ref _isThinkingModeEnabled, value);
    }

    public string QuestionText
    {
        get => _questionText;
        set => SetProperty(ref _questionText, value);
    }

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

    public bool IsSending
    {
        get => _isSending;
        private set
        {
            if (SetProperty(ref _isSending, value))
            {
                OnPropertyChanged(nameof(SendButtonText));
                NewSessionCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string SendButtonText => IsSending ? "■" : "➤";

    public ReentrantAsyncCommand SubmitCommand { get; }

    public AsyncRelayCommand NewSessionCommand { get; }

    public RelayCommand PhotoPlaceholderCommand { get; }

    public RelayCommand VoicePlaceholderCommand { get; }

    public AsyncRelayCommand<string> QuickActionCommand { get; }

    private Task SubmitOrStopAsync()
    {
        if (IsSending)
        {
            _sendCancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        return SubmitMessageAsync(QuestionText);
    }

    private async Task SubmitMessageAsync(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            ShowToast("先写下想问的问题。");
            return;
        }

        var userText = message.Trim();
        var answer = new ChatMessageViewModel("AI TUTOR", "AI 老师正在思考...", false);
        var hasReceivedDelta = false;
        var conversationContext = BuildConversationContext();
        var cancellationTokenSource = new CancellationTokenSource();
        _sendCancellationTokenSource = cancellationTokenSource;
        IsSending = true;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            QuestionText = string.Empty;
            Messages.Add(new ChatMessageViewModel("ME", userText, true));
            Messages.Add(answer);
        });

        try
        {
            var finalResponse = await Task.Run(() => _apiClient.AskStreamAsync(new AgentRequest
            {
                UserId = UserId,
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
                await Application.Current.Dispatcher.InvokeAsync(() =>
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
                answer.MessageText = !string.IsNullOrWhiteSpace(finalResponse?.AnswerText)
                    ? finalResponse.AnswerText
                    : "AI 老师暂时没有返回内容，请稍后再试。";
            }

            RememberTurn(userText, answer.RawMessageText);
        }
        catch (OperationCanceledException)
        {
            if (!hasReceivedDelta)
            {
                answer.MessageText = "已停止回答。";
            }
            else
            {
                answer.AppendMessageText($"{Environment.NewLine}{Environment.NewLine}已停止回答。");
            }
        }
        catch (Exception ex)
        {
            answer.MessageText = $"连接 AI 老师时遇到问题：{ex.Message}";
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
        _sessionId = null;
        _conversationTurns.Clear();
        Messages.Clear();
        Messages.Add(new ChatMessageViewModel("AI TUTOR", "新的会话开始了。今天想先解决哪道题？", false));
        await Task.CompletedTask;
    }

    private async Task HandleQuickActionAsync(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        if (action == "加入错题本")
        {
            ShowToast("已加入错题本。");
            return;
        }

        await SubmitMessageAsync(action);
    }

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

    private void RememberTurn(string userText, string assistantText)
    {
        _conversationTurns.Add(new ConversationTurn(userText, assistantText));
        if (_conversationTurns.Count > 8)
        {
            _conversationTurns.RemoveRange(0, _conversationTurns.Count - 8);
        }
    }

    private async void ShowToast(string message)
    {
        ToastMessage = message;
        await Task.Delay(1400);
        ToastMessage = null;
    }

    private static string TrimForContext(string text)
    {
        var normalized = text.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal).Trim();
        return normalized.Length > 500 ? normalized[..500] + "..." : normalized;
    }

    private sealed record ConversationTurn(string UserText, string AssistantText);
}

public sealed class ChatMessageViewModel : ViewModelBase
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

    public Brush BubbleBrush => IsUser
        ? new SolidColorBrush(Color.FromRgb(37, 99, 235))
        : Brushes.White;

    public Brush MessageBrush => IsUser
        ? Brushes.White
        : new SolidColorBrush(Color.FromRgb(30, 41, 59));

    public Brush SpeakerBrush => IsUser
        ? new SolidColorBrush(Color.FromRgb(147, 197, 253))
        : new SolidColorBrush(Color.FromRgb(148, 163, 184));

    public HorizontalAlignment BubbleAlignment => IsUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;

    public double BubbleMaxWidth => IsUser ? 460 : 920;

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

    public string RawMessageText => _messageText;

    public void AppendMessageText(string delta)
    {
        _messageText += delta;
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(RawMessageText));
    }
}
