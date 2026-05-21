using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using SkiaSharp;

namespace AiTutor.Maui.Services;

/// <summary>
/// 使用 SkiaSharp 在原图像素坐标上裁剪题目图片。
/// </summary>
public sealed class ImageCropService : IImageCropService
{
    private const int MaxLongSide = 1600;
    private const int JpegQuality = 85;

    private readonly ILogger<ImageCropService> _logger;

    public ImageCropService(ILogger<ImageCropService> logger)
    {
        _logger = logger;
    }

    public Task<SizeF> GetImageSizeAsync(string imagePath)
    {
        using var bitmap = LoadBitmap(imagePath);
        return Task.FromResult(new SizeF(bitmap.Width, bitmap.Height));
    }

    public Task<byte[]> CropToJpegBytesAsync(string imagePath, RectF cropRect)
    {
        using var source = LoadBitmap(imagePath);
        var safeRect = Clamp(cropRect, source.Width, source.Height);
        if (safeRect.Width <= 1 || safeRect.Height <= 1)
        {
            throw new InvalidOperationException("裁剪区域无效，请重新选择题目框。");
        }

        var cropInfo = new SKImageInfo((int)MathF.Round(safeRect.Width), (int)MathF.Round(safeRect.Height));
        using var cropped = new SKBitmap(cropInfo);
        using (var canvas = new SKCanvas(cropped))
        {
            var src = new SKRectI((int)safeRect.Left, (int)safeRect.Top, (int)safeRect.Right, (int)safeRect.Bottom);
            var dst = new SKRect(0, 0, cropInfo.Width, cropInfo.Height);
            canvas.DrawBitmap(source, src, dst);
        }

        using var resized = ResizeIfNeeded(cropped);
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);
        var bytes = data.ToArray();

        _logger.LogInformation(
            "裁剪图片完成。原图={Width}x{Height}, 裁剪区域={Crop}, 输出={OutWidth}x{OutHeight}, JPEG={Size} bytes",
            source.Width,
            source.Height,
            safeRect,
            resized.Width,
            resized.Height,
            bytes.Length);

        return Task.FromResult(bytes);
    }

    private static SKBitmap LoadBitmap(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("图片文件不存在。", imagePath);
        }

        using var stream = File.OpenRead(imagePath);
        return SKBitmap.Decode(stream) ?? throw new InvalidOperationException("图片解码失败，请换一张更清晰的图片。");
    }

    private static RectF Clamp(RectF rect, int imageWidth, int imageHeight)
    {
        var left = Math.Clamp(rect.Left, 0, imageWidth - 1);
        var top = Math.Clamp(rect.Top, 0, imageHeight - 1);
        var right = Math.Clamp(rect.Right, left + 1, imageWidth);
        var bottom = Math.Clamp(rect.Bottom, top + 1, imageHeight);
        return new RectF(left, top, right - left, bottom - top);
    }

    private static SKBitmap ResizeIfNeeded(SKBitmap bitmap)
    {
        var longSide = Math.Max(bitmap.Width, bitmap.Height);
        if (longSide <= MaxLongSide)
        {
            return bitmap.Copy();
        }

        var scale = MaxLongSide / (float)longSide;
        var targetWidth = Math.Max(1, (int)MathF.Round(bitmap.Width * scale));
        var targetHeight = Math.Max(1, (int)MathF.Round(bitmap.Height * scale));
        var resized = bitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.Medium);
        return resized ?? bitmap.Copy();
    }
}
