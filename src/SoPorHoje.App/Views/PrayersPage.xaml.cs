using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Meditações com cópia para a área de transferência.</summary>
public partial class PrayersPage : ContentPage
{
    public PrayersPage(PrayersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
