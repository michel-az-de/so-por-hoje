using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela de Reuniões Online com detecção de reunião ao vivo.</summary>
public partial class MeetingsPage : ContentPage
{
    private readonly MeetingsViewModel _vm;

    public MeetingsPage(MeetingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadMeetingsCommand.Execute(null);
        _vm.StartLiveTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.StopLiveTimer();
    }
}
