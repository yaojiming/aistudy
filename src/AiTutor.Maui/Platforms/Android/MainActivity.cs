using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;

namespace AiTutor.Maui;

[Activity(Theme = "@style/Maui.MainTheme.NoActionBar", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, HardwareAccelerated = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // 模拟器冷启动偶发只创建窗口不刷新画面。保持硬件加速，避免触摸坐标错位；
        // 只设置首屏背景，并在 OnResume 里触发一次窗口刷新。
        Window?.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.White));
    }

    protected override void OnResume()
    {
        base.OnResume();

        Window?.DecorView?.PostDelayed(() =>
        {
            Window?.DecorView?.RequestLayout();
            Window?.DecorView?.Invalidate();
        }, 120);
    }
}
