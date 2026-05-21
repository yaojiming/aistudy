#if ANDROID
using Android.OS;

namespace AiTutor.Maui;

/// <summary>
/// Android 启动和入口跳转性能日志。只记录耗时，不记录用户内容。
/// </summary>
public static class StartupTrace
{
    private const string Tag = "AiTutor.Perf";

    public static long Mark(string message)
    {
        Android.Util.Log.Info(Tag, $"[PERF] {message}");
        return SystemClock.ElapsedRealtime();
    }

    public static void Log(string message)
    {
        Android.Util.Log.Info(Tag, $"[PERF] {message}");
    }

    public static void Cost(string name, long startMs)
    {
        var cost = SystemClock.ElapsedRealtime() - startMs;
        Android.Util.Log.Info(Tag, $"[PERF] {name} cost={cost}ms");
    }
}
#endif
