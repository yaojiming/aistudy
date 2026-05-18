using System.Windows.Input;

namespace AiTutor.Wpf.Infrastructure;

/// <summary>
/// 允许执行中的按钮再次触发，主要用于“发送/停止”同一个按钮。
/// </summary>
public sealed class ReentrantAsyncCommand : ICommand
{
    private readonly Func<Task> _execute;

    public ReentrantAsyncCommand(Func<Task> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter) => await _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
