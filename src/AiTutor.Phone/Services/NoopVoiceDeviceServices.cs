namespace AiTutor.Maui.Services;

public sealed class NoopVoiceRecorderService : IVoiceRecorderService
{
    public Task StartRecordAsync()
    {
        throw new PlatformNotSupportedException("当前平台暂不支持录音，请在 Android 设备上测试。");
    }

    public Task<VoiceRecordResult> StopRecordAsync()
    {
        return Task.FromResult(new VoiceRecordResult
        {
            Success = false,
            ErrorMessage = "当前平台暂不支持录音"
        });
    }

    public Task CancelRecordAsync()
    {
        return Task.CompletedTask;
    }
}

public sealed class NoopAudioPlayerService : IAudioPlayerService
{
    public bool IsPlaying => false;

    public Task PlayAsync(string audioPathOrUrl)
    {
        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}

