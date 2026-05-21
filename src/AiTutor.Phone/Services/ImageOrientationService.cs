using AiTutor.Maui.Models;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AiTutor.Maui.Services;

/// <summary>
/// 使用 SkiaSharp 读取 EXIF Orientation 并做基础旋转。这里不再通过 OCR 判断文字方向，避免选图后重复 OCR 导致卡顿。
/// </summary>
public sealed class ImageOrientationService : IImageOrientationService
{
    private readonly ILogger<ImageOrientationService> _logger;

    public ImageOrientationService(ILogger<ImageOrientationService> logger)
    {
        _logger = logger;
    }

    public async Task<CorrectedImageResult> CorrectOrientationAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException("图片文件不存在，请重新选择图片。", imagePath);
        }

        var exifRotation = GetExifRotation(imagePath);
        var correctedPath = await SaveRotatedCopyAsync(imagePath, exifRotation, cancellationToken);
        _logger.LogInformation(
            "图片方向纠正完成。Rotation={Rotation}, Original={Original}, Corrected={Corrected}",
            exifRotation,
            imagePath,
            correctedPath);

        return new CorrectedImageResult(
            imagePath,
            correctedPath,
            exifRotation,
            exifRotation != 0,
            exifRotation == 0 ? "未发现 EXIF 旋转信息，直接使用原方向。" : $"已按 EXIF 旋转 {exifRotation} 度。");
    }

    private static int GetExifRotation(string imagePath)
    {
        using var codec = SKCodec.Create(imagePath);
        return codec?.EncodedOrigin switch
        {
            SKEncodedOrigin.RightTop => 90,
            SKEncodedOrigin.BottomRight => 180,
            SKEncodedOrigin.LeftBottom => 270,
            _ => 0
        };
    }

    private static async Task<string> SaveRotatedCopyAsync(
        string imagePath,
        int rotationDegrees,
        CancellationToken cancellationToken)
    {
        var normalizedRotation = NormalizeRotation(rotationDegrees);
        using var bitmap = LoadBitmap(imagePath);
        using var rotated = Rotate(bitmap, normalizedRotation);
        using var image = SKImage.FromBitmap(rotated);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        var destination = Path.Combine(
            FileSystem.CacheDirectory,
            $"aitutor-corrected-exif-{Guid.NewGuid():N}.jpg");

        await using var stream = File.Create(destination);
        data.SaveTo(stream);
        await stream.FlushAsync(cancellationToken);
        return destination;
    }

    private static SKBitmap LoadBitmap(string imagePath)
    {
        using var stream = File.OpenRead(imagePath);
        return SKBitmap.Decode(stream) ?? throw new InvalidOperationException("图片解码失败，请换一张更清晰的图片。");
    }

    private static SKBitmap Rotate(SKBitmap source, int rotationDegrees)
    {
        if (rotationDegrees == 0)
        {
            return source.Copy();
        }

        var targetWidth = rotationDegrees is 90 or 270 ? source.Height : source.Width;
        var targetHeight = rotationDegrees is 90 or 270 ? source.Width : source.Height;
        var rotated = new SKBitmap(targetWidth, targetHeight);

        using var canvas = new SKCanvas(rotated);
        canvas.Clear(SKColors.Transparent);
        switch (rotationDegrees)
        {
            case 90:
                canvas.Translate(targetWidth, 0);
                canvas.RotateDegrees(90);
                break;
            case 180:
                canvas.Translate(targetWidth, targetHeight);
                canvas.RotateDegrees(180);
                break;
            case 270:
                canvas.Translate(0, targetHeight);
                canvas.RotateDegrees(270);
                break;
        }

        canvas.DrawBitmap(source, 0, 0);
        canvas.Flush();
        return rotated;
    }

    private static int NormalizeRotation(int rotationDegrees)
    {
        var normalized = rotationDegrees % 360;
        return normalized < 0 ? normalized + 360 : normalized;
    }
}

