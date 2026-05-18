using AiTutor.Wpf.Infrastructure;
using AiTutor.Wpf.Services;

namespace AiTutor.Wpf.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly HomeViewModel _home;
    private readonly ChatViewModel _chat;
    private readonly PhotoQuestionViewModel _photoQuestion;
    private readonly HomeworkCheckViewModel _homeworkCheck;
    private readonly WrongBookViewModel _wrongBook;
    private readonly SettingsViewModel _settingsViewModel;
    private ViewModelBase _currentPage;

    public MainViewModel(IApiClientService apiClient, IAppSettingsService settings, IFileDialogService fileDialog)
    {
        _chat = new ChatViewModel(apiClient, settings);
        _photoQuestion = new PhotoQuestionViewModel(apiClient, settings, fileDialog);
        _homeworkCheck = new HomeworkCheckViewModel(apiClient, settings, fileDialog);
        _wrongBook = new WrongBookViewModel();
        _settingsViewModel = new SettingsViewModel(settings);
        _home = new HomeViewModel(
            () => CurrentPage = _chat,
            () => CurrentPage = _photoQuestion,
            () => CurrentPage = _homeworkCheck,
            () => CurrentPage = _wrongBook,
            () => CurrentPage = _settingsViewModel);

        _currentPage = _home;

        NavigateHomeCommand = new RelayCommand(() => CurrentPage = _home);
        NavigateChatCommand = new RelayCommand(() => CurrentPage = _chat);
        NavigatePhotoQuestionCommand = new RelayCommand(() => CurrentPage = _photoQuestion);
        NavigateHomeworkCheckCommand = new RelayCommand(() => CurrentPage = _homeworkCheck);
        NavigateWrongBookCommand = new RelayCommand(() => CurrentPage = _wrongBook);
        NavigateSettingsCommand = new RelayCommand(() => CurrentPage = _settingsViewModel);
    }

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public RelayCommand NavigateHomeCommand { get; }

    public RelayCommand NavigateChatCommand { get; }

    public RelayCommand NavigatePhotoQuestionCommand { get; }

    public RelayCommand NavigateHomeworkCheckCommand { get; }

    public RelayCommand NavigateWrongBookCommand { get; }

    public RelayCommand NavigateSettingsCommand { get; }
}
