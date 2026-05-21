namespace AiTutor.Maui.Services;

public interface ITabletMediaPickerService
{
    /// <summary>
    /// 从系统图片选择器选择一张题目或作业图片。
    /// </summary>
    Task<FileResult?> PickImageAsync();

    /// <summary>
    /// 调用系统相机拍摄一张题目或作业图片。
    /// </summary>
    Task<FileResult?> CapturePhotoAsync();
}

/// <summary>
/// 平板端图片选择服务，封装 MAUI MediaPicker，避免 Page 后台直接访问平台 API。
/// </summary>
public class TabletMediaPickerService : ITabletMediaPickerService
{
    /// <summary>
    /// 选择本地图片。
    /// </summary>
    /// <returns>用户选择的图片文件；取消时返回 null。</returns>
    public Task<FileResult?> PickImageAsync()
    {
        return MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "选择题目图片"
        });
    }

    /// <summary>
    /// 拍摄一张新图片。
    /// </summary>
    /// <returns>拍摄完成后的图片文件；取消时返回 null。</returns>
    public Task<FileResult?> CapturePhotoAsync()
    {
        return MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "拍摄题目图片"
        });
    }
}
