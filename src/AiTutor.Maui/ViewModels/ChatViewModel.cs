using System.Collections.ObjectModel;
using System.Windows.Input;
using AiTutor.Maui.Services;
using AiTutor.Shared.Agent;

namespace AiTutor.Maui.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private readonly IApiClientService _apiClientService;
    private string? _selectedGrade = "五年级";
    private string? _selectedSubject = "数学";
    private string? _selectedThinkingMode = "standard";
    private string _questionText = string.Empty;
    private string? _toastMessage;

    public ChatViewModel(IApiClientService apiClientService)
    {
        _apiClientService = apiClientService;
        Messages =
        [
            new ChatMessageViewModel("AI TUTOR", "你好啊，小状元！我是你的 AI 学习小助手。不管是数学难题、古诗词解析，还是英语语法，你都可以问我哦！", false)
        ];
        QuickQuestions = ["再讲简单点", "给我例子", "出一道类似题", "加入错题本"];
        SubmitCommand = new AsyncCommand(SubmitAsync);
        NewSessionCommand = new AsyncCommand(NewSessionAsync);
        BackCommand = new AsyncCommand(() => Shell.Current.GoToAsync("//home"));
        PhotoPlaceholderCommand = new AsyncCommand(() => Shell.Current.GoToAsync("photo-question"));
        VoicePlaceholderCommand = new AsyncCommand(() => Shell.Current.DisplayAlert("语音", "语音问答入口已预留。", "知道了"));
        QuickActionCommand = new AsyncCommand<string>(HandleQuickActionAsync);
    }

    public ObservableCollection<ChatMessageViewModel> Messages { get; }
    public ObservableCollection<string> QuickQuestions { get; }

    public string? SelectedGrade { get => _selectedGrade; set => SetProperty(ref _selectedGrade, value); }
    public string? SelectedSubject { get => _selectedSubject; set => SetProperty(ref _selectedSubject, value); }
    public string? SelectedThinkingMode { get => _selectedThinkingMode; set => SetProperty(ref _selectedThinkingMode, value); }
    public string QuestionText { get => _questionText; set => SetProperty(ref _questionText, value); }

    /// <summary>
    /// 页面轻提示文本，用于替代平台 Toast 依赖。
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

    public ICommand SubmitCommand { get; }
    public ICommand NewSessionCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand PhotoPlaceholderCommand { get; }
    public ICommand VoicePlaceholderCommand { get; }
    public ICommand QuickActionCommand { get; }

    private Task SubmitAsync()
    {
        return RunBusyAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(QuestionText))
            {
                ErrorMessage = "先写下想问的问题。";
                return;
            }

            var userText = QuestionText.Trim();
            QuestionText = string.Empty;
            Messages.Add(new ChatMessageViewModel("ME", userText, true));
            var answer = new ChatMessageViewModel("AI TUTOR", string.Empty, false);
            Messages.Add(answer);

            await _apiClientService.AskStreamAsync(new AgentRequest
            {
                UserId = "test-user",
                Grade = SelectedGrade,
                Subject = SelectedSubject,
                InputType = "text",
                Mode = "ask",
                ThinkingMode = SelectedThinkingMode,
                QuestionText = userText
            }, async delta =>
            {
                await MainThread.InvokeOnMainThreadAsync(() => answer.MessageText += delta);
            });
        });
    }

    private Task NewSessionAsync()
    {
        Messages.Clear();
        Messages.Add(new ChatMessageViewModel("AI TUTOR", "新的会话开始了。今天想先解决哪道题？", false));
        return Task.CompletedTask;
    }

    private async Task HandleQuickActionAsync(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        if (action == "加入错题本")
        {
            ToastMessage = "已加入错题本";
            await Task.Delay(1600);
            ToastMessage = null;
            return;
        }

        QuestionText = action;
        await SubmitAsync();
    }
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
    public string MessageText { get => _messageText; set => SetProperty(ref _messageText, value); }
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
