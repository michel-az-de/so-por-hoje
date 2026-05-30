using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Combinados — compromissos diários.</summary>
public partial class TraditionsPage : ContentPage
{
    public TraditionsPage(TraditionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
