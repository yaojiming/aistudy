using Android.Content;
using Android.Provider;
using Android.Widget;
using AiTutor.Phone.Services;
using Microsoft.Extensions.Logging;
using Application = Android.App.Application;

namespace AiTutor.Phone.Platforms.Android.Services;

/// <summary>
/// Android 手机端 Launcher 入口实现，只调用系统 Intent 或已安装 App。
/// </summary>
public sealed class PhoneLauncherService(ILogger<PhoneLauncherService> logger) : IPhoneLauncherService
{
    private const string WeChatPackageName = "com.tencent.mm";

    public Task OpenWeChatAsync()
    {
        try
        {
            var context = GetContext();
            var intent = context.PackageManager?.GetLaunchIntentForPackage(WeChatPackageName);
            if (intent is null)
            {
                return NotifyAsync("未安装微信");
            }

            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "打开微信失败。");
            return NotifyAsync("未安装微信");
        }

        return Task.CompletedTask;
    }

    public Task OpenCameraAsync()
    {
        var intent = new Intent(MediaStore.ActionImageCapture);
        return StartExternalActivityAsync(intent, "未找到相机应用", "打开相机失败。");
    }

    public Task OpenPhoneAsync()
    {
        var intent = new Intent(Intent.ActionDial);
        return StartExternalActivityAsync(intent, "未找到电话应用", "打开拨号界面失败。");
    }

    public Task OpenSmsAsync()
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("sms:"));
        return StartExternalActivityAsync(intent, "未找到短信应用", "打开短信应用失败。");
    }

    public Task OpenGalleryAsync()
    {
        var intent = new Intent(Intent.ActionView);
        intent.SetDataAndType(MediaStore.Images.Media.ExternalContentUri, "image/*");
        return StartExternalActivityAsync(intent, "未找到图片应用", "打开图库失败。");
    }

    public Task OpenAITutorAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//home", false));
    }

    public Task OpenSystemSettingsAsync()
    {
        var intent = new Intent(Settings.ActionSettings);
        return StartExternalActivityAsync(intent, "未找到系统设置", "打开系统设置失败。");
    }

    private static Context GetContext()
    {
        return Platform.CurrentActivity ?? Application.Context;
    }

    private Task StartExternalActivityAsync(Intent intent, string missingMessage, string logMessage)
    {
        try
        {
            var context = GetContext();
            if (intent.ResolveActivity(context.PackageManager!) is null)
            {
                return NotifyAsync(missingMessage);
            }

            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{LogMessage}", logMessage);
            return NotifyAsync(missingMessage);
        }

        return Task.CompletedTask;
    }

    private static Task NotifyAsync(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Toast.MakeText(GetContext(), message, ToastLength.Short)?.Show();
        });

        return Task.CompletedTask;
    }
}
