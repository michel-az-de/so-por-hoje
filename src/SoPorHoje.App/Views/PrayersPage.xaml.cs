using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Orações do A.A. com cópia para clipboard.</summary>
public partial class PrayersPage : ContentPage
{
    public PrayersPage(PrayersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
