using Microsoft.Extensions.Logging;

namespace AiTutor.Phone.Services;

/// <summary>
/// 非 Android 平台的桌面入口占位实现，方便 Windows 构建和调试 Launcher UI。
/// </summary>
public sealed class PhoneLauncherService(ILogger<PhoneLauncherService> logger) : IPhoneLauncherService
{
    public Task OpenWeChatAsync() => ShowUnsupportedAsync("微信");

    public Task OpenCameraAsync() => ShowUnsupportedAsync("相机");

    public Task OpenPhoneAsync() => ShowUnsupportedAsync("电话");

    public Task OpenSmsAsync() => ShowUnsupportedAsync("短信");

    public Task OpenGalleryAsync() => ShowUnsupportedAsync("图片");

    public Task OpenAITutorAsync() => Shell.Current.GoToAsync("//home", false);

    public Task OpenSystemSettingsAsync() => ShowUnsupportedAsync("系统设置");

    private async Task ShowUnsupportedAsync(string name)
    {
        logger.LogInformation("当前平台不支持打开 {Name} 原生入口。", name);
        await Shell.Current.DisplayAlert(name, $"当前平台不支持打开{name}，请在 Android 手机上测试。", "知道了");
    }
}
