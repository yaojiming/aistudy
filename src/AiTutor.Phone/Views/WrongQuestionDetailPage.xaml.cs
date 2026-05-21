using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class WrongQuestionDetailPage : ContentPage
{
    public WrongQuestionDetailPage() : this(ServiceHelper.GetService<WrongQuestionDetailViewModel>())
    {
    }

    public WrongQuestionDetailPage(WrongQuestionDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
