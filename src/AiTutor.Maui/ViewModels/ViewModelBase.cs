using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AiTutor.Maui.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    private bool _isBusy;
    private string? _errorMessage;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 页面是否正在执行网络请求或图片处理。
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// 统一错误提示文本。
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// 小学年级选项，供问答、拍照讲题和作业检查页面复用。
    /// </summary>
    public ObservableCollection<string> Grades { get; } = new(["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"]);

    /// <summary>
    /// 第一阶段支持的学科选项。
    /// </summary>
    public ObservableCollection<string> Subjects { get; } = new(["数学", "语文", "英语"]);

    /// <summary>
    /// 璁剧疆灞炴€у苟瑙﹀彂鍙樻洿閫氱煡锛屽噺灏?ViewModel 閲嶅浠ｇ爜銆?    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// 瑙﹀彂灞炴€у彉鏇撮€氱煡銆?    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 鍖呰９寮傛鎿嶄綔锛岀粺涓€澶勭悊 Busy 鐘舵€佸拰閿欒娑堟伅銆?    /// </summary>
    protected async Task RunBusyAsync(Func<Task> action)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            ErrorMessage = null;
            IsBusy = true;
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// 鍒ゆ柇鍛戒护褰撳墠鏄惁鍙互鎵ц锛岄伩鍏嶉噸澶嶇偣鍑昏Е鍙戝苟鍙戣姹傘€?    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    /// <summary>
    /// 鎵ц寮傛鍛戒护锛屽苟鍦ㄦ墽琛屽墠鍚庡埛鏂版寜閽彲鐢ㄧ姸鎬併€?    /// </summary>
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        try
        {
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

