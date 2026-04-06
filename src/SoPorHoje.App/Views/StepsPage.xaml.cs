using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Os 12 Passos do A.A.</summary>
public partial class StepsPage : ContentPage
{
    public StepsPage(StepsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
