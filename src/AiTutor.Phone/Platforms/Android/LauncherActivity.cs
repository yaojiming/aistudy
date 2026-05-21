#if ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AColor = Android.Graphics.Color;
using ATypeface = Android.Graphics.Typeface;
using ATypefaceStyle = Android.Graphics.TypefaceStyle;
using AView = Android.Views.View;
using Uri = Android.Net.Uri;

namespace AiTutor.Maui;

[Activity(
    Label = "AITutor",
    MainLauncher = true,
    Exported = true,
    LaunchMode = LaunchMode.SingleTask,
    ExcludeFromRecents = true,
    HardwareAccelerated = true,
    Theme = "@style/AppTheme.NoActionBar")]
[IntentFilter(
    new[] { Intent.ActionMain },
    Categories = new[]
    {
        Intent.CategoryHome,
        Intent.CategoryDefault
    })]
public class LauncherActivity : Activity
{
    private bool _firstResumeLogged;
    private TextView? _dateTextView;
    private TextView? _timeTextView;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var start = StartupTrace.Mark("LauncherActivity.OnCreate start");
        EnableDebugStrictMode();

        base.OnCreate(savedInstanceState);

        Window?.SetBackgroundDrawable(new ColorDrawable(AColor.ParseColor("#F7F9FC")));

        var setContentStart = StartupTrace.Mark("LauncherActivity.SetContentView start");
        SetContentView(BuildLauncherView());
        StartupTrace.Cost("LauncherActivity.SetContentView", setContentStart);
        Window?.DecorView?.Post(EnterHomeFullScreenMode);

        AndroidHomeDiagnostics.LogHomeStatus(this, "LauncherActivity.OnCreate", Intent);
        ApplyDeviceOwnerHomePolicyIfEnabled();
        StartupTrace.Cost("LauncherActivity.OnCreate", start);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        TrySetCurrentIntent(intent);
        StartupTrace.Log("LauncherActivity.OnNewIntent");
        AndroidHomeDiagnostics.LogHomeStatus(this, "LauncherActivity.OnNewIntent", intent);
    }

    protected override void OnResume()
    {
        var start = StartupTrace.Mark("LauncherActivity.OnResume start");
        base.OnResume();
        UpdateClockText();
        Window?.DecorView?.Post(EnterHomeFullScreenMode);

        if (!_firstResumeLogged)
        {
            _firstResumeLogged = true;
            StartupTrace.Log("LauncherActivity.FirstResume");
        }

        StartupTrace.Cost("LauncherActivity.OnResume", start);
    }

    public override void OnBackPressed()
    {
        StartupTrace.Log("LauncherActivity.OnBackPressed ignored");
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);
        if (hasFocus)
        {
            Window?.DecorView?.Post(EnterHomeFullScreenMode);
        }
    }

    private AView BuildLauncherView()
    {
        var root = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        root.SetBackgroundColor(AColor.ParseColor("#F7F9FC"));
        root.SetPadding(Dp(24), Dp(24), Dp(24), Dp(22));
        root.AddView(BuildHeaderView());

        var grid = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        grid.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            0,
            1f);

        AddRow(grid,
            CreateTile("微", "微信", "#D9FBE7", "#16A34A", () => StartEntry("WeChat", OpenWeChat)),
            CreateTile("拍", "相机", "#E0F2FE", "#0284C7", () => StartEntry("Camera", OpenCamera)));

        AddRow(grid,
            CreateTile("话", "电话", "#FEF3C7", "#D97706", () => StartEntry("Phone", OpenPhone)),
            CreateTile("信", "短信", "#FCE7F3", "#DB2777", () => StartEntry("Sms", OpenSms)));

        AddRow(grid,
            CreateTile("图", "图片", "#DCFCE7", "#15803D", () => StartEntry("Gallery", OpenGallery)),
            CreateTile("学", "AITutor", "#DBEAFE", "#2563EB", () => StartEntry("AITutor", OpenAITutor)));

        root.AddView(grid);
        UpdateClockText();
        return root;
    }

    private AView BuildHeaderView()
    {
        var header = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        header.SetGravity(GravityFlags.CenterVertical);
        header.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            Dp(76));

        var textPanel = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        textPanel.SetGravity(GravityFlags.CenterVertical);
        textPanel.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1f);

        _dateTextView = new TextView(this)
        {
            TextSize = 15
        };
        _dateTextView.SetTextColor(AColor.ParseColor("#64748B"));
        textPanel.AddView(_dateTextView);

        _timeTextView = new TextView(this)
        {
            TextSize = 31
        };
        _timeTextView.SetTypeface(ATypeface.DefaultBold, ATypefaceStyle.Bold);
        _timeTextView.SetTextColor(AColor.ParseColor("#0F172A"));
        textPanel.AddView(_timeTextView);

        header.AddView(textPanel);
        header.AddView(CreateSettingsButton());
        return header;
    }

    private AView CreateSettingsButton()
    {
        var button = new TextView(this)
        {
            Text = "设置",
            TextSize = 15,
            Gravity = GravityFlags.Center
        };
        button.SetTypeface(ATypeface.DefaultBold, ATypefaceStyle.Bold);
        button.SetTextColor(AColor.ParseColor("#475569"));

        var background = new GradientDrawable();
        background.SetColor(AColor.White);
        background.SetCornerRadius(Dp(18));
        background.SetStroke(Dp(1), AColor.ParseColor("#E2E8F0"));
        button.Background = background;
        button.Clickable = true;
        button.Focusable = true;
        button.SetOnClickListener(new ClickListener(() => StartEntry("Settings", OpenSystemSettings)));

        var layout = new LinearLayout.LayoutParams(Dp(76), Dp(48));
        layout.SetMargins(Dp(12), 0, 0, 0);
        button.LayoutParameters = layout;
        return button;
    }

    private void AddRow(LinearLayout parent, AView left, AView right)
    {
        var row = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        row.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            0,
            1f);

        row.AddView(left);
        row.AddView(right);
        parent.AddView(row);
    }

    private AView CreateTile(string icon, string label, string iconBackground, string iconColor, Action onClick)
    {
        var tile = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        tile.SetGravity(GravityFlags.Center);

        var tileParams = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1f);
        tileParams.SetMargins(Dp(10), Dp(10), Dp(10), Dp(10));
        tile.LayoutParameters = tileParams;

        var tileBackground = new GradientDrawable();
        tileBackground.SetColor(AColor.White);
        tileBackground.SetCornerRadius(Dp(24));
        tileBackground.SetStroke(Dp(1), AColor.ParseColor("#E2E8F0"));
        tile.Background = tileBackground;
        tile.Clickable = true;
        tile.Focusable = true;
        tile.SetOnClickListener(new ClickListener(onClick));

        var iconView = new TextView(this)
        {
            Text = icon,
            Gravity = GravityFlags.Center,
            TextSize = 30
        };
        iconView.SetTypeface(ATypeface.DefaultBold, ATypefaceStyle.Bold);
        iconView.SetTextColor(AColor.ParseColor(iconColor));

        var iconShape = new GradientDrawable();
        iconShape.SetColor(AColor.ParseColor(iconBackground));
        iconShape.SetCornerRadius(Dp(18));
        iconView.Background = iconShape;

        var iconParams = new LinearLayout.LayoutParams(Dp(74), Dp(74));
        iconParams.SetMargins(0, 0, 0, Dp(12));
        tile.AddView(iconView, iconParams);

        var labelView = new TextView(this)
        {
            Text = label,
            Gravity = GravityFlags.Center,
            TextSize = 21
        };
        labelView.SetTypeface(ATypeface.DefaultBold, ATypefaceStyle.Bold);
        labelView.SetTextColor(AColor.ParseColor("#0F172A"));
        tile.AddView(labelView);

        return tile;
    }

    private void StartEntry(string entryName, Action action)
    {
        var start = StartupTrace.Mark($"Start {entryName} begin");
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Android.Util.Log.Warn("AiTutor.Launcher", $"Start {entryName} failed. {ex}");
            Toast.MakeText(this, $"{entryName} 启动失败", ToastLength.Short)?.Show();
        }
        finally
        {
            StartupTrace.Cost($"Start {entryName}", start);
        }
    }

    private void OpenWeChat()
    {
        var intent = PackageManager?.GetLaunchIntentForPackage("com.tencent.mm");
        if (intent is null)
        {
            Toast.MakeText(this, "未安装微信", ToastLength.Short)?.Show();
            return;
        }

        intent.AddFlags(ActivityFlags.NewTask);
        StartActivity(intent);
    }

    private void OpenCamera()
    {
        using var appCamera = new Intent(Intent.ActionMain);
        appCamera.AddCategory("android.intent.category.APP_CAMERA");

        using var stillCamera = new Intent("android.media.action.STILL_IMAGE_CAMERA");
        StartFirstResolvable("未找到相机应用", appCamera, stillCamera);
    }

    private void OpenPhone()
    {
        using var intent = new Intent(Intent.ActionDial, Uri.Parse("tel:"));
        StartIfResolvable(intent, "未找到电话应用");
    }

    private void OpenSms()
    {
        using var appMessaging = new Intent(Intent.ActionMain);
        appMessaging.AddCategory(Intent.CategoryAppMessaging);

        using var mainMessaging = new Intent(Intent.ActionMain);
        mainMessaging.AddCategory("android.intent.category.APP_MESSAGING");

        StartFirstResolvable("未找到短信应用", appMessaging, mainMessaging);
    }

    private void OpenGallery()
    {
        using var appGallery = new Intent(Intent.ActionMain);
        appGallery.AddCategory("android.intent.category.APP_GALLERY");

        using var viewImages = new Intent(Intent.ActionView);
        viewImages.SetType("image/*");

        using var pickImages = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);

        StartFirstResolvable("未找到图片应用", appGallery, viewImages, pickImages);
    }

    private void OpenAITutor()
    {
        using var intent = new Intent(this, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
        StartActivity(intent);
    }

    private void OpenSystemSettings()
    {
        using var intent = new Intent(Settings.ActionSettings);
        StartIfResolvable(intent, "未找到系统设置");
    }

    private void StartIfResolvable(Intent intent, string errorMessage)
    {
        if (!CanResolve(intent))
        {
            Toast.MakeText(this, errorMessage, ToastLength.Short)?.Show();
            return;
        }

        StartActivity(intent);
    }

    private void StartFirstResolvable(string errorMessage, params Intent[] intents)
    {
        foreach (var intent in intents)
        {
            intent.AddFlags(ActivityFlags.NewTask);
            if (!CanResolve(intent))
            {
                continue;
            }

            StartActivity(intent);
            return;
        }

        Toast.MakeText(this, errorMessage, ToastLength.Short)?.Show();
    }

    private bool CanResolve(Intent intent)
    {
        if (PackageManager is null)
        {
            return false;
        }

#pragma warning disable CA1416
        return intent.ResolveActivity(PackageManager) is not null;
#pragma warning restore CA1416
    }

    private void UpdateClockText()
    {
        if (_dateTextView is null || _timeTextView is null)
        {
            return;
        }

        var now = DateTime.Now;
        _dateTextView.Text = $"{now:yyyy年M月d日} {GetChineseWeekday(now.DayOfWeek)}";
        _timeTextView.Text = now.ToString("HH:mm");
    }

    private static string GetChineseWeekday(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "星期一",
            DayOfWeek.Tuesday => "星期二",
            DayOfWeek.Wednesday => "星期三",
            DayOfWeek.Thursday => "星期四",
            DayOfWeek.Friday => "星期五",
            DayOfWeek.Saturday => "星期六",
            DayOfWeek.Sunday => "星期日",
            _ => string.Empty
        };
    }

    private void EnterHomeFullScreenMode()
    {
        if (Window is null)
        {
            return;
        }

        Window.AddFlags(WindowManagerFlags.Fullscreen);

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

    private void ApplyDeviceOwnerHomePolicyIfEnabled()
    {
        if (!KioskConfig.EnableDeviceOwnerHomePolicy)
        {
            return;
        }

        if (!AndroidDeviceOwnerManager.IsDeviceOwner(this))
        {
            Android.Util.Log.Info("AiTutor.DeviceOwner", "Home policy enabled, but current app is not DeviceOwner.");
            return;
        }

        AndroidDeviceOwnerManager.SetPersistentHome(this);
    }

    private void TrySetCurrentIntent(Intent? intent)
    {
        if (intent is null)
        {
            return;
        }

        if ((int)Build.VERSION.SdkInt < 35)
        {
            return;
        }

#pragma warning disable CA1416
        SetIntent(intent, null);
#pragma warning restore CA1416
    }

    private int Dp(float value)
    {
        var density = Resources?.DisplayMetrics?.Density ?? 1f;
        return (int)(value * density + 0.5f);
    }

    private static void EnableDebugStrictMode()
    {
#if DEBUG
        var threadPolicy = new StrictMode.ThreadPolicy.Builder()
            .DetectDiskReads()
            .DetectDiskWrites()
            .DetectNetwork()
            .PenaltyLog()
            .Build();

        StrictMode.SetThreadPolicy(threadPolicy);

        var vmPolicy = new StrictMode.VmPolicy.Builder()
            .DetectLeakedClosableObjects()
            .PenaltyLog()
            .Build();

        StrictMode.SetVmPolicy(vmPolicy);
#endif
    }

    private sealed class ClickListener(Action onClick) : Java.Lang.Object, AView.IOnClickListener
    {
        public void OnClick(AView? v)
        {
            onClick();
        }
    }
}
#endif
