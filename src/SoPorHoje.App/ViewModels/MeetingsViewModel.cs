using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.App.Interfaces;
using SoPorHoje.App.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel da tela de Reuniões Online com atualização ao vivo a cada 30 s.</summary>
public partial class MeetingsViewModel : BaseViewModel
{
    private readonly IMeetingRepository _meetingRepo;
    private IDispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private ObservableCollection<MeetingDisplayItem> _meetings = new();

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasAnyMeetings;

    public MeetingsViewModel(IMeetingRepository meetingRepo)
    {
        _meetingRepo = meetingRepo;
        Title = "Reuniões Online";
    }

    [RelayCommand]
    private async Task LoadMeetingsAsync()
    {
        await RunSafeAsync(async () =>
        {
            var all = await _meetingRepo.GetAllAsync();
            var sorted = all
                .OrderByDescending(m => m.IsLiveNow)
                .ThenBy(m => m.StartTime)
                .Select(m => new MeetingDisplayItem(m))
                .ToList();

            Meetings = new ObservableCollection<MeetingDisplayItem>(sorted);
            HasAnyMeetings = Meetings.Any();
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadMeetingsAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private static async Task JoinMeetingAsync(MeetingDisplayItem item)
    {
        if (!string.IsNullOrWhiteSpace(item.Meeting.MeetingUrl))
            await Launcher.OpenAsync(new Uri(item.Meeting.MeetingUrl));
    }

    [RelayCommand]
    private static async Task OpenLinkAsync(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
            await Launcher.OpenAsync(new Uri(url));
    }

    /// <summary>Inicia o timer que atualiza o status "ao vivo" a cada 30 segundos.</summary>
    public void StartLiveTimer()
    {
        StopLiveTimer();

        _refreshTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_refreshTimer is null) return;

        _refreshTimer.Interval = TimeSpan.FromSeconds(30);
        _refreshTimer.Tick += OnTimerTick;
        _refreshTimer.Start();
    }

    public void StopLiveTimer()
    {
        if (_refreshTimer is null) return;
        _refreshTimer.Tick -= OnTimerTick;
        _refreshTimer.Stop();
        _refreshTimer = null;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        foreach (var m in Meetings)
            m.RefreshLiveStatus();

        var reordered = Meetings
            .OrderByDescending(m => m.IsLive)
            .ThenBy(m => m.Meeting.StartTime)
            .ToList();

        Meetings = new ObservableCollection<MeetingDisplayItem>(reordered);
        HasAnyMeetings = Meetings.Any();
    }
}
