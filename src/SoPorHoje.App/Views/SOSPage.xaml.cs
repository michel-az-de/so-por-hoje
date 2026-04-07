using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela SOS — acessível de qualquer lugar como modal.</summary>
public partial class SOSPage : ContentPage
{
    private readonly SOSViewModel _vm;

    public SOSPage(SOSViewModel viewModel)
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
