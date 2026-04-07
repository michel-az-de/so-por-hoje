using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela de fichas de sobriedade com sistema de celebração.</summary>
public partial class ChipsPage : ContentPage
{
    private readonly ChipsViewModel _vm;

    public ChipsPage(ChipsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }
}
