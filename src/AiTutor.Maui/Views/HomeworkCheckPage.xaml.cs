using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class HomeworkCheckPage : ContentPage
{
    public HomeworkCheckPage() : this(ServiceHelper.GetService<HomeworkCheckViewModel>())
    {
    }

    public HomeworkCheckPage(HomeworkCheckViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
