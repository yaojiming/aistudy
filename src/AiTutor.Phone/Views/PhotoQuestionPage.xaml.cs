using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class PhotoQuestionPage : ContentPage
{
    public PhotoQuestionPage() : this(ServiceHelper.GetService<PhotoQuestionViewModel>())
    {
    }

    public PhotoQuestionPage(PhotoQuestionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
