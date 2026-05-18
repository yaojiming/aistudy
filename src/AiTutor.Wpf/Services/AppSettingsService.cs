using System.IO;
using System.Text.Json;

namespace AiTutor.Wpf.Services;

public sealed class ApiClientOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5088";

    public int TimeoutSeconds { get; set; } = 600;
}

public sealed class ModelSelectionOptions
{
    public List<string> VisionModelNames { get; set; } = ["glm-4.5v"];

    public string PhotoQuestionDefaultModel { get; set; } = "glm-4.5v";

    public string HomeworkCheckDefaultModel { get; set; } = "glm-4.5v";
}

public sealed class WpfAppSettings
{
    public ApiClientOptions Api { get; set; } = new();

    public string CurrentGrade { get; set; } = "三年级";

    public string CurrentSubject { get; set; } = "数学";

    public ModelSelectionOptions Models { get; set; } = new();
}

public interface IAppSettingsService
{
    event EventHandler? SettingsChanged;

    WpfAppSettings Current { get; }

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(WpfAppSettings settings, CancellationToken cancellationToken = default);
}

/// <summary>
/// WPF 客户端本地设置服务，只保存后端地址、年级学科和模型名称，不保存任何 API Key。
/// </summary>
public sealed class AppSettingsService : IAppSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public AppSettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var directory = Path.Combine(appData, "AiTutor.Wpf");
        Directory.CreateDirectory(directory);
        _settingsPath = Path.Combine(directory, "settings.json");
    }

    public event EventHandler? SettingsChanged;

    public WpfAppSettings Current { get; private set; } = new();

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsPath))
        {
            await SaveAsync(Current, cancellationToken);
            return;
        }

        await using var stream = File.OpenRead(_settingsPath);
        Current = await JsonSerializer.DeserializeAsync<WpfAppSettings>(stream, JsonOptions, cancellationToken)
            ?? new WpfAppSettings();
        Normalize();
    }

    public async Task SaveAsync(WpfAppSettings settings, CancellationToken cancellationToken = default)
    {
        Current = settings;
        Normalize();
        await using var stream = File.Create(_settingsPath);
        await JsonSerializer.SerializeAsync(stream, Current, JsonOptions, cancellationToken);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Normalize()
    {
        Current.Api.BaseUrl = string.IsNullOrWhiteSpace(Current.Api.BaseUrl)
            ? "http://localhost:5088"
            : Current.Api.BaseUrl.Trim().TrimEnd('/');

        if (Current.Api.TimeoutSeconds < 30)
        {
            Current.Api.TimeoutSeconds = 30;
        }

        if (Current.Models.VisionModelNames.Count == 0)
        {
            Current.Models.VisionModelNames.Add("glm-4.5v");
        }

        if (string.IsNullOrWhiteSpace(Current.Models.PhotoQuestionDefaultModel))
        {
            Current.Models.PhotoQuestionDefaultModel = Current.Models.VisionModelNames[0];
        }

        if (string.IsNullOrWhiteSpace(Current.Models.HomeworkCheckDefaultModel))
        {
            Current.Models.HomeworkCheckDefaultModel = Current.Models.VisionModelNames[0];
        }
    }
}
