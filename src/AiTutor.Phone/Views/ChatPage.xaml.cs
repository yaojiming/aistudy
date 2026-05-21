using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;
using System.Collections.Specialized;

namespace AiTutor.Maui.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;

    public ChatPage() : this(ServiceHelper.GetService<ChatViewModel>())
    {
    }

    public ChatPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        viewModel.Messages.CollectionChanged += OnMessagesChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ScrollToBottom();
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                MessagesScroll.ScrollToAsync(0, double.MaxValue, false));
        }
    }

    private void ScrollToBottom()
    {
        MainThread.BeginInvokeOnMainThread(() =>
            MessagesScroll.ScrollToAsync(0, double.MaxValue, false));
    }
}
