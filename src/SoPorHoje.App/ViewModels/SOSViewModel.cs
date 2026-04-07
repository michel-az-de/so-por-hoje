using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.Core.Interfaces;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel da tela de Emergência (SOS) com respiração guiada, frases de enfrentamento e HALT.</summary>
public partial class SOSViewModel : BaseViewModel
{
    private readonly IMeetingRepository _meetingRepo;
    private CancellationTokenSource? _breathingCts;

    // ── Breathing ─────────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _isBreathingActive;

    [ObservableProperty]
    private string _breathingInstruction = "Inspire";

    [ObservableProperty]
    private int _breathingSeconds;

    [ObservableProperty]
    private int _breathingCycle;

    [ObservableProperty]
    private bool _breathingComplete;

    // ── Live Meeting ───────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _hasLiveMeeting;

    [ObservableProperty]
    private string _liveMeetingUrl = string.Empty;

    // ── HALT ───────────────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _isHungry;

    [ObservableProperty]
    private bool _isAngry;

    [ObservableProperty]
    private bool _isLonely;

    [ObservableProperty]
    private bool _isTired;

    public IReadOnlyList<string> CopingPhrases { get; } = new List<string>
    {
        "Isso também vai passar.",
        "Um dia de cada vez. Uma hora de cada vez.",
        "Eu sou mais forte do que essa vontade.",
        "Ligo para meu padrinho/madrinha agora.",
        "Não vou beber hoje, não importa o que aconteça.",
        "Minha família precisa de mim sóbrio.",
        "O desejo passa em cerca de 20 minutos.",
        "Cada vez que recuso, fico mais forte.",
    };

    public SOSViewModel(IMeetingRepository meetingRepo)
    {
        _meetingRepo = meetingRepo;
        Title = "SOS";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var live = await _meetingRepo.GetLiveNowAsync();
            HasLiveMeeting = live.Count > 0;
            LiveMeetingUrl = live.Count > 0 ? live[0].MeetingUrl : string.Empty;
        });
    }

    [RelayCommand]
    private async Task StartBreathingAsync()
    {
        _breathingCts?.Cancel();
        _breathingCts = new CancellationTokenSource();
        var ct = _breathingCts.Token;

        IsBreathingActive = true;
        BreathingComplete = false;

        try
        {
            for (int cycle = 1; cycle <= 4 && !ct.IsCancellationRequested; cycle++)
            {
                BreathingCycle = cycle;

                // Inspire — 4 seconds
                BreathingInstruction = "Inspire...";
                for (int s = 4; s >= 1 && !ct.IsCancellationRequested; s--)
                {
                    BreathingSeconds = s;
                    await Task.Delay(1000, ct);
                }

                // Hold — 4 seconds
                BreathingInstruction = "Segure...";
                for (int s = 4; s >= 1 && !ct.IsCancellationRequested; s--)
                {
                    BreathingSeconds = s;
                    await Task.Delay(1000, ct);
                }

                // Expire — 6 seconds
                BreathingInstruction = "Expire...";
                for (int s = 6; s >= 1 && !ct.IsCancellationRequested; s--)
                {
                    BreathingSeconds = s;
                    await Task.Delay(1000, ct);
                }
            }

            if (!ct.IsCancellationRequested)
            {
                BreathingInstruction = "Muito bem 🌿";
                BreathingSeconds = 0;
                BreathingComplete = true;
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled — ignore
        }
        finally
        {
            IsBreathingActive = false;
        }
    }

    [RelayCommand]
    private void StopBreathing()
    {
        _breathingCts?.Cancel();
        IsBreathingActive = false;
        BreathingComplete = false;
    }

    [RelayCommand]
    private void ToggleHungry() => IsHungry = !IsHungry;

    [RelayCommand]
    private void ToggleAngry() => IsAngry = !IsAngry;

    [RelayCommand]
    private void ToggleLonely() => IsLonely = !IsLonely;

    [RelayCommand]
    private void ToggleTired() => IsTired = !IsTired;

    [RelayCommand]
    private static async Task CallCvvAsync()
    {
        await Launcher.OpenAsync(new Uri("tel:188"));
    }

    [RelayCommand]
    private static async Task JoinMeetingAsync(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
            await Launcher.OpenAsync(new Uri(url));
    }
}
