using AiTutor.Maui.Services;
using AiTutor.Phone.Services;
using System.Globalization;

namespace AiTutor.Phone.Pages;

public partial class LauncherHomePage : ContentPage
{
    private readonly IPhoneLauncherService _launcherService;
    private IDispatcherTimer? _clockTimer;

    public LauncherHomePage() : this(ServiceHelper.GetService<IPhoneLauncherService>())
    {
    }

    public LauncherHomePage(IPhoneLauncherService launcherService)
    {
        InitializeComponent();
        _launcherService = launcherService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        UpdateClock();
        _clockTimer ??= Dispatcher.CreateTimer();
        _clockTimer.Interval = TimeSpan.FromSeconds(1);
        _clockTimer.Tick -= OnClockTick;
        _clockTimer.Tick += OnClockTick;
        _clockTimer.Start();
    }

    protected override void OnDisappearing()
    {
        _clockTimer?.Stop();
        base.OnDisappearing();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private void OnClockTick(object? sender, EventArgs e)
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        var culture = CultureInfo.GetCultureInfo("zh-CN");
        var now = DateTime.Now;

        DateLabel.Text = now.ToString("yyyy-MM-dd", culture);
        WeekLabel.Text = now.ToString("dddd", culture);
        TimeLabel.Text = now.ToString("HH:mm:ss", culture);
    }

    private async void OnWeChatTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenWeChatAsync();
    }

    private async void OnCameraTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenCameraAsync();
    }

    private async void OnPhoneTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenPhoneAsync();
    }

    private async void OnSmsTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenSmsAsync();
    }

    private async void OnGalleryTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenGalleryAsync();
    }

    private async void OnAITutorTapped(object? sender, TappedEventArgs e)
    {
        await _launcherService.OpenAITutorAsync();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await _launcherService.OpenSystemSettingsAsync();
    }
}
