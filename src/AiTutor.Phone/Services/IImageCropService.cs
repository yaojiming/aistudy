using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// 原图像素坐标裁剪服务。
/// </summary>
public interface IImageCropService
{
    /// <summary>
    /// 读取本地图片像素尺寸。
    /// </summary>
    Task<SizeF> GetImageSizeAsync(string imagePath);

    /// <summary>
    /// 根据原图像素坐标裁剪并编码为 JPEG。
    /// </summary>
    Task<byte[]> CropToJpegBytesAsync(string imagePath, RectF cropRect);
}
