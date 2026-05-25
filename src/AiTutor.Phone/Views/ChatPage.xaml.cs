using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;
using System.Collections.Specialized;

namespace AiTutor.Maui.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;
    private bool _isHoldVoiceTouchActive;
    private double _holdVoiceStartY;

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

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.LeavePageAsync();
    }

    private async void OnHoldToTalkStartInteraction(object? sender, TouchEventArgs e)
    {
        if (_isHoldVoiceTouchActive)
        {
            return;
        }

        _isHoldVoiceTouchActive = true;
        _holdVoiceStartY = e.Touches.FirstOrDefault().Y;
        await _viewModel.BeginHoldVoiceRecordAsync();
    }

    private void OnHoldToTalkDragInteraction(object? sender, TouchEventArgs e)
    {
        var touch = e.Touches.FirstOrDefault();
        _viewModel.UpdateHoldVoiceCancelState(touch.Y - _holdVoiceStartY);
    }

    private async void OnHoldToTalkEndInteraction(object? sender, TouchEventArgs e)
    {
        if (!_isHoldVoiceTouchActive)
        {
            return;
        }

        _isHoldVoiceTouchActive = false;
        await _viewModel.FinishHoldVoiceRecordAsync();
    }

    private async void OnHoldToTalkCancelInteraction(object? sender, EventArgs e)
    {
        if (!_isHoldVoiceTouchActive)
        {
            return;
        }

        _isHoldVoiceTouchActive = false;
        await _viewModel.CancelHoldVoiceRecordAsync();
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
