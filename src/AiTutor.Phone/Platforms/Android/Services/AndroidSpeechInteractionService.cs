#if ANDROID
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Microsoft.Extensions.Logging;

namespace AiTutor.Maui.Services;

/// <summary>
/// Android 端语音交互实现：使用系统 SpeechRecognizer 做语音转文字，使用 MAUI TextToSpeech 播报答案。
/// </summary>
public sealed class AndroidSpeechInteractionService(ILogger<AndroidSpeechInteractionService> logger) : ISpeechInteractionService
{
    private SpeechRecognizer? _speechRecognizer;
    private TaskCompletionSource<string?>? _listenCompletionSource;

    public async Task<string?> ListenOnceAsync(CancellationToken cancellationToken = default)
    {
        var permission = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permission != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("语音输入", "需要麦克风权限才能使用语音提问。", "知道了");
            return null;
        }

        var activity = Platform.CurrentActivity;
        if (activity is null)
        {
            await Shell.Current.DisplayAlert("语音输入", "当前页面还没有准备好，请稍后再试。", "知道了");
            return null;
        }

        if (!SpeechRecognizer.IsRecognitionAvailable(activity))
        {
            await Shell.Current.DisplayAlert("语音输入", "当前设备没有可用的语音识别服务。", "知道了");
            return null;
        }

        StopListening();

        _listenCompletionSource = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => _listenCompletionSource.TrySetCanceled(cancellationToken));

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(activity);
            _speechRecognizer.SetRecognitionListener(new OneShotRecognitionListener(
                result => _listenCompletionSource?.TrySetResult(result),
                error =>
                {
                    logger.LogWarning("Android speech recognition failed. ErrorCode={ErrorCode}", error);
                    _listenCompletionSource?.TrySetResult(null);
                }));

            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, "zh-CN");
            intent.PutExtra(RecognizerIntent.ExtraPrompt, "请说出你想问 AI 老师的问题");
            intent.PutExtra(RecognizerIntent.ExtraPartialResults, false);
            intent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

            _speechRecognizer.StartListening(intent);
        });

        try
        {
            return await _listenCompletionSource.Task.WaitAsync(TimeSpan.FromSeconds(18), cancellationToken);
        }
        catch (TimeoutException)
        {
            logger.LogWarning("Android speech recognition timed out.");
            return null;
        }
        finally
        {
            StopListening();
        }
    }

    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        var speechText = SpeechTextNormalizer.Normalize(text);
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
        StopListening();
    }

    private void StopListening()
    {
        var recognizer = _speechRecognizer;
        _speechRecognizer = null;

        if (recognizer is null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                recognizer.StopListening();
                recognizer.Cancel();
                recognizer.Destroy();
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to stop Android speech recognizer.");
            }
        });
    }

    private static async Task<Locale?> TryGetChineseLocaleAsync()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        return locales.FirstOrDefault(locale => locale.Language.StartsWith("zh", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class OneShotRecognitionListener(Action<string?> onResult, Action<SpeechRecognizerError> onError)
        : Java.Lang.Object, IRecognitionListener
    {
        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[]? buffer)
        {
        }

        public void OnEndOfSpeech()
        {
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            onError(error);
        }

        public void OnEvent(int eventType, Bundle? @params)
        {
        }

        public void OnPartialResults(Bundle? partialResults)
        {
        }

        public void OnReadyForSpeech(Bundle? @params)
        {
        }

        public void OnResults(Bundle? results)
        {
            var matches = results?.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            onResult(matches?.FirstOrDefault());
        }

        public void OnRmsChanged(float rmsdB)
        {
        }
    }
}
#endif
