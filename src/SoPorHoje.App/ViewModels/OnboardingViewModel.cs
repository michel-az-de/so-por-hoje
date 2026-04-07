using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel de Onboarding — exibido na primeira abertura do app.</summary>
public partial class OnboardingViewModel : BaseViewModel
{
    private readonly IUserRepository _userRepo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _maximumDate = DateTime.Today;

    public bool CanStart => SelectedDate <= DateTime.Today;

    public OnboardingViewModel(IUserRepository userRepo)
    {
        _userRepo = userRepo;
        Title = "Bem-vindo";
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        await RunSafeAsync(async () =>
        {
            var profile = new UserProfile
            {
                SobrietyDate = SelectedDate.Date,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            await _userRepo.SaveProfileAsync(profile);

            // Navigate to AppShell after onboarding is complete.
            // Shell.Current is unavailable here because MainPage is a NavigationPage (not Shell),
            // so we resolve AppShell from the application's DI container.
            if (Application.Current is not null)
            {
                var services = Application.Current.Handler?.MauiContext?.Services;
                var shell = services?.GetService<AppShell>()
                            ?? throw new InvalidOperationException("AppShell not registered in DI container.");
                Application.Current.MainPage = shell;
            }
        });
    }
}
