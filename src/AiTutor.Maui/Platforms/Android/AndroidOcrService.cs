#if ANDROID
using AiTutor.Maui.Models;
using Android.Gms.Tasks;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Chinese;
using AndroidUri = Android.Net.Uri;
using GmsTask = Android.Gms.Tasks.Task;

namespace AiTutor.Maui.Services;

/// <summary>
/// Android 本地 OCR 实现。使用 Google ML Kit Text Recognition v2 中文 bundled 模型，不调用在线 OCR。
/// </summary>
public sealed class AndroidOcrService : IOcrService
{
    private readonly ILogger<AndroidOcrService> _logger;

    public AndroidOcrService(ILogger<AndroidOcrService> logger)
    {
        _logger = logger;
    }

    public async System.Threading.Tasks.Task WarmUpAsync()
    {
        try
        {
            // 创建 1 像素的纯黑 PNG 作为预热输入，触发 MLKit TFLite 模型加载
            var warmupPath = Path.Combine(FileSystem.CacheDirectory, "aitutor-ocr-warmup.png");
            var warmupBytes = Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPj/HwADBwIAMCbHYQAAAABJRU5ErkJggg==");
            await File.WriteAllBytesAsync(warmupPath, warmupBytes);

            _logger.LogInformation("OCR 模型预热开始...");
            await RecognizeLinesAsync(warmupPath);
            _logger.LogInformation("OCR 模型预热完成。");

            try { File.Delete(warmupPath); } catch { }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OCR 模型预热失败，首次使用时将正常加载。");
        }
    }

    public async Task<IReadOnlyList<OcrLineInfo>> RecognizeLinesAsync(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("OCR 图片文件不存在。", imagePath);
        }

        _logger.LogInformation("OCR 开始：{ImagePath}", imagePath);
        var recognizer = TextRecognition.GetClient(new ChineseTextRecognizerOptions.Builder().Build());
        try
        {
            var image = InputImage.FromFilePath(Platform.AppContext, AndroidUri.FromFile(new Java.IO.File(imagePath)));
            var result = await AwaitJavaTaskAsync<Text>(recognizer.Process(image));
            var lines = new List<OcrLineInfo>();
            var index = 0;

            foreach (var block in result.TextBlocks)
            {
                foreach (var line in block.Lines)
                {
                    var box = line.BoundingBox;
                    if (box is null)
                    {
                        continue;
                    }

                    var bounds = new RectF(box.Left, box.Top, box.Width(), box.Height());
                    var info = new OcrLineInfo(line.Text ?? string.Empty, bounds, null, index++);
                    lines.Add(info);
                    _logger.LogDebug("OCR 行：{Text} Bounds={Bounds}", info.Text, info.Bounds);
                }
            }

            _logger.LogInformation("OCR 结束：行数={Count}", lines.Count);
            return lines;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 失败：{ImagePath}", imagePath);
            throw;
        }
        finally
        {
            recognizer.Close();
        }
    }

    /// <summary>
    /// 把 Android Java Task 转换为 .NET Task。ML Kit 绑定没有统一 async API 时使用。
    /// </summary>
    private static Task<TResult> AwaitJavaTaskAsync<TResult>(GmsTask javaTask)
        where TResult : Java.Lang.Object
    {
        var completionSource = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        javaTask.AddOnSuccessListener(new OcrSuccessListener<TResult>(completionSource));
        javaTask.AddOnFailureListener(new OcrFailureListener<TResult>(completionSource));
        return completionSource.Task;
    }

    private sealed class OcrSuccessListener<TResult> : Java.Lang.Object, IOnSuccessListener
        where TResult : Java.Lang.Object
    {
        private readonly TaskCompletionSource<TResult> _completionSource;

        public OcrSuccessListener(TaskCompletionSource<TResult> completionSource)
        {
            _completionSource = completionSource;
        }

        public void OnSuccess(Java.Lang.Object? result)
        {
            if (result is null)
            {
                _completionSource.TrySetException(new InvalidOperationException("OCR 返回结果为空。"));
                return;
            }

            _completionSource.TrySetResult(result.JavaCast<TResult>());
        }
    }

    private sealed class OcrFailureListener<TResult> : Java.Lang.Object, IOnFailureListener
        where TResult : Java.Lang.Object
    {
        private readonly TaskCompletionSource<TResult> _completionSource;

        public OcrFailureListener(TaskCompletionSource<TResult> completionSource)
        {
            _completionSource = completionSource;
        }

        public void OnFailure(Java.Lang.Exception exception)
        {
            _completionSource.TrySetException(new InvalidOperationException(exception.Message, exception));
        }
    }
}
#endif
