using Android.App;
using Android.Runtime;

namespace AiTutor.Maui;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp()
	{
		var start = StartupTrace.Mark("MauiApp.Create start");
		var app = MauiProgram.CreateMauiApp();
		StartupTrace.Cost("MauiApp.Create", start);
		return app;
	}
}
