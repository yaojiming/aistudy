using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// 非 Android 平台的语音交互占位实现，保证 Windows 调试和构建可用。
/// </summary>
public sealed class SpeechInteractionService(ILogger<SpeechInteractionService> logger) : ISpeechInteractionService
{
    public async Task<string?> ListenOnceAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("当前平台暂不支持系统语音识别。");
        await Shell.Current.DisplayAlert("语音输入", "当前平台暂不支持语音输入，请在 Android 手机上测试。", "知道了");
        return null;
    }

    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        var speechText = NormalizeForSpeech(text);
        if (string.IsNullOrWhiteSpace(speechText))
        {
            return;
        }

        await TextToSpeech.Default.SpeakAsync(speechText, new SpeechOptions
        {
            Locale = await TryGetChineseLocaleAsync(),
            Pitch = 1.0f,
            Volume = 1.0f
        }, cancellationToken);
    }

    public void Stop()
    {
        // MAUI 的 ITextToSpeech 在当前目标框架没有公开取消方法；
        // 后续 SpeakAsync 会使用新的 CancellationToken 控制本轮播报生命周期。
    }

    private static async Task<Locale?> TryGetChineseLocaleAsync()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        return locales.FirstOrDefault(locale => locale.Language.StartsWith("zh", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeForSpeech(string text)
    {
        var normalized = Regex.Replace(text, @"[#*_`>$\\{}]", string.Empty);
        normalized = normalized.Replace("AI TUTOR", "AI老师", StringComparison.OrdinalIgnoreCase);
        normalized = normalized.Replace(@"\times", "乘以", StringComparison.OrdinalIgnoreCase);
        normalized = normalized.Replace(@"\div", "除以", StringComparison.OrdinalIgnoreCase);
        normalized = normalized.Replace("text", string.Empty, StringComparison.OrdinalIgnoreCase);
        return Regex.Replace(normalized, @"\s+", " ").Trim();
    }
}
