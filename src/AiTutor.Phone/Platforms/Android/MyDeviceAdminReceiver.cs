#if ANDROID
using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Util;

namespace AiTutor.Maui;

/// <summary>
/// Android DeviceAdmin 接收器。只有设备所有者部署流程授权后，DeviceOwner 策略才会生效。
/// </summary>
[BroadcastReceiver(
    Name = "com.beibei.aitutor.phone.MyDeviceAdminReceiver",
    Permission = "android.permission.BIND_DEVICE_ADMIN",
    Exported = true)]
[MetaData("android.app.device_admin", Resource = "@xml/device_admin_receiver")]
[IntentFilter(new[] { DeviceAdminReceiver.ActionDeviceAdminEnabled })]
public sealed class MyDeviceAdminReceiver : DeviceAdminReceiver
{
    private const string Tag = "AiTutor.DeviceAdmin";

    public override void OnEnabled(Context context, Intent intent)
    {
        base.OnEnabled(context, intent);
        Log.Info(Tag, "Device admin enabled.");
    }

    public override void OnDisabled(Context context, Intent intent)
    {
        base.OnDisabled(context, intent);
        Log.Info(Tag, "Device admin disabled.");
    }
}
#endif
