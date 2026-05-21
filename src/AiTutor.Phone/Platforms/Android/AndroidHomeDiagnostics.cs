#if ANDROID
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace AiTutor.Maui;

/// <summary>
/// 输出 Android Home/Launcher 诊断信息，帮助区分 Manifest 配置问题和系统手势导航/Quickstep 接管问题。
/// </summary>
public static class AndroidHomeDiagnostics
{
    private const string Tag = "AiTutor.Home";

    public static bool IsCurrentDefaultHome(Context context)
    {
        var home = ResolveCurrentHome(context);
        return string.Equals(home.PackageName, context.PackageName, StringComparison.Ordinal);
    }

    public static string GetCurrentHomePackage(Context context)
    {
        var home = ResolveCurrentHome(context);
        return string.IsNullOrWhiteSpace(home.PackageName)
            ? "unknown"
            : $"{home.PackageName}/{home.ActivityName}";
    }

    public static void LogHomeStatus(Context context, string source, Intent? intent = null)
    {
        var home = ResolveCurrentHome(context);
        var packageName = context.PackageName ?? string.Empty;
        var isCurrentHome = string.Equals(home.PackageName, packageName, StringComparison.Ordinal);
        var isDeviceOwner = AndroidDeviceOwnerManager.IsDeviceOwner(context);
        var action = intent?.Action ?? "null";
        var categories = intent?.Categories is null ? "null" : string.Join(",", intent.Categories);
        var launchedAsHome = IsHomeIntent(intent);

        Log.Info(Tag,
            $"source={source}; currentPackage={packageName}; intentAction={action}; intentCategories={categories}; " +
            $"launchedAsHome={launchedAsHome}; resolvedHome={home.PackageName}/{home.ActivityName}; " +
            $"isCurrentDefaultHome={isCurrentHome}; isDeviceOwner={isDeviceOwner}; sdk={(int)Build.VERSION.SdkInt}");

        if (!isCurrentHome)
        {
            Log.Warn(Tag,
                "AITutor is not the resolved default Home. Please choose AITutor in Settings > Apps > Default apps > Home app.");
            return;
        }

        Log.Info(Tag,
            "AITutor is the resolved default Home. If swipe-up still returns to the system launcher, the device may be using gesture navigation/Quickstep or OEM launcher policy to intercept Home.");
    }

    private static bool IsHomeIntent(Intent? intent)
    {
        return intent?.Action == Intent.ActionMain &&
               intent.HasCategory(Intent.CategoryHome);
    }

    private static HomeResolveResult ResolveCurrentHome(Context context)
    {
        try
        {
            using var intent = new Intent(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryHome);
            intent.AddCategory(Intent.CategoryDefault);

            var packageManager = context.PackageManager;
            if (packageManager is null)
            {
                return new HomeResolveResult(string.Empty, string.Empty);
            }

#pragma warning disable CA1416
            var resolveInfo = packageManager.ResolveActivity(intent, PackageInfoFlags.MatchDefaultOnly);
#pragma warning restore CA1416
            var activityInfo = resolveInfo?.ActivityInfo;
            return new HomeResolveResult(activityInfo?.PackageName ?? string.Empty, activityInfo?.Name ?? string.Empty);
        }
        catch (Exception ex)
        {
            Log.Warn(Tag, $"Failed to resolve current Home. {ex}");
            return new HomeResolveResult(string.Empty, string.Empty);
        }
    }

    private sealed record HomeResolveResult(string PackageName, string ActivityName);
}
#endif
