using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class HomeworkCheckResultPage : ContentPage
{
    public HomeworkCheckResultPage() : this(ServiceHelper.GetService<HomeworkCheckViewModel>())
    {
    }

    public HomeworkCheckResultPage(HomeworkCheckViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
