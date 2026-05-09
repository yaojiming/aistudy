using Microsoft.Extensions.DependencyInjection;

namespace AiTutor.Maui.Services;

public static class ServiceHelper
{
    /// <summary>
    /// 从 MAUI 应用服务容器中解析 Page 需要的 ViewModel。
    /// </summary>
    /// <typeparam name="T">要解析的服务类型。</typeparam>
    /// <returns>已注册的服务实例。</returns>
    public static T GetService<T>() where T : notnull
    {
        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("应用服务容器尚未初始化。");

        return services.GetRequiredService<T>();
    }
}
