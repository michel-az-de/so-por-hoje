using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>As 12 Tradições do A.A.</summary>
public partial class TraditionsPage : ContentPage
{
    public TraditionsPage(TraditionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
