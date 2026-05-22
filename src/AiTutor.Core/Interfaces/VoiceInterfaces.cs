using AiTutor.Shared.Voice;

namespace AiTutor.Core.Interfaces;

public interface ISpeechToTextService
{
    Task<SpeechToTextResult> ConvertSpeechToTextAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

public interface ITextToSpeechService
{
    Task<TtsResult> ConvertTextToSpeechAsync(string text, CancellationToken cancellationToken = default);
}

