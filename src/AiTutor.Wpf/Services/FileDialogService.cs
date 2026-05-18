using Microsoft.Win32;

namespace AiTutor.Wpf.Services;

public interface IFileDialogService
{
    string? PickImageFile();
}

/// <summary>
/// WPF 文件选择服务，用于选择拍照讲题和作业检查图片。
/// </summary>
public sealed class FileDialogService : IFileDialogService
{
    public string? PickImageFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择学习图片",
            Filter = "图片文件|*.jpg;*.jpeg;*.png;*.webp|所有文件|*.*",
            Multiselect = false
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
