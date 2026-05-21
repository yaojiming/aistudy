using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class WrongBookPage : ContentPage
{
    public WrongBookPage() : this(ServiceHelper.GetService<WrongBookViewModel>())
    {
    }

    public WrongBookPage(WrongBookViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
