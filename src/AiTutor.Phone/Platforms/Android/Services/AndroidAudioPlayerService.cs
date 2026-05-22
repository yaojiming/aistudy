#if ANDROID
using Android.Media;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

public sealed class AndroidAudioPlayerService : IAudioPlayerService
{
    private readonly ILogger<AndroidAudioPlayerService> _logger;
    private MediaPlayer? _player;
    private TaskCompletionSource? _playCompletionSource;

    public AndroidAudioPlayerService(ILogger<AndroidAudioPlayerService> logger)
    {
        _logger = logger;
    }

    public bool IsPlaying => _player?.IsPlaying == true;

    public async Task PlayAsync(string audioPathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(audioPathOrUrl))
        {
            return;
        }

        await StopAsync();

        var completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _playCompletionSource = completionSource;
        _player = new MediaPlayer();
        if (Uri.TryCreate(audioPathOrUrl, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            _player.SetDataSource(audioPathOrUrl);
        }
        else
        {
            _player.SetDataSource(audioPathOrUrl);
        }

        _player.Prepared += (_, _) =>
        {
            try
            {
                _player?.Start();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to start audio playback.");
            }
        };
        _player.Completion += (_, _) =>
        {
            completionSource.TrySetResult();
            _ = StopAsync();
        };
        _player.Error += (_, args) =>
        {
            _logger.LogWarning("Android audio playback error. What={What}, Extra={Extra}", args.What, args.Extra);
            completionSource.TrySetException(new InvalidOperationException("音频播放失败"));
            _ = StopAsync();
        };

        _player.PrepareAsync();
        await completionSource.Task;
    }

    public Task PauseAsync()
    {
        if (_player?.IsPlaying == true)
        {
            _player.Pause();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        var player = _player;
        var completionSource = _playCompletionSource;
        _player = null;
        _playCompletionSource = null;
        completionSource?.TrySetResult();
        if (player is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            if (player.IsPlaying)
            {
                player.Stop();
            }
        }
        catch
        {
            // Ignore playback stop errors.
        }
        finally
        {
            player.Reset();
            player.Release();
            player.Dispose();
        }

        return Task.CompletedTask;
    }
}
#endif
