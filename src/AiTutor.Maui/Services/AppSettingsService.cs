using System.Text.Json;

namespace AiTutor.Maui.Services;

public interface IAppSettingsService
{
    /// <summary>
    /// 读取 MAUI 内置配置文件中的 API 地址与超时设置。
    /// </summary>
    Task<ApiClientOptions> GetApiOptionsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 从 Resources/Raw/appsettings.json 读取客户端配置。
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private ApiClientOptions? _cachedOptions;

    /// <summary>
    /// 获取 API 配置，并缓存结果以避免每次请求重复读取包内文件。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>API 客户端配置。</returns>
    public async Task<ApiClientOptions> GetApiOptionsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedOptions is not null)
        {
            return _cachedOptions;
        }

        try
        {
            // appsettings.json 作为 MauiAsset 打包，运行时从应用包读取。
            await using var stream = await FileSystem.OpenAppPackageFileAsync("appsettings.json");
            var settings = await JsonSerializer.DeserializeAsync<AppSettingsFile>(stream, cancellationToken: cancellationToken);
            _cachedOptions = settings?.Api ?? new ApiClientOptions();
        }
        catch
        {
            _cachedOptions = new ApiClientOptions();
        }

        _cachedOptions.BaseUrl = _cachedOptions.BaseUrl.TrimEnd('/');
        return _cachedOptions;
    }

    private sealed class AppSettingsFile
    {
        /// <summary>
        /// API 节点配置。
        /// </summary>
        public ApiClientOptions Api { get; set; } = new();
    }
}
