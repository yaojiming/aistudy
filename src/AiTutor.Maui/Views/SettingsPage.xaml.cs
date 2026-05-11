using AiTutor.Maui.Services;
using AiTutor.Maui.ViewModels;

namespace AiTutor.Maui.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage() : this(ServiceHelper.GetService<SettingsViewModel>())
    {
    }

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
