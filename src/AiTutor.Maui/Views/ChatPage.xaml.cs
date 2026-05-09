using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class ChatPage : ContentPage
{
    public ChatPage() : this(ServiceHelper.GetService<ChatViewModel>())
    {
    }

    public ChatPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
