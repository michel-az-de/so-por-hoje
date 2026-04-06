using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Hub do Programa de A.A.</summary>
public partial class ProgramPage : ContentPage
{
    public ProgramPage(ProgramViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
