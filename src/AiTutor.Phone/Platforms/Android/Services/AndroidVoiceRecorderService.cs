#if ANDROID
using Android.Media;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

public sealed class AndroidVoiceRecorderService : IVoiceRecorderService
{
    private const int SampleRate = 16000;

    private readonly ILogger<AndroidVoiceRecorderService> _logger;
    private AudioRecord? _audioRecord;
    private CancellationTokenSource? _recordingCancellationTokenSource;
    private Task? _recordingTask;
    private DateTimeOffset _startedAt;
    private string? _currentPath;

    public AndroidVoiceRecorderService(ILogger<AndroidVoiceRecorderService> logger)
    {
        _logger = logger;
    }

    public async Task StartRecordAsync()
    {
        var permission = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permission != PermissionStatus.Granted)
        {
            throw new InvalidOperationException("需要麦克风权限才能录音");
        }

        await CancelRecordAsync();

        var directory = Path.Combine(FileSystem.CacheDirectory, "voice");
        Directory.CreateDirectory(directory);
        _currentPath = Path.Combine(directory, $"voice-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}.pcm");

        var minBufferSize = AudioRecord.GetMinBufferSize(
            SampleRate,
            ChannelIn.Mono,
            Android.Media.Encoding.Pcm16bit);
        if (minBufferSize <= 0)
        {
            throw new InvalidOperationException("当前设备不支持 16k PCM 录音");
        }

        var bufferSize = Math.Max(minBufferSize, SampleRate);
        _audioRecord = new AudioRecord(
            AudioSource.Mic,
            SampleRate,
            ChannelIn.Mono,
            Android.Media.Encoding.Pcm16bit,
            bufferSize);
        if (_audioRecord.State != State.Initialized)
        {
            _audioRecord.Release();
            _audioRecord.Dispose();
            _audioRecord = null;
            throw new InvalidOperationException("录音设备初始化失败");
        }

        _recordingCancellationTokenSource = new CancellationTokenSource();
        _startedAt = DateTimeOffset.UtcNow;
        _audioRecord.StartRecording();
        _recordingTask = Task.Run(
            () => WritePcmAudioAsync(_audioRecord, _currentPath, bufferSize, _recordingCancellationTokenSource.Token),
            _recordingCancellationTokenSource.Token);

        _logger.LogInformation("Android PCM voice recording started. Path={Path}, SampleRate={SampleRate}", _currentPath, SampleRate);
    }

    public async Task<VoiceRecordResult> StopRecordAsync()
    {
        var audioRecord = _audioRecord;
        var path = _currentPath;
        var cancellationTokenSource = _recordingCancellationTokenSource;
        var recordingTask = _recordingTask;
        _audioRecord = null;
        _currentPath = null;
        _recordingCancellationTokenSource = null;
        _recordingTask = null;

        if (audioRecord is null || string.IsNullOrWhiteSpace(path))
        {
            return new VoiceRecordResult
            {
                Success = false,
                ErrorMessage = "当前没有正在录制的语音"
            };
        }

        var duration = Math.Max(0, (DateTimeOffset.UtcNow - _startedAt).TotalSeconds);
        try
        {
            cancellationTokenSource?.Cancel();
            audioRecord.Stop();
            if (recordingTask is not null)
            {
                try
                {
                    await recordingTask;
                }
                catch (OperationCanceledException)
                {
                    // Recording loop exits through cancellation when the user stops recording.
                }
            }

            audioRecord.Release();
            audioRecord.Dispose();
            cancellationTokenSource?.Dispose();

            var exists = File.Exists(path) && new FileInfo(path).Length > 0;
            _logger.LogInformation("Android PCM voice recording stopped. Path={Path}, DurationSeconds={DurationSeconds}, Exists={Exists}", path, duration, exists);
            return new VoiceRecordResult
            {
                Success = exists,
                AudioFilePath = path,
                DurationSeconds = duration,
                ErrorMessage = exists ? string.Empty : "录音文件保存失败"
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Android PCM voice recording stop failed. Path={Path}", path);
            cancellationTokenSource?.Dispose();
            TryDelete(path);
            return new VoiceRecordResult
            {
                Success = false,
                ErrorMessage = "录音时间太短或录音失败"
            };
        }
    }

    public async Task CancelRecordAsync()
    {
        var audioRecord = _audioRecord;
        var path = _currentPath;
        var cancellationTokenSource = _recordingCancellationTokenSource;
        var recordingTask = _recordingTask;
        _audioRecord = null;
        _currentPath = null;
        _recordingCancellationTokenSource = null;
        _recordingTask = null;

        cancellationTokenSource?.Cancel();
        if (audioRecord is not null)
        {
            try
            {
                audioRecord.Stop();
            }
            catch
            {
                // Ignore recorder stop errors when canceling.
            }

            if (recordingTask is not null)
            {
                try
                {
                    await recordingTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when canceling recording.
                }
            }

            audioRecord.Release();
            audioRecord.Dispose();
        }

        cancellationTokenSource?.Dispose();
        if (!string.IsNullOrWhiteSpace(path))
        {
            TryDelete(path);
        }
    }

    private static async Task WritePcmAudioAsync(
        AudioRecord audioRecord,
        string path,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[bufferSize];
        await using var output = File.Create(path);
        while (!cancellationToken.IsCancellationRequested)
        {
            var bytesRead = audioRecord.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            }
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Cache cleanup failure should not interrupt the user flow.
        }
    }
}
#endif
