using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Services;

/// <summary>
/// AspectFit 图片预览坐标和原图像素坐标转换工具。
/// </summary>
public static class ImageCoordinateMapper
{
    /// <summary>
    /// 将原图像素区域转换为页面显示区域。
    /// </summary>
    public static RectF ImageRectToViewRect(RectF imageRect, float imageWidth, float imageHeight, float viewWidth, float viewHeight)
    {
        var transform = CreateTransform(imageWidth, imageHeight, viewWidth, viewHeight);
        return new RectF(
            transform.OffsetX + imageRect.X * transform.Scale,
            transform.OffsetY + imageRect.Y * transform.Scale,
            imageRect.Width * transform.Scale,
            imageRect.Height * transform.Scale);
    }

    /// <summary>
    /// 将页面点击点转换为原图像素坐标。
    /// </summary>
    public static PointF ViewPointToImagePoint(PointF viewPoint, float imageWidth, float imageHeight, float viewWidth, float viewHeight)
    {
        var transform = CreateTransform(imageWidth, imageHeight, viewWidth, viewHeight);
        return new PointF(
            (viewPoint.X - transform.OffsetX) / transform.Scale,
            (viewPoint.Y - transform.OffsetY) / transform.Scale);
    }

    /// <summary>
    /// 判断显示坐标点击点是否落在原图像素区域内。
    /// </summary>
    public static bool IsViewPointInsideImageRect(PointF viewPoint, RectF imageRect, float imageWidth, float imageHeight, float viewWidth, float viewHeight)
    {
        var imagePoint = ViewPointToImagePoint(viewPoint, imageWidth, imageHeight, viewWidth, viewHeight);
        return imageRect.Contains(imagePoint);
    }

    private static (float Scale, float OffsetX, float OffsetY) CreateTransform(float imageWidth, float imageHeight, float viewWidth, float viewHeight)
    {
        if (imageWidth <= 0 || imageHeight <= 0 || viewWidth <= 0 || viewHeight <= 0)
        {
            return (1, 0, 0);
        }

        var scale = MathF.Min(viewWidth / imageWidth, viewHeight / imageHeight);
        var displayedWidth = imageWidth * scale;
        var displayedHeight = imageHeight * scale;
        return (scale, (viewWidth - displayedWidth) / 2, (viewHeight - displayedHeight) / 2);
    }
}
