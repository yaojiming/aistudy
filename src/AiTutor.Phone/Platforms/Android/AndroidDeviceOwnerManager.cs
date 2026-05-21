#if ANDROID
using Android.App.Admin;
using Android.Content;
using Android.Util;
using JavaClass = Java.Lang.Class;
using JavaSecurityException = Java.Lang.SecurityException;

namespace AiTutor.Maui;

/// <summary>
/// DeviceOwner 模式下的 Home 固定策略。普通安装没有权限执行这些策略。
/// </summary>
public static class AndroidDeviceOwnerManager
{
    private const string Tag = "AiTutor.DeviceOwner";

    public static bool IsDeviceOwner(Context context)
    {
        try
        {
            var manager = context.GetSystemService(Context.DevicePolicyService) as DevicePolicyManager;
            return manager?.IsDeviceOwnerApp(context.PackageName) == true;
        }
        catch (System.Exception ex)
        {
            Log.Warn(Tag, $"Failed to check DeviceOwner status. {ex}");
            return false;
        }
    }

    public static void SetPersistentHome(Context context)
    {
        try
        {
            if (!IsDeviceOwner(context))
            {
                Log.Info(Tag, "Skip SetPersistentHome because current app is not DeviceOwner.");
                return;
            }

            var manager = context.GetSystemService(Context.DevicePolicyService) as DevicePolicyManager;
            if (manager is null)
            {
                Log.Warn(Tag, "DevicePolicyManager is unavailable.");
                return;
            }

            using var filter = new IntentFilter(Intent.ActionMain);
            filter.AddCategory(Intent.CategoryHome);
            filter.AddCategory(Intent.CategoryDefault);

            using var admin = new ComponentName(context, JavaClass.FromType(typeof(MyDeviceAdminReceiver)));
            using var homeActivity = new ComponentName(context, JavaClass.FromType(typeof(LauncherActivity)));

            manager.AddPersistentPreferredActivity(admin, filter, homeActivity);
            Log.Info(Tag, $"Persistent preferred Home set to {homeActivity.PackageName}/{homeActivity.ClassName}.");
        }
        catch (JavaSecurityException ex)
        {
            Log.Error(Tag, $"SetPersistentHome failed: app is not allowed to manage DeviceOwner policy. {ex}");
        }
        catch (System.Exception ex)
        {
            Log.Error(Tag, $"SetPersistentHome failed. {ex}");
        }
    }

    public static void ClearPersistentHome(Context context)
    {
        try
        {
            if (!IsDeviceOwner(context))
            {
                Log.Info(Tag, "Skip ClearPersistentHome because current app is not DeviceOwner.");
                return;
            }

            var manager = context.GetSystemService(Context.DevicePolicyService) as DevicePolicyManager;
            if (manager is null)
            {
                Log.Warn(Tag, "DevicePolicyManager is unavailable.");
                return;
            }

            using var admin = new ComponentName(context, JavaClass.FromType(typeof(MyDeviceAdminReceiver)));
            manager.ClearPackagePersistentPreferredActivities(admin, context.PackageName);
            Log.Info(Tag, $"Persistent preferred Home cleared for {context.PackageName}.");
        }
        catch (JavaSecurityException ex)
        {
            Log.Error(Tag, $"ClearPersistentHome failed: app is not allowed to manage DeviceOwner policy. {ex}");
        }
        catch (System.Exception ex)
        {
            Log.Error(Tag, $"ClearPersistentHome failed. {ex}");
        }
    }
}
#endif
