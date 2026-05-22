using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;
using AiTutor.Maui.Views;
using AiTutor.Phone.Pages;
using AiTutor.Phone.Services;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
        builder.Services.AddSingleton<ITabletMediaPickerService, TabletMediaPickerService>();
#if ANDROID
        builder.Services.AddSingleton<IOcrService, AndroidOcrService>();
#elif WINDOWS
        builder.Services.AddSingleton<IOcrService, WindowsOcrService>();
#else
        builder.Services.AddSingleton<IOcrService, NoopOcrService>();
#endif
        builder.Services.AddSingleton<IQuestionRegionBuilder, QuestionRegionBuilder>();
        builder.Services.AddSingleton<IImageCropService, ImageCropService>();
        builder.Services.AddSingleton<IImageOrientationService, ImageOrientationService>();
        builder.Services.AddSingleton<IAiQuestionRegionDetector, AiQuestionRegionDetector>();
        builder.Services.AddSingleton<IHomeworkQuestionDetectService, HomeworkQuestionDetectService>();
        builder.Services.AddSingleton<IHomeworkCheckWorkflowService, ApiHomeworkCheckWorkflowService>();
        builder.Services.AddSingleton<IWrongBookService, LocalWrongBookService>();
        builder.Services.AddSingleton(_ => new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        });
        builder.Services.AddSingleton<IApiClientService, ApiClientService>();
#if ANDROID
        builder.Services.AddSingleton<IPhoneLauncherService, AiTutor.Phone.Platforms.Android.Services.PhoneLauncherService>();
        builder.Services.AddSingleton<ISpeechInteractionService, AndroidSpeechInteractionService>();
        builder.Services.AddSingleton<IVoiceRecorderService, AndroidVoiceRecorderService>();
        builder.Services.AddSingleton<IAudioPlayerService, AndroidAudioPlayerService>();
#else
        builder.Services.AddSingleton<IPhoneLauncherService, PhoneLauncherService>();
        builder.Services.AddSingleton<ISpeechInteractionService, SpeechInteractionService>();
        builder.Services.AddSingleton<IVoiceRecorderService, NoopVoiceRecorderService>();
        builder.Services.AddSingleton<IAudioPlayerService, NoopAudioPlayerService>();
#endif
        builder.Services.AddSingleton<ISpeechToTextService, ApiSpeechToTextService>();
        builder.Services.AddSingleton<ITextToSpeechService, ApiTextToSpeechService>();
        builder.Services.AddSingleton<IAiChatService, ApiAiChatService>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<PhotoQuestionViewModel>();
        builder.Services.AddSingleton<HomeworkCheckViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddSingleton<WrongBookViewModel>();
        builder.Services.AddTransient<WrongQuestionDetailViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<PhotoQuestionPage>();
        builder.Services.AddTransient<HomeworkCheckPage>();
        builder.Services.AddTransient<HomeworkCheckResultPage>();
        builder.Services.AddTransient<HomeworkCheckImagePage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<WrongBookPage>();
        builder.Services.AddTransient<WrongQuestionDetailPage>();
        builder.Services.AddTransient<LauncherHomePage>();

        return builder.Build();
    }
}
