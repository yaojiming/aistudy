namespace AiTutor.Shared.Voice;

public sealed class SpeechToTextResult
{
    public bool Success { get; set; }

    public string Text { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public int? StatusCode { get; set; }

    public long ElapsedMilliseconds { get; set; }
}

public sealed class TtsRequest
{
    public string Text { get; set; } = string.Empty;
}

public sealed class TtsResult
{
    public bool Success { get; set; }

    public string AudioFilePath { get; set; } = string.Empty;

    public string AudioUrl { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public int? StatusCode { get; set; }

    public long ElapsedMilliseconds { get; set; }
}

