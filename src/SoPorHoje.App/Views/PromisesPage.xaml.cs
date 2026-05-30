using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>As Promessas — o que a recuperação devolve.</summary>
public partial class PromisesPage : ContentPage
{
    public PromisesPage(PromisesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
