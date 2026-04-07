using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel da tela inicial com contador de sobriedade, compromisso diário e reflexão.</summary>
public partial class HomeViewModel : BaseViewModel
{
    private readonly IUserRepository _userRepo;
    private readonly IPledgeRepository _pledgeRepo;
    private readonly IReflectionRepository _reflectionRepo;
    private readonly IMeetingRepository _meetingRepo;
    private readonly IChipService _chipService;
    private IDispatcherTimer? _liveTimer;

    // ── Sobriety ────────────────────────────────────────────────────────────

    [ObservableProperty]
    private int _soberDays;

    [ObservableProperty]
    private string _soberDaysText = string.Empty;

    [ObservableProperty]
    private string _currentChipEmoji = "🌅";

    [ObservableProperty]
    private string _currentChipName = string.Empty;

    [ObservableProperty]
    private string _currentChipColor = "#D4A017";

    [ObservableProperty]
    private double _chipProgress;

    [ObservableProperty]
    private string _daysUntilNextChipText = string.Empty;

    // ── Pledge ───────────────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPledgedToday))]
    private bool _hasPledgedToday;

    // ── Personal Reason ─────────────────────────────────────────────────────

    [ObservableProperty]
    private string? _personalReason;

    [ObservableProperty]
    private bool _isEditingReason;

    [ObservableProperty]
    private string _editingReasonText = string.Empty;

    // ── Reflection ───────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _reflectionTitle = string.Empty;

    [ObservableProperty]
    private string _reflectionQuote = string.Empty;

    [ObservableProperty]
    private string _reflectionText = string.Empty;

    [ObservableProperty]
    private string _reflectionReference = string.Empty;

    [ObservableProperty]
    private bool _isReflectionExpanded;

    [ObservableProperty]
    private bool _hasReflection;

    // ── Live Meeting ──────────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _hasLiveMeeting;

    [ObservableProperty]
    private string _liveMeetingName = string.Empty;

    [ObservableProperty]
    private string _liveMeetingUrl = string.Empty;

    [ObservableProperty]
    private string _nextMeetingText = string.Empty;

    public HomeViewModel(
        IUserRepository userRepo,
        IPledgeRepository pledgeRepo,
        IReflectionRepository reflectionRepo,
        IMeetingRepository meetingRepo,
        IChipService chipService)
    {
        _userRepo = userRepo;
        _pledgeRepo = pledgeRepo;
        _reflectionRepo = reflectionRepo;
        _meetingRepo = meetingRepo;
        _chipService = chipService;
        Title = "Início";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            await LoadProfileAsync();
            await LoadPledgeAsync();
            await LoadReflectionAsync();
            await LoadLiveMeetingAsync();
        });
    }

    private async Task LoadProfileAsync()
    {
        var profile = await _userRepo.GetProfileAsync();
        if (profile is null) return;

        SoberDays = profile.SoberDays;
        PersonalReason = profile.PersonalReason;

        var months = SoberDays / 30;
        SoberDaysText = months > 0
            ? $"{SoberDays} dias sóbrios · {months} {(months == 1 ? "mês" : "meses")}"
            : $"{SoberDays} {(SoberDays == 1 ? "dia sóbrio" : "dias sóbrios")}";

        var chip = _chipService.GetCurrentChip(SoberDays);
        CurrentChipEmoji = chip.Emoji;
        CurrentChipName = $"{chip.Emoji} {chip.Label}";
        CurrentChipColor = chip.ColorHex;
        ChipProgress = _chipService.GetProgressToNext(SoberDays);

        var daysUntil = _chipService.GetDaysUntilNext(SoberDays);
        DaysUntilNextChipText = daysUntil > 0 ? $"Faltam {daysUntil} dias" : "Última ficha conquistada! 🏆";
    }

    private async Task LoadPledgeAsync()
    {
        var pledge = await _pledgeRepo.GetTodaysPledgeAsync();
        HasPledgedToday = pledge is not null;
    }

    private async Task LoadReflectionAsync()
    {
        var reflection = await _reflectionRepo.GetTodaysReflectionAsync();
        if (reflection is null)
        {
            HasReflection = false;
            return;
        }

        HasReflection = true;
        ReflectionTitle = reflection.Title;
        ReflectionQuote = reflection.Quote;
        ReflectionText = reflection.Text;
        ReflectionReference = reflection.Reference;
    }

    private async Task LoadLiveMeetingAsync()
    {
        var live = await _meetingRepo.GetLiveNowAsync();
        if (live.Count > 0)
        {
            HasLiveMeeting = true;
            LiveMeetingName = live[0].GroupName;
            LiveMeetingUrl = live[0].MeetingUrl;
            NextMeetingText = string.Empty;
        }
        else
        {
            HasLiveMeeting = false;
            LiveMeetingName = string.Empty;
            LiveMeetingUrl = string.Empty;

            var next = await _meetingRepo.GetNextAsync();
            NextMeetingText = next is not null && next.MinutesUntilStart.HasValue
                ? $"Próxima: {next.GroupName} em {next.MinutesUntilStart} min"
                : string.Empty;
        }
    }

    [RelayCommand]
    private async Task MakePledgeAsync()
    {
        await RunSafeAsync(async () =>
        {
            var pledge = new DailyPledge
            {
                PledgeDate = DateTime.Today,
                PledgedAt = DateTime.UtcNow,
            };
            await _pledgeRepo.SavePledgeAsync(pledge);
            HasPledgedToday = true;
        });
    }

    [RelayCommand]
    private void StartEditingReason()
    {
        EditingReasonText = PersonalReason ?? string.Empty;
        IsEditingReason = true;
    }

    [RelayCommand]
    private void CancelEditingReason()
    {
        IsEditingReason = false;
        EditingReasonText = string.Empty;
    }

    [RelayCommand]
    private async Task SaveReasonAsync()
    {
        if (string.IsNullOrWhiteSpace(EditingReasonText)) return;

        await RunSafeAsync(async () =>
        {
            var profile = await _userRepo.GetProfileAsync();
            if (profile is null) return;

            profile.PersonalReason = EditingReasonText.Trim();
            profile.UpdatedAt = DateTime.UtcNow;
            await _userRepo.SaveProfileAsync(profile);

            PersonalReason = profile.PersonalReason;
            IsEditingReason = false;
            EditingReasonText = string.Empty;
        });
    }

    [RelayCommand]
    private void ToggleReflection()
    {
        IsReflectionExpanded = !IsReflectionExpanded;
    }

    [RelayCommand]
    private static async Task JoinLiveMeetingAsync(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
            await Launcher.OpenAsync(new Uri(url));
    }

    /// <summary>Inicia o timer que recalcula reunião ao vivo a cada 30 segundos.</summary>
    public void StartLiveTimer()
    {
        StopLiveTimer();
        _liveTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_liveTimer is null) return;
        _liveTimer.Interval = TimeSpan.FromSeconds(30);
        _liveTimer.Tick += async (_, _) => await LoadLiveMeetingAsync();
        _liveTimer.Start();
    }

    public void StopLiveTimer()
    {
        if (_liveTimer is null) return;
        _liveTimer.Stop();
        _liveTimer = null;
    }
}
