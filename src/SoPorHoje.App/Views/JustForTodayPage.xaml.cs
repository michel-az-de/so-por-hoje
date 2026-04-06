using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Meditações Só Por Hoje.</summary>
public partial class JustForTodayPage : ContentPage
{
    public JustForTodayPage(JustForTodayViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
