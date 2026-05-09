using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class HomePage : ContentPage
{
    public HomePage() : this(ServiceHelper.GetService<HomeViewModel>())
    {
    }

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
