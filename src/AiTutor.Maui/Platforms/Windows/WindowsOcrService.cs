#if WINDOWS
using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

namespace AiTutor.Maui.Services;

/// <summary>
/// Windows Machine 端本地 OCR 实现，使用 Windows.Media.Ocr 获取文字行和原图像素坐标。
/// 不调用任何在线 OCR 服务。
/// </summary>
public sealed class WindowsOcrService : IOcrService
{
    private readonly ILogger<WindowsOcrService> _logger;

    public WindowsOcrService(ILogger<WindowsOcrService> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<OcrLineInfo>> RecognizeLinesAsync(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("图片文件不存在。", imagePath);
        }

        var engine = OcrEngine.TryCreateFromUserProfileLanguages()
            ?? OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("zh-Hans"))
            ?? throw new InvalidOperationException("当前 Windows 系统未启用可用的 OCR 语言包。");

        var file = await StorageFile.GetFileFromPathAsync(imagePath);
        using var randomAccessStream = await file.OpenReadAsync();
        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
        using var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        var result = await engine.RecognizeAsync(bitmap);

        var lines = new List<OcrLineInfo>();
        var index = 0;
        foreach (var line in result.Lines)
        {
            var bounds = line.Words.Count == 0
                ? new RectF()
                : MergeWordBounds(line.Words);

            var text = line.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text) || bounds.Width <= 0 || bounds.Height <= 0)
            {
                continue;
            }

            lines.Add(new OcrLineInfo(text, bounds, null, index++));
            _logger.LogDebug("Windows OCR Line {Index}: {Text}, Bounds={Bounds}", index, text, bounds);
        }

        _logger.LogInformation("Windows OCR 完成。ImagePath={ImagePath}, LineCount={LineCount}", imagePath, lines.Count);
        return lines;
    }

    private static RectF MergeWordBounds(IReadOnlyList<OcrWord> words)
    {
        var left = words.Min(x => (float)x.BoundingRect.X);
        var top = words.Min(x => (float)x.BoundingRect.Y);
        var right = words.Max(x => (float)(x.BoundingRect.X + x.BoundingRect.Width));
        var bottom = words.Max(x => (float)(x.BoundingRect.Y + x.BoundingRect.Height));
        return new RectF(left, top, right - left, bottom - top);
    }
}
#endif
