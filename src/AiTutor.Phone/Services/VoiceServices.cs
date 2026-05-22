using AiTutor.Shared.Voice;

namespace AiTutor.Maui.Services;

public sealed class VoiceRecordResult
{
    public bool Success { get; set; }

    public string AudioFilePath { get; set; } = string.Empty;

    public double DurationSeconds { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}

public sealed class AiChatResult
{
    public bool Success { get; set; }

    public string AnswerText { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public int? StatusCode { get; set; }

    public long ElapsedMilliseconds { get; set; }
}

public interface IVoiceRecorderService
{
    Task StartRecordAsync();

    Task<VoiceRecordResult> StopRecordAsync();

    Task CancelRecordAsync();
}

public interface IAudioPlayerService
{
    bool IsPlaying { get; }

    Task PlayAsync(string audioPathOrUrl);

    Task PauseAsync();

    Task StopAsync();
}

public interface ISpeechToTextService
{
    Task<SpeechToTextResult> ConvertSpeechToTextAsync(string audioFilePath, CancellationToken cancellationToken = default);
}

public interface ITextToSpeechService
{
    Task<TtsResult> ConvertTextToSpeechAsync(string text, CancellationToken cancellationToken = default);
}

public interface IAiChatService
{
    Task<AiChatResult> AskAsync(string userText, CancellationToken cancellationToken = default);
}

