using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;
using AiTutor.Maui.Views;
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

        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
        builder.Services.AddSingleton<ITabletMediaPickerService, TabletMediaPickerService>();
        builder.Services.AddSingleton<IOcrService, AndroidOcrService>();
        builder.Services.AddSingleton<IQuestionRegionBuilder, QuestionRegionBuilder>();
        builder.Services.AddSingleton<IImageCropService, ImageCropService>();
        builder.Services.AddSingleton(_ => new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        });
        builder.Services.AddSingleton<IApiClientService, ApiClientService>();

        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ChatViewModel>();
        builder.Services.AddTransient<PhotoQuestionViewModel>();
        builder.Services.AddTransient<HomeworkCheckViewModel>(sp => new HomeworkCheckViewModel(
            sp.GetRequiredService<IApiClientService>(),
            sp.GetRequiredService<ITabletMediaPickerService>(),
            sp.GetRequiredService<IAppSettingsService>(),
            sp.GetRequiredService<IOcrService>(),
            sp.GetRequiredService<IQuestionRegionBuilder>(),
            sp.GetRequiredService<IImageCropService>(),
            sp.GetRequiredService<ILogger<ImageAskViewModelBase>>()));
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddSingleton<WrongBookViewModel>();
        builder.Services.AddTransient<WrongQuestionDetailViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<PhotoQuestionPage>();
        builder.Services.AddTransient<HomeworkCheckPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<WrongBookPage>();
        builder.Services.AddTransient<WrongQuestionDetailPage>();

        var app = builder.Build();

        // 后台预热 OCR 模型，避免首次拍照/选图时 UI 卡顿
        Task.Run(async () =>
        {
            try
            {
                var ocrService = app.Services.GetRequiredService<IOcrService>();
                await ocrService.WarmUpAsync();
            }
            catch
            {
                // 预热失败不影响正常使用
            }
        });

        return app;
    }
}
