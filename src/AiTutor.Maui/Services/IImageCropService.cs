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

    /// <summary>
    /// 检测图片中文档倾斜角度。使用像素投影分析法，正值表示顺时针倾斜。
    /// </summary>
    /// <param name="imagePath">图片路径。</param>
    /// <returns>倾斜角度（度），无法检测时返回 null。</returns>
    Task<float?> DetectSkewAngleAsync(string imagePath);

    /// <summary>
    /// 按指定角度旋转图片，保存为新文件并返回路径。正值顺时针。
    /// </summary>
    /// <param name="imagePath">原始图片路径。</param>
    /// <param name="angleDegrees">旋转角度（度），正值顺时针，负值逆时针。</param>
    /// <returns>旋转后图片的本地路径。</returns>
    Task<string> RotateImageAsync(string imagePath, float angleDegrees);
}
