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
    /// 页面是否正在执行网络请求或图片处理，用于控制加载状态。
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// 统一错误提示文本，由页面绑定展示。
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// 小学年级选项。
    /// </summary>
    public ObservableCollection<string> Grades { get; } = new(["一年级", "二年级", "三年级", "四年级", "五年级", "六年级"]);

    /// <summary>
    /// 第一阶段支持的学科选项。
    /// </summary>
    public ObservableCollection<string> Subjects { get; } = new(["数学", "语文", "英语"]);

    /// <summary>
    /// 思考模式选项，传给后端影响真实模型的讲解细致程度。
    /// </summary>
    public ObservableCollection<string> ThinkingModes { get; } = new(["standard", "brief", "deep"]);

    /// <summary>
    /// 设置属性并触发变更通知，减少 ViewModel 重复代码。
    /// </summary>
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
    /// 触发属性变更通知。
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 包裹异步操作，统一处理 Busy 状态和错误消息。
    /// </summary>
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
    /// 判断命令当前是否可以执行，避免重复点击触发并发请求。
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    /// <summary>
    /// 执行异步命令，并在执行前后刷新按钮可用状态。
    /// </summary>
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
