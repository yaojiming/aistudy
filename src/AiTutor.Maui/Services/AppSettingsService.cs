using System.Text.Json;

namespace AiTutor.Maui.Services;

public interface IAppSettingsService
{
    /// <summary>
    /// 读取当前 API 地址与超时设置。用户保存的地址优先于内置 appsettings.json。
    /// </summary>
    Task<ApiClientOptions> GetApiOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取当前学生年级。
    /// </summary>
    string GetCurrentGrade();

    /// <summary>
    /// 保存当前学生年级。
    /// </summary>
    void SaveCurrentGrade(string grade);

    /// <summary>
    /// 保存后端 API 地址。
    /// </summary>
    Task SaveApiBaseUrlAsync(string baseUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置变更后通知首页和新打开的学习页面刷新默认值。
    /// </summary>
    event EventHandler? SettingsChanged;
}

/// <summary>
/// 统一管理 MAUI 客户端配置。内置默认值来自 Resources/Raw/appsettings.json，运行时修改写入 Preferences。
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private const string ApiBaseUrlKey = "aitutor_api_base_url";
    private const string CurrentGradeKey = "aitutor_current_grade";
    private const string DefaultGrade = "三年级";

    private ApiClientOptions? _cachedOptions;

    public event EventHandler? SettingsChanged;

    /// <summary>
    /// 获取 API 配置，并把用户在设置页保存的后端地址覆盖到配置里。
    /// </summary>
    public async Task<ApiClientOptions> GetApiOptionsAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedOptions is null)
        {
            _cachedOptions = await LoadBundledApiOptionsAsync(cancellationToken);
        }

        var savedBaseUrl = Preferences.Get(ApiBaseUrlKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(savedBaseUrl))
        {
            _cachedOptions.BaseUrl = savedBaseUrl;
        }

        _cachedOptions.BaseUrl = NormalizeBaseUrl(_cachedOptions.BaseUrl);
        return _cachedOptions;
    }

    public string GetCurrentGrade()
    {
        return Preferences.Get(CurrentGradeKey, DefaultGrade);
    }

    public void SaveCurrentGrade(string grade)
    {
        if (string.IsNullOrWhiteSpace(grade))
        {
            return;
        }

        Preferences.Set(CurrentGradeKey, grade.Trim());
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task SaveApiBaseUrlAsync(string baseUrl, CancellationToken cancellationToken = default)
    {
        var normalizedBaseUrl = NormalizeBaseUrl(baseUrl);
        if (!Uri.TryCreate(normalizedBaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("后端地址必须是 http:// 或 https:// 开头的完整地址，例如 http://10.0.2.2:5088。");
        }

        Preferences.Set(ApiBaseUrlKey, normalizedBaseUrl);
        var options = await GetApiOptionsAsync(cancellationToken);
        options.BaseUrl = normalizedBaseUrl;
        _cachedOptions = options;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private static string NormalizeBaseUrl(string baseUrl)
    {
        return string.IsNullOrWhiteSpace(baseUrl)
            ? new ApiClientOptions().BaseUrl.TrimEnd('/')
            : baseUrl.Trim().TrimEnd('/');
    }

    private static async Task<ApiClientOptions> LoadBundledApiOptionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync("appsettings.json");
            var settings = await JsonSerializer.DeserializeAsync<AppSettingsFile>(stream, cancellationToken: cancellationToken);
            return settings?.Api ?? new ApiClientOptions();
        }
        catch
        {
            return new ApiClientOptions();
        }
    }

    private sealed class AppSettingsFile
    {
        /// <summary>
        /// API 节点配置。
        /// </summary>
        public ApiClientOptions Api { get; set; } = new();
    }
}
