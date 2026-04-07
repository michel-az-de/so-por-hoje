using SoPorHoje.App.Views;
using SoPorHoje.Core.Interfaces;

namespace SoPorHoje.App;

public partial class App : Application
{
    private readonly IUserRepository _userRepo;
    private readonly AppShell _shell;
    private readonly OnboardingPage _onboardingPage;

    public App(AppShell shell, OnboardingPage onboardingPage, IUserRepository userRepo)
    {
        InitializeComponent();
        _shell = shell;
        _onboardingPage = onboardingPage;
        _userRepo = userRepo;
        MainPage = new ContentPage(); // Temporary blank page shown briefly while OnStart() checks user profile
    }

    protected override async void OnStart()
    {
        base.OnStart();
        var hasProfile = await _userRepo.HasProfileAsync();
        MainPage = hasProfile ? _shell : (Page)new NavigationPage(_onboardingPage);
    }
}
