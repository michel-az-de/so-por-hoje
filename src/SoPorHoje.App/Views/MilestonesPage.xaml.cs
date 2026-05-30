using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Marcos — fichas de tempo de recuperação.</summary>
public partial class MilestonesPage : ContentPage
{
    private readonly MilestonesViewModel _viewModel;

    public MilestonesPage(MilestonesViewModel viewModel)
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
