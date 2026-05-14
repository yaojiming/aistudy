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

        // 模拟器冷启动偶发只创建窗口但不绘制 MAUI 内容；先固定白色背景，并在 OnResume 触发一次重绘。
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
