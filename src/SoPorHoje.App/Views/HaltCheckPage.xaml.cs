using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Ferramenta interativa Check HALT.</summary>
public partial class HaltCheckPage : ContentPage
{
    private readonly HaltCheckViewModel _vm;

    public HaltCheckPage(HaltCheckViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    private void OnHaltCardTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string indexStr && int.TryParse(indexStr, out var index))
        {
            var item = _vm.Items[index];
            item.IsExpanded = !item.IsExpanded;
        }
    }
}
