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
    private readonly IVoiceRecorderService _voiceRecorderService;
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly ITextToSpeechService _textToSpeechService;
    private readonly IAiChatService _aiChatService;
    private readonly List<ConversationTurn> _conversationTurns = [];

    private string? _selectedGrade = "三年级";
    private string? _selectedSubject = "数学";
    private bool _isThinkingModeEnabled;
    private string _questionText = string.Empty;
    private string? _toastMessage;
    private string? _sessionId;
    private bool _isSending;
    private bool _isListening;
    private bool _isRecording;
    private bool _isRecognizing;
    private bool _isAiThinking;
    private bool _isSynthesizing;
    private bool _isPlaying;
    private bool _isVoiceInputMode;
    private bool _isVoiceRecordingCanceling;
    private int _recordSeconds;
    private string? _currentPlayingMessageId;
    private CancellationTokenSource? _sendCancellationTokenSource;
    private CancellationTokenSource? _listenCancellationTokenSource;
    private CancellationTokenSource? _voiceFlowCancellationTokenSource;
    private CancellationTokenSource? _recordTimerCancellationTokenSource;

    public ChatViewModel(
        IApiClientService apiClientService,
        IAppSettingsService settingsService,
        ICurrentUserService currentUserService,
        ISpeechInteractionService speechInteractionService,
        IVoiceRecorderService voiceRecorderService,
        IAudioPlayerService audioPlayerService,
        ISpeechToTextService speechToTextService,
        ITextToSpeechService textToSpeechService,
        IAiChatService aiChatService)
    {
        _apiClientService = apiClientService;
        _settingsService = settingsService;
        _currentUserService = currentUserService;
        _speechInteractionService = speechInteractionService;
        _voiceRecorderService = voiceRecorderService;
        _audioPlayerService = audioPlayerService;
        _speechToTextService = speechToTextService;
        _textToSpeechService = textToSpeechService;
        _aiChatService = aiChatService;

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
        BackCommand = new AsyncCommand(BackAsync);
        PhotoPlaceholderCommand = new AsyncCommand(() => Shell.Current.GoToAsync("photo-question", false));
        VoicePlaceholderCommand = new AsyncCommand(ToggleVoiceInputModeAsync);
        ToggleVoiceRecordCommand = new ReentrantAsyncCommand(ToggleVoiceRecordAsync);
        CancelVoiceRecordCommand = new AsyncCommand(CancelVoiceRecordAsync);
        PlayVoiceCommand = new AsyncCommand<ChatMessageViewModel>(PlayVoiceMessageAsync);
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

    public string QuestionText
    {
        get => _questionText;
        set
        {
            if (SetProperty(ref _questionText, value))
            {
                OnPropertyChanged(nameof(HasQuestionText));
                OnPropertyChanged(nameof(CanShowTextSendButton));
            }
        }
    }

    public bool HasQuestionText => !string.IsNullOrWhiteSpace(QuestionText);

    public bool CanShowTextSendButton => IsTextInputMode && HasQuestionText;

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
                RefreshVoiceStateProperties();
            }
        }
    }

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

    public bool IsRecording
    {
        get => _isRecording;
        private set
        {
            if (SetProperty(ref _isRecording, value))
            {
                OnPropertyChanged(nameof(VoiceButtonText));
                OnPropertyChanged(nameof(VoiceRecordHint));
                OnPropertyChanged(nameof(VoiceButtonOpacity));
                OnPropertyChanged(nameof(VoiceRecordingWaveText));
                OnPropertyChanged(nameof(HoldToTalkText));
                OnPropertyChanged(nameof(VoiceRecordingOverlayText));
                RefreshVoiceStateProperties();
            }
        }
    }

    public bool IsRecognizing { get => _isRecognizing; private set { if (SetProperty(ref _isRecognizing, value)) RefreshVoiceStateProperties(); } }

    public bool IsAiThinking { get => _isAiThinking; private set { if (SetProperty(ref _isAiThinking, value)) RefreshVoiceStateProperties(); } }

    public bool IsSynthesizing { get => _isSynthesizing; private set { if (SetProperty(ref _isSynthesizing, value)) RefreshVoiceStateProperties(); } }

    public bool IsPlaying { get => _isPlaying; private set => SetProperty(ref _isPlaying, value); }

    public string? CurrentPlayingMessageId { get => _currentPlayingMessageId; private set => SetProperty(ref _currentPlayingMessageId, value); }

    public bool IsVoiceInputMode
    {
        get => _isVoiceInputMode;
        private set
        {
            if (SetProperty(ref _isVoiceInputMode, value))
            {
                OnPropertyChanged(nameof(IsTextInputMode));
                OnPropertyChanged(nameof(InputModeButtonText));
                OnPropertyChanged(nameof(HoldToTalkText));
                OnPropertyChanged(nameof(CanShowTextSendButton));
            }
        }
    }

    public bool IsTextInputMode => !IsVoiceInputMode;

    public bool IsVoiceRecordingCanceling
    {
        get => _isVoiceRecordingCanceling;
        private set
        {
            if (SetProperty(ref _isVoiceRecordingCanceling, value))
            {
                OnPropertyChanged(nameof(VoiceRecordingOverlayText));
                OnPropertyChanged(nameof(VoiceRecordingOverlayColor));
            }
        }
    }

    public int RecordSeconds
    {
        get => _recordSeconds;
        private set
        {
            if (SetProperty(ref _recordSeconds, value))
            {
                OnPropertyChanged(nameof(VoiceRecordHint));
                OnPropertyChanged(nameof(VoiceRecordingWaveText));
            }
        }
    }

    public double VoiceButtonOpacity => IsRecording || IsListening ? 0.72 : 1.0;

    public string SendButtonText => IsSending ? "■" : "➤";

    public string VoiceButtonText => IsRecording ? "■" : "🎙";

    public string VoiceRecordHint => IsRecording ? $"正在听你说话... {RecordSeconds}s" : string.Empty;

    public string VoiceRecordingWaveText => RecordSeconds % 2 == 0 ? "▂ ▃ ▅ ▆ ▅ ▃ ▂" : "▃ ▅ ▆ ▇ ▆ ▅ ▃";

    public string InputModeButtonText => IsVoiceInputMode ? "⌨" : "🎙";

    public string HoldToTalkText => IsRecording ? "松开发送" : "按住 说话";

    public string VoiceRecordingOverlayText => IsVoiceRecordingCanceling ? "松开手指，取消发送" : "手指上滑，取消发送";

    public Color VoiceRecordingOverlayColor => IsVoiceRecordingCanceling ? Color.FromArgb("#B91C1C") : Color.FromArgb("#111827");

    public bool CanStartRecord => !IsRecording && !IsSending && !IsRecognizing && !IsAiThinking && !IsSynthesizing;

    public bool CanStopRecord => IsRecording;

    public bool CanCancelRecord => IsRecording;

    public ICommand SubmitCommand { get; }

    public ICommand NewSessionCommand { get; }

    public ICommand BackCommand { get; }

    public ICommand PhotoPlaceholderCommand { get; }

    public ICommand VoicePlaceholderCommand { get; }

    public ICommand ToggleVoiceRecordCommand { get; }

    public ICommand CancelVoiceRecordCommand { get; }

    public ICommand PlayVoiceCommand { get; }

    public ICommand QuickActionCommand { get; }

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

        return SubmitMessageAsync(QuestionText);
    }

    private async Task BackAsync()
    {
        await LeavePageAsync();
        await Shell.Current.GoToAsync("//home", false);
    }

    private Task ToggleVoiceInputModeAsync()
    {
        if (IsRecording)
        {
            return Task.CompletedTask;
        }

        IsVoiceInputMode = !IsVoiceInputMode;
        return Task.CompletedTask;
    }

    public async Task LeavePageAsync()
    {
        StopRecordTimer();
        IsRecording = false;
        IsPlaying = false;
        CurrentPlayingMessageId = null;
        _sendCancellationTokenSource?.Cancel();
        _voiceFlowCancellationTokenSource?.Cancel();
        StopListening();
        _speechInteractionService.Stop();
        await _audioPlayerService.StopAsync();

        try
        {
            await _voiceRecorderService.CancelRecordAsync();
        }
        catch
        {
            // 离开页面时清理录音资源，失败不影响导航。
        }
    }

    public Task BeginHoldVoiceRecordAsync()
    {
        IsVoiceRecordingCanceling = false;
        return IsVoiceInputMode ? StartVoiceRecordAsync() : Task.CompletedTask;
    }

    public Task FinishHoldVoiceRecordAsync()
    {
        if (IsVoiceRecordingCanceling)
        {
            return CancelHoldVoiceRecordAsync();
        }

        return IsRecording ? StopVoiceRecordAndSendAsync() : Task.CompletedTask;
    }

    public void UpdateHoldVoiceCancelState(double totalY)
    {
        if (IsRecording)
        {
            IsVoiceRecordingCanceling = totalY <= -70;
        }
    }

    public async Task CancelHoldVoiceRecordAsync()
    {
        IsVoiceRecordingCanceling = false;
        await CancelVoiceRecordAsync(showToast: true);
    }

    private async Task ToggleVoiceRecordAsync()
    {
        if (IsRecording)
        {
            await StopVoiceRecordAndSendAsync();
            return;
        }

        await StartVoiceRecordAsync();
    }

    private async Task StartVoiceRecordAsync()
    {
        if (!CanStartRecord)
        {
            await ShowToastAsync("AI 老师正在处理上一条消息，稍等一下。");
            return;
        }

        await _audioPlayerService.StopAsync();
        _speechInteractionService.Stop();
        _voiceFlowCancellationTokenSource?.Cancel();
        _voiceFlowCancellationTokenSource = new CancellationTokenSource();

        try
        {
            RecordSeconds = 0;
            IsVoiceRecordingCanceling = false;
            await _voiceRecorderService.StartRecordAsync();
            IsRecording = true;
            StartRecordTimer(_voiceFlowCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            await ShowToastAsync(ex.Message.Contains("权限", StringComparison.OrdinalIgnoreCase) ? ex.Message : "语音服务未配置或录音不可用");
        }
    }

    private async Task StopVoiceRecordAndSendAsync()
    {
        StopRecordTimer();
        IsRecording = false;
        IsVoiceRecordingCanceling = false;

        VoiceRecordResult recordResult;
        try
        {
            recordResult = await _voiceRecorderService.StopRecordAsync();
        }
        catch (Exception ex)
        {
            await ShowToastAsync($"录音失败：{ex.Message}");
            return;
        }

        if (!recordResult.Success)
        {
            await ShowToastAsync(recordResult.ErrorMessage);
            return;
        }

        if (recordResult.DurationSeconds < 1)
        {
            await ShowToastAsync("说话时间太短");
            return;
        }

        var userVoiceMessage = new ChatMessageViewModel("ME", "语音消息", true)
        {
            MessageType = ChatMessageType.Voice,
            VoiceFilePath = recordResult.AudioFilePath,
            VoiceDuration = recordResult.DurationSeconds,
            Status = ChatMessageStatus.Recognizing
        };

        await MainThread.InvokeOnMainThreadAsync(() => Messages.Add(userVoiceMessage));
        await RunVoiceQuestionFlowAsync(userVoiceMessage, _voiceFlowCancellationTokenSource?.Token ?? CancellationToken.None);
    }

    private Task CancelVoiceRecordAsync()
    {
        return CancelVoiceRecordAsync(showToast: true);
    }

    private async Task CancelVoiceRecordAsync(bool showToast)
    {
        StopRecordTimer();
        IsRecording = false;
        IsVoiceRecordingCanceling = false;
        _voiceFlowCancellationTokenSource?.Cancel();
        await _voiceRecorderService.CancelRecordAsync();
        if (showToast)
        {
            await ShowToastAsync("已取消发送");
        }
    }

    private async Task RunVoiceQuestionFlowAsync(ChatMessageViewModel userVoiceMessage, CancellationToken cancellationToken)
    {
        try
        {
            IsRecognizing = true;
            var asr = await _speechToTextService.ConvertSpeechToTextAsync(userVoiceMessage.VoiceFilePath, cancellationToken);
            IsRecognizing = false;

            if (!asr.Success || string.IsNullOrWhiteSpace(asr.Text))
            {
                userVoiceMessage.Status = ChatMessageStatus.Failed;
                userVoiceMessage.ErrorMessage = string.IsNullOrWhiteSpace(asr.ErrorMessage) ? "语音识别失败，请重试" : asr.ErrorMessage;
                await ShowToastAsync(userVoiceMessage.ErrorMessage);
                return;
            }

            userVoiceMessage.RecognizedText = asr.Text.Trim();
            userVoiceMessage.MessageText = userVoiceMessage.RecognizedText;
            userVoiceMessage.Status = ChatMessageStatus.Thinking;

            IsAiThinking = true;
            var ai = await _aiChatService.AskAsync(userVoiceMessage.RecognizedText, cancellationToken);
            IsAiThinking = false;

            if (!ai.Success || string.IsNullOrWhiteSpace(ai.AnswerText))
            {
                userVoiceMessage.Status = ChatMessageStatus.Failed;
                var error = string.IsNullOrWhiteSpace(ai.ErrorMessage) ? "AI 回答失败，请重试" : ai.ErrorMessage;
                await MainThread.InvokeOnMainThreadAsync(() => Messages.Add(new ChatMessageViewModel("AI TUTOR", error, false)
                {
                    Status = ChatMessageStatus.Failed,
                    ErrorMessage = error
                }));
                return;
            }

            userVoiceMessage.Status = ChatMessageStatus.Completed;
            var assistantMessage = new ChatMessageViewModel("AI TUTOR", ai.AnswerText, false)
            {
                MessageType = ChatMessageType.Mixed,
                Status = ChatMessageStatus.Synthesizing
            };

            await MainThread.InvokeOnMainThreadAsync(() => Messages.Add(assistantMessage));
            RememberTurn(userVoiceMessage.RecognizedText, ai.AnswerText);

            IsSynthesizing = true;
            var tts = await _textToSpeechService.ConvertTextToSpeechAsync(ai.AnswerText, cancellationToken);
            IsSynthesizing = false;

            if (!tts.Success)
            {
                assistantMessage.Status = ChatMessageStatus.Completed;
                assistantMessage.ErrorMessage = string.IsNullOrWhiteSpace(tts.ErrorMessage) ? "语音合成失败" : tts.ErrorMessage;
                await ShowToastAsync("AI 文字已返回，但语音合成失败");
                return;
            }

            assistantMessage.VoiceFilePath = tts.AudioFilePath;
            assistantMessage.VoiceUrl = tts.AudioUrl;
            assistantMessage.Status = ChatMessageStatus.Playing;
            await PlayVoiceMessageAsync(assistantMessage);
        }
        catch (OperationCanceledException)
        {
            userVoiceMessage.Status = ChatMessageStatus.Failed;
            userVoiceMessage.ErrorMessage = "语音问答已取消";
        }
        catch (Exception ex)
        {
            IsRecognizing = false;
            IsAiThinking = false;
            IsSynthesizing = false;
            userVoiceMessage.Status = ChatMessageStatus.Failed;
            userVoiceMessage.ErrorMessage = ex.Message.Contains("未配置", StringComparison.OrdinalIgnoreCase)
                ? "语音服务未配置"
                : $"语音问答失败：{ex.Message}";
            await ShowToastAsync(userVoiceMessage.ErrorMessage);
        }
    }

    private async Task PlayVoiceMessageAsync(ChatMessageViewModel? message)
    {
        if (message is null)
        {
            return;
        }

        var audio = ResolvePlayableAudio(message);
        if (string.IsNullOrWhiteSpace(audio))
        {
            await ShowToastAsync("这条消息没有可播放的语音");
            return;
        }

        await _audioPlayerService.StopAsync();
        CurrentPlayingMessageId = message.MessageId;
        IsPlaying = true;
        message.Status = ChatMessageStatus.Playing;

        try
        {
            await _audioPlayerService.PlayAsync(audio);
            if (CurrentPlayingMessageId == message.MessageId)
            {
                CurrentPlayingMessageId = null;
            }

            IsPlaying = false;
            message.Status = ChatMessageStatus.Completed;
        }
        catch (Exception ex)
        {
            if (CurrentPlayingMessageId == message.MessageId)
            {
                CurrentPlayingMessageId = null;
            }

            IsPlaying = false;
            message.Status = ChatMessageStatus.Completed;
            await ShowToastAsync($"播放失败：{ex.Message}");
        }
    }

    private static string ResolvePlayableAudio(ChatMessageViewModel message)
    {
        if (!string.IsNullOrWhiteSpace(message.VoiceFilePath) && File.Exists(message.VoiceFilePath))
        {
            return message.VoiceFilePath;
        }

        if (!string.IsNullOrWhiteSpace(message.VoiceUrl))
        {
            return message.VoiceUrl;
        }

        return string.Empty;
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

        _speechInteractionService.Stop();
        await _audioPlayerService.StopAsync();

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

            answer.Status = ChatMessageStatus.Completed;
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
                answer.Status = ChatMessageStatus.Failed;
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
        if (IsSending || IsRecording)
        {
            await ShowToastAsync("AI 老师正在处理消息，稍后再开新会话。");
            return;
        }

        StopListening();
        await _audioPlayerService.StopAsync();
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

        await SubmitMessageAsync(action);
    }

    private void StartRecordTimer(CancellationToken cancellationToken)
    {
        StopRecordTimer();
        _recordTimerCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _recordTimerCancellationTokenSource.Token;

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token).ConfigureAwait(false);
                if (!token.IsCancellationRequested)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => RecordSeconds++);
                }
            }
        }, token);
    }

    private void StopRecordTimer()
    {
        _recordTimerCancellationTokenSource?.Cancel();
        _recordTimerCancellationTokenSource?.Dispose();
        _recordTimerCancellationTokenSource = null;
    }

    private async Task ShowToastAsync(string message)
    {
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = message);
        await Task.Delay(1400);
        await MainThread.InvokeOnMainThreadAsync(() => ToastMessage = null);
    }

    private void StopCurrentAnswer()
    {
        _sendCancellationTokenSource?.Cancel();
        _speechInteractionService.Stop();
        _ = _audioPlayerService.StopAsync();
    }

    private void StopListening()
    {
        _listenCancellationTokenSource?.Cancel();
        _speechInteractionService.Stop();
    }

    private void RefreshVoiceStateProperties()
    {
        OnPropertyChanged(nameof(CanStartRecord));
        OnPropertyChanged(nameof(CanStopRecord));
        OnPropertyChanged(nameof(CanCancelRecord));
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

    private static string TrimForContext(string text)
    {
        var normalized = text.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal).Trim();
        return normalized.Length > 500 ? normalized[..500] + "..." : normalized;
    }

    private sealed record ConversationTurn(string UserText, string AssistantText);
}

public enum ChatMessageType
{
    Text,
    Voice,
    Image,
    Mixed
}

public enum ChatMessageStatus
{
    Recording,
    Uploading,
    Recognizing,
    Thinking,
    Synthesizing,
    Playing,
    Completed,
    Failed
}

public class ChatMessageViewModel : ViewModelBase
{
    private string _messageText;
    private string _recognizedText = string.Empty;
    private string _voiceFilePath = string.Empty;
    private string _voiceUrl = string.Empty;
    private double _voiceDuration;
    private ChatMessageType _messageType = ChatMessageType.Text;
    private ChatMessageStatus _status = ChatMessageStatus.Completed;
    private string _errorMessage = string.Empty;

    public ChatMessageViewModel(string speakerText, string messageText, bool isUser)
    {
        MessageId = Guid.NewGuid().ToString("N");
        SpeakerText = speakerText;
        _messageText = messageText;
        IsUser = isUser;
    }

    public string MessageId { get; }

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
                OnPropertyChanged(nameof(HasVisibleText));
            }
        }
    }

    public string RecognizedText
    {
        get => _recognizedText;
        set
        {
            if (SetProperty(ref _recognizedText, value))
            {
                OnPropertyChanged(nameof(HasRecognizedText));
            }
        }
    }

    public string VoiceFilePath
    {
        get => _voiceFilePath;
        set
        {
            if (SetProperty(ref _voiceFilePath, value))
            {
                OnPropertyChanged(nameof(HasVoice));
            }
        }
    }

    public string VoiceUrl
    {
        get => _voiceUrl;
        set
        {
            if (SetProperty(ref _voiceUrl, value))
            {
                OnPropertyChanged(nameof(HasVoice));
            }
        }
    }

    public double VoiceDuration
    {
        get => _voiceDuration;
        set
        {
            if (SetProperty(ref _voiceDuration, value))
            {
                OnPropertyChanged(nameof(VoiceDurationText));
            }
        }
    }

    public ChatMessageType MessageType
    {
        get => _messageType;
        set
        {
            if (SetProperty(ref _messageType, value))
            {
                OnPropertyChanged(nameof(IsVoice));
            }
        }
    }

    public ChatMessageStatus Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(HasStatusText));
            }
        }
    }

    public new string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public string VisibleText => _messageText;

    public bool HasVisibleText => !string.IsNullOrWhiteSpace(_messageText);

    public bool HasRecognizedText => !string.IsNullOrWhiteSpace(RecognizedText);

    public bool IsVoice => MessageType is ChatMessageType.Voice or ChatMessageType.Mixed;

    public bool HasVoice => !string.IsNullOrWhiteSpace(VoiceFilePath) || !string.IsNullOrWhiteSpace(VoiceUrl);

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string VoiceDurationText => VoiceDuration > 0 ? $"{Math.Ceiling(VoiceDuration)}\"" : string.Empty;

    public string StatusText => Status switch
    {
        ChatMessageStatus.Recording => "正在录音",
        ChatMessageStatus.Uploading => "正在上传",
        ChatMessageStatus.Recognizing => "正在识别",
        ChatMessageStatus.Thinking => "AI 正在思考",
        ChatMessageStatus.Synthesizing => "正在生成语音",
        ChatMessageStatus.Playing => "正在播放",
        ChatMessageStatus.Failed => "失败",
        _ => string.Empty
    };

    public bool HasStatusText => !string.IsNullOrWhiteSpace(StatusText);

    public string RawMessageText => _messageText;

    public void AppendMessageText(string delta)
    {
        _messageText += delta;
        OnPropertyChanged(nameof(MessageText));
        OnPropertyChanged(nameof(VisibleText));
        OnPropertyChanged(nameof(HasVisibleText));
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

    public async void Execute(object? parameter)
    {
        await _execute();
    }
}
