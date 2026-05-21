using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class HomeworkCheckImagePage : ContentPage
{
    public HomeworkCheckImagePage() : this(ServiceHelper.GetService<HomeworkCheckViewModel>())
    {
    }

    public HomeworkCheckImagePage(HomeworkCheckViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
