using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Histórico — combinados e estatísticas pessoais.</summary>
public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    public HistoryPage(HistoryViewModel viewModel)
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
            // InitializeAsync trata erros internamente (RunSafeAsync); guard de async void.
        }
    }
}
