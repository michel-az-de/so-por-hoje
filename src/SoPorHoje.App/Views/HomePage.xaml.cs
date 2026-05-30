using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela "Hoje" — onboarding no primeiro uso e o painel diário.</summary>
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch
        {
            // InitializeAsync já trata e registra erros internamente (RunSafeAsync);
            // este guard evita que uma exceção escape de um async void e derrube a UI.
        }
    }
}
