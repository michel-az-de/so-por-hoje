using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>
/// Tela "Hoje": onboarding no primeiro uso e, depois, o painel diário —
/// contador de recuperação, marco/ficha, reflexão do dia e o combinado de hoje.
/// Tudo offline (SQLite é a fonte da verdade).
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    private readonly IUserRepository _users;
    private readonly IPledgeRepository _pledges;
    private readonly IReflectionRepository _reflections;
    private readonly IChipService _chips;

    public HomeViewModel(IUserRepository users, IPledgeRepository pledges,
                         IReflectionRepository reflections, IChipService chips)
    {
        _users = users;
        _pledges = pledges;
        _reflections = reflections;
        _chips = chips;
        Title = "Hoje";
    }

    // ── Estado de tela ─────────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowDashboard))]
    private bool _needsOnboarding;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowDashboard))]
    private bool _isLoaded;

    public bool ShowDashboard => IsLoaded && !NeedsOnboarding;

    // ── Onboarding ─────────────────────────────────────────────────────────
    [ObservableProperty] private DateTime _onboardingDate = DateTime.Today;
    [ObservableProperty] private string _onboardingReason = string.Empty;

    // ── Painel ─────────────────────────────────────────────────────────────
    [ObservableProperty] private int _soberDays;
    [ObservableProperty] private string _soberDaysLabel = string.Empty;
    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private string? _personalReason;
    [ObservableProperty] private bool _hasPersonalReason;

    [ObservableProperty] private string _chipEmoji = string.Empty;
    [ObservableProperty] private string _chipName = string.Empty;
    [ObservableProperty] private double _chipProgress;
    [ObservableProperty] private string _nextChipText = string.Empty;

    // ── Reflexão ───────────────────────────────────────────────────────────
    [ObservableProperty] private string _reflectionTitle = string.Empty;
    [ObservableProperty] private string _reflectionQuote = string.Empty;
    [ObservableProperty] private string _reflectionText = string.Empty;
    [ObservableProperty] private bool _hasReflection;

    // ── Combinado de hoje ──────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNotPledged))]
    private bool _hasPledgedToday;

    public bool HasNotPledged => !HasPledgedToday;

    public Task InitializeAsync() => RunSafeAsync(LoadAsync);

    private async Task LoadAsync()
    {
        await EnsureReflectionsSeededAsync();

        var profile = await _users.GetProfileAsync();
        if (profile is null)
        {
            NeedsOnboarding = true;
            IsLoaded = true;
            return;
        }

        NeedsOnboarding = false;

        SoberDays = profile.SoberDays;
        SoberDaysLabel = SoberDays == 1 ? "1 dia" : $"{SoberDays} dias";
        Greeting = BuildGreeting();
        PersonalReason = profile.PersonalReason;
        HasPersonalReason = !string.IsNullOrWhiteSpace(profile.PersonalReason);

        var current = _chips.GetCurrentChip(SoberDays);
        ChipEmoji = current.Emoji;
        ChipName = current.Label;
        ChipProgress = _chips.GetProgressToNext(SoberDays);
        var next = _chips.GetNextChip(SoberDays);
        NextChipText = next is null
            ? "Você alcançou todos os marcos. 💛"
            : $"Faltam {_chips.GetDaysUntilNext(SoberDays)} dias para {next.Label}.";

        await LoadReflectionAsync();
        await LoadPledgeAsync();

        IsLoaded = true;
    }

    private async Task EnsureReflectionsSeededAsync()
    {
        try
        {
            if (await _reflections.GetCountAsync() > 0) return;
            using var stream = await FileSystem.OpenAppPackageFileAsync("reflections-pt-BR.json");
            await _reflections.SeedFromJsonAsync(stream);
        }
        catch (Exception ex)
        {
            // Seed é best-effort: sem reflexões empacotadas, o app segue sem a reflexão do dia.
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] Falha ao semear reflexões: {ex}");
        }
    }

    private async Task LoadReflectionAsync()
    {
        var r = await _reflections.GetTodaysReflectionAsync();
        if (r is null)
        {
            // Ainda não há reflexão para a data exata: mostra uma do acervo disponível.
            var all = await _reflections.GetAllAsync();
            if (all.Count > 0)
                r = all[DateTime.Now.DayOfYear % all.Count];
        }

        HasReflection = r is not null;
        if (r is not null)
        {
            ReflectionTitle = r.Title;
            ReflectionQuote = r.Quote;
            ReflectionText = r.Text;
        }
    }

    private async Task LoadPledgeAsync()
    {
        var pledge = await _pledges.GetTodaysPledgeAsync();
        HasPledgedToday = pledge is not null;
    }

    private static string BuildGreeting()
    {
        var hour = DateTime.Now.Hour;
        if (hour < 12) return "Bom dia.";
        if (hour < 18) return "Boa tarde.";
        return "Boa noite.";
    }

    [RelayCommand]
    private Task SaveOnboardingAsync() => RunSafeAsync(async () =>
    {
        var profile = new UserProfile
        {
            SobrietyDate = OnboardingDate.Date,
            PersonalReason = string.IsNullOrWhiteSpace(OnboardingReason) ? null : OnboardingReason.Trim(),
        };
        await _users.SaveProfileAsync(profile);
        await LoadAsync();
    });

    [RelayCommand]
    private Task MakePledgeAsync() => RunSafeAsync(async () =>
    {
        if (await _pledges.GetTodaysPledgeAsync() is not null)
        {
            HasPledgedToday = true;
            return;
        }

        await _pledges.SavePledgeAsync(new DailyPledge
        {
            PledgeDate = DateTime.Today,
            PledgedAt = DateTime.Now,
        });
        HasPledgedToday = true;
    });
}
