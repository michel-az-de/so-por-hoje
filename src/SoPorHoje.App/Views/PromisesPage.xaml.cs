using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>As Promessas do A.A.</summary>
public partial class PromisesPage : ContentPage
{
    public PromisesPage(PromisesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
