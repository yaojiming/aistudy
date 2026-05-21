using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;
using System.Globalization;

namespace AiTutor.Maui.Views;

public partial class HomePage : ContentPage
{
    private IDispatcherTimer? _clockTimer;

    public HomePage() : this(ServiceHelper.GetService<HomeViewModel>())
    {
    }

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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

    private void OnClockTick(object? sender, EventArgs e)
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        var culture = CultureInfo.GetCultureInfo("zh-CN");
        DateLabel.Text = now.ToString("yyyy年M月d日", culture);
        WeekLabel.Text = now.ToString("dddd", culture);
        TimeLabel.Text = now.ToString("HH:mm:ss", culture);
    }
}
