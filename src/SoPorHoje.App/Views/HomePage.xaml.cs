using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela inicial com contador de sobriedade, compromisso diário e reflexão do dia.</summary>
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
        _vm.StartLiveTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.StopLiveTimer();
    }
}
