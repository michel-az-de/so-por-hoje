using SoPorHoje.App.ViewModels;

namespace SoPorHoje.App.Views;

/// <summary>Tela de onboarding exibida apenas na primeira abertura do app.</summary>
public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
