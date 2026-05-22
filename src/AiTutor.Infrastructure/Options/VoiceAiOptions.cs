namespace AiTutor.Infrastructure.Options;

public sealed class VoiceAiOptions
{
    public AsrOptions Asr { get; set; } = new();

    public ChatVoiceOptions Chat { get; set; } = new();

    public TtsOptions Tts { get; set; } = new();

    public AudioOptions Audio { get; set; } = new();
}

public sealed class AsrOptions
{
    public string Provider { get; set; } = "Xunfei";

    public string BaseUrl { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public string Model { get; set; } = "iat";

    public int TimeoutSeconds { get; set; } = 60;
}

public sealed class ChatVoiceOptions
{
    public string Provider { get; set; } = "DeepSeek";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "deepseek-chat";

    public int TimeoutSeconds { get; set; } = 90;
}

public sealed class TtsOptions
{
    public string Provider { get; set; } = "Xunfei";

    public string BaseUrl { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public string Model { get; set; } = "tts";

    public string Voice { get; set; } = "xiaoyan";

    public string Format { get; set; } = "mp3";

    public int TimeoutSeconds { get; set; } = 60;
}

public sealed class AudioOptions
{
    public string RecordFormat { get; set; } = "m4a";

    public int MaxRecordSeconds { get; set; } = 60;

    public int MinRecordSeconds { get; set; } = 1;

    public bool AutoPlayAssistantVoice { get; set; } = true;

    public bool StopCurrentPlaybackWhenUserRecords { get; set; } = true;
}
