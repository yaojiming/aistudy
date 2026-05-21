using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;

namespace AiTutor.Maui;

[Activity(
    Theme = "@style/AiTutor.SplashTheme",
    Exported = true,
    LaunchMode = LaunchMode.SingleTask,
    HardwareAccelerated = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var start = StartupTrace.Mark("MainActivity.OnCreate start");
        base.OnCreate(savedInstanceState);

        Window?.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.White));
        EnterFullScreenMode();
        StartupTrace.Cost("MainActivity.OnCreate", start);
    }

    protected override void OnNewIntent(Android.Content.Intent? intent)
    {
        base.OnNewIntent(intent);
        EnterFullScreenMode();
        StartupTrace.Log("MainActivity.OnNewIntent");
    }

    protected override void OnResume()
    {
        base.OnResume();
        EnterFullScreenMode();

        Window?.DecorView?.PostDelayed(() =>
        {
            EnterFullScreenMode();
            Window?.DecorView?.RequestLayout();
            Window?.DecorView?.Invalidate();
        }, 120);
    }

    public override void OnBackPressed()
    {
        try
        {
            base.OnBackPressed();
        }
        catch (Exception ex)
        {
            Android.Util.Log.Warn("AiTutor.Main", $"Back navigation failed. {ex}");
            Finish();
        }
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);
        if (hasFocus)
        {
            EnterFullScreenMode();
        }
    }

    private void EnterFullScreenMode()
    {
        if (Window is null)
        {
            return;
        }

        Window.AddFlags(WindowManagerFlags.Fullscreen);
        Window.AddFlags(WindowManagerFlags.KeepScreenOn);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
            var insetsController = Window.InsetsController;
            if (insetsController is not null)
            {
                insetsController.Hide(WindowInsets.Type.StatusBars() | WindowInsets.Type.NavigationBars());
                insetsController.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
            }

            return;
        }

#pragma warning disable CS0618
        Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
            SystemUiFlags.ImmersiveSticky |
            SystemUiFlags.LayoutStable |
            SystemUiFlags.LayoutHideNavigation |
            SystemUiFlags.LayoutFullscreen |
            SystemUiFlags.HideNavigation |
            SystemUiFlags.Fullscreen);
#pragma warning restore CS0618
    }

}
