namespace AiTutor.Phone.Services;

/// <summary>
/// 手机桌面入口服务，负责把儿童桌面按钮转发到系统或已安装 App。
/// </summary>
public interface IPhoneLauncherService
{
    Task OpenWeChatAsync();

    Task OpenCameraAsync();

    Task OpenPhoneAsync();

    Task OpenSmsAsync();

    Task OpenGalleryAsync();

    Task OpenAITutorAsync();

    Task OpenSystemSettingsAsync();
}
