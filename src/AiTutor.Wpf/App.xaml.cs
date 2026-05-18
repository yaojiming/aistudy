using System.Net.Http;
using System.Windows;
using AiTutor.Wpf.Services;
using AiTutor.Wpf.ViewModels;

namespace AiTutor.Wpf;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var settings = new AppSettingsService();
        await settings.LoadAsync();

        var apiClient = new ApiClientService(new HttpClient(), settings);
        var fileDialog = new FileDialogService();
        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(apiClient, settings, fileDialog)
        };

        mainWindow.Show();
    }
}
