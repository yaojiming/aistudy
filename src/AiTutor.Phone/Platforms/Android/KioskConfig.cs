#if ANDROID
namespace AiTutor.Maui;

/// <summary>
/// Android Launcher/Kiosk 相关开关。普通安装不会获得 DeviceOwner 权限。
/// </summary>
public static class KioskConfig
{
    /// <summary>
    /// 当前应用是 DeviceOwner 时，是否把 AITutor 固定为系统 Home。
    /// </summary>
    public const bool EnableDeviceOwnerHomePolicy = true;
}
#endif
