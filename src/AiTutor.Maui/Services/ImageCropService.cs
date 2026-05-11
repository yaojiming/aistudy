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
    private const float MinSkewThresholdDegrees = 0.3f;

    private readonly ILogger<ImageCropService> _logger;

    public ImageCropService(ILogger<ImageCropService> logger)
    {
        _logger = logger;
    }

    public async Task<float?> DetectSkewAngleAsync(string imagePath)
    {
        using var source = LoadBitmap(imagePath);

        // 降采样到较小尺寸以加速分析
        var analyzeMaxSide = 600;
        using var small = ResizeToMaxSide(source, analyzeMaxSide);
        var w = small.Width;
        var h = small.Height;

        // 转灰度 + 二值化
        var pixels = new byte[w * h];
        var span = small.Pixels;
        for (var i = 0; i < span.Length; i++)
        {
            var color = span[i];
            var gray = (byte)((color.Red * 77 + color.Green * 150 + color.Blue * 29) >> 8);
            pixels[i] = gray < 128 ? (byte)0 : (byte)1; // 0=前景(文字), 1=背景
        }

        // 在 -6° ~ +6° 范围内以 0.5° 步长搜索最佳角度
        var bestAngle = 0f;
        var bestVariance = 0f;
        var searchRange = 6f;
        var step = 0.5f;

        for (var angle = -searchRange; angle <= searchRange; angle += step)
        {
            var variance = ComputeHorizontalProjectionVariance(pixels, w, h, angle * MathF.PI / 180f);
            if (variance > bestVariance)
            {
                bestVariance = variance;
                bestAngle = angle;
            }
        }

        _logger.LogInformation("倾斜检测：最佳角度={Angle:F2}°, 投影方差={Variance:F2}", bestAngle, bestVariance);
        return bestAngle;
    }

    /// <summary>
    /// 计算以指定角度做水平投影时行间差异的方差。文字行清晰时方差大。
    /// </summary>
    private static float ComputeHorizontalProjectionVariance(byte[] pixels, int w, int h, float angleRad)
    {
        var cos = MathF.Cos(angleRad);
        var sin = MathF.Sin(angleRad);
        var halfW = w / 2f;
        var halfH = h / 2f;

        // 投影到旋转后的水平行上
        var projectionCount = Math.Max(h, (int)(h * MathF.Abs(cos) + w * MathF.Abs(sin)) + 1);
        var projection = new int[projectionCount];

        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                var idx = y * w + x;
                if (pixels[idx] == 0) // 前景像素（文字）
                {
                    var rx = x - halfW;
                    var ry = y - halfH;
                    var projY = (int)(rx * sin + ry * cos + halfH);
                    if (projY >= 0 && projY < projectionCount)
                    {
                        projection[projY]++;
                    }
                }
            }
        }

        // 计算行间方差
        float mean = 0;
        for (var i = 0; i < projectionCount; i++)
        {
            mean += projection[i];
        }

        mean /= projectionCount;

        float variance = 0;
        for (var i = 0; i < projectionCount; i++)
        {
            var diff = projection[i] - mean;
            variance += diff * diff;
        }

        return variance / projectionCount;
    }

    private static SKBitmap ResizeToMaxSide(SKBitmap bitmap, int maxSide)
    {
        var longSide = Math.Max(bitmap.Width, bitmap.Height);
        if (longSide <= maxSide)
        {
            return bitmap.Copy();
        }

        var scale = maxSide / (float)longSide;
        var newWidth = Math.Max(1, (int)MathF.Round(bitmap.Width * scale));
        var newHeight = Math.Max(1, (int)MathF.Round(bitmap.Height * scale));
        return bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium) ?? bitmap.Copy();
    }

    public async Task<string> RotateImageAsync(string imagePath, float angleDegrees)
    {
        if (MathF.Abs(angleDegrees) < MinSkewThresholdDegrees)
        {
            _logger.LogInformation("旋转角度 {Angle:F2}° 低于阈值 {Threshold}°，跳过旋转。", angleDegrees, MinSkewThresholdDegrees);
            return imagePath;
        }

        using var source = LoadBitmap(imagePath);
        var radians = angleDegrees * MathF.PI / 180f;

        // 计算旋转后画布大小
        var cos = MathF.Abs(MathF.Cos(radians));
        var sin = MathF.Abs(MathF.Sin(radians));
        var newWidth = (int)MathF.Ceiling(source.Width * cos + source.Height * sin);
        var newHeight = (int)MathF.Ceiling(source.Width * sin + source.Height * cos);

        using var rotated = new SKBitmap(newWidth, newHeight);
        using var canvas = new SKCanvas(rotated);
        canvas.Clear(SKColors.White);
        canvas.Translate(newWidth / 2f, newHeight / 2f);
        canvas.RotateRadians(radians);
        canvas.DrawBitmap(source, -source.Width / 2f, -source.Height / 2f);

        using var resized = ResizeIfNeeded(rotated);
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);

        var outputPath = Path.Combine(FileSystem.CacheDirectory, $"aitutor-rotated-{Guid.NewGuid():N}.jpg");
        await File.WriteAllBytesAsync(outputPath, data.ToArray());

        _logger.LogInformation(
            "图片旋转完成：角度={Angle:F2}°, 原尺寸={SrcW}x{SrcH}, 新尺寸={NewW}x{NewH}, 输出={Path}",
            angleDegrees, source.Width, source.Height, resized.Width, resized.Height, outputPath);

        return outputPath;
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
