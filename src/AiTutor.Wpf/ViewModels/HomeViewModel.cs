using AiTutor.Wpf.Infrastructure;

namespace AiTutor.Wpf.ViewModels;

public sealed class HomeViewModel : ViewModelBase
{
    public HomeViewModel(
        Action openChat,
        Action openPhotoQuestion,
        Action openHomeworkCheck,
        Action openWrongBook,
        Action openSettings)
    {
        OpenChatCommand = new RelayCommand(openChat);
        OpenPhotoQuestionCommand = new RelayCommand(openPhotoQuestion);
        OpenHomeworkCheckCommand = new RelayCommand(openHomeworkCheck);
        OpenWrongBookCommand = new RelayCommand(openWrongBook);
        OpenSettingsCommand = new RelayCommand(openSettings);
    }

    public RelayCommand OpenChatCommand { get; }

    public RelayCommand OpenPhotoQuestionCommand { get; }

    public RelayCommand OpenHomeworkCheckCommand { get; }

    public RelayCommand OpenWrongBookCommand { get; }

    public RelayCommand OpenSettingsCommand { get; }
}
