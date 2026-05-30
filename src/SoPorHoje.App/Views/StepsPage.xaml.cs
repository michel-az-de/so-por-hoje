using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>A Trilha — princípios de recuperação.</summary>
public partial class StepsPage : ContentPage
{
    public StepsPage(StepsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
