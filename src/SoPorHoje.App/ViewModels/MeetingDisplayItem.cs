using CommunityToolkit.Mvvm.ComponentModel;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>
/// Wrapper de apresentação para <see cref="OnlineMeeting"/> com status ao vivo reativo.
/// </summary>
public partial class MeetingDisplayItem : ObservableObject
{
    public OnlineMeeting Meeting { get; }

    [ObservableProperty]
    private bool _isLive;

    [ObservableProperty]
    private int? _minutesUntil;

    [ObservableProperty]
    private string _daysLabel = string.Empty;

    [ObservableProperty]
    private string _timeLabel = string.Empty;

    public MeetingDisplayItem(OnlineMeeting m)
    {
        Meeting = m;
        DaysLabel = FormatDays(m.DaysOfWeekMask);
        TimeLabel = $"{m.StartTime:hh\\:mm}–{m.EndTime:hh\\:mm}";
        RefreshLiveStatus();
    }

    public void RefreshLiveStatus()
    {
        IsLive = Meeting.IsLiveNow;
        MinutesUntil = Meeting.MinutesUntilStart;
    }

    private static string FormatDays(int mask)
    {
        if (mask == 127) return "Todos os dias";
        if (mask == 62) return "Seg–Sex";
        if (mask == 126) return "Seg–Sáb";
        if (mask == 65) return "Dom e Sáb";

        var names = new[] { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };
        var days = new List<string>();
        for (int i = 0; i < 7; i++)
            if ((mask & (1 << i)) != 0)
                days.Add(names[i]);
        return string.Join(", ", days);
    }
}


    [ObservableProperty]
    private bool _isLive;

    [ObservableProperty]
    private int? _minutesUntil;

    [ObservableProperty]
    private string _daysLabel = string.Empty;

    [ObservableProperty]
    private string _timeLabel = string.Empty;

    public MeetingDisplayItem(OnlineMeeting m)
    {
        Meeting = m;
        DaysLabel = FormatDays(m.DaysOfWeekMask);
        TimeLabel = $"{m.StartTime:hh\\:mm}–{m.EndTime:hh\\:mm}";
        RefreshLiveStatus();
    }

    public void RefreshLiveStatus()
    {
        IsLive = Meeting.IsLiveNow;
        MinutesUntil = Meeting.MinutesUntilStart;
    }

    private static string FormatDays(int mask)
    {
        if (mask == 127) return "Todos os dias";
        if (mask == 62) return "Seg–Sex";
        if (mask == 126) return "Seg–Sáb";
        if (mask == 65) return "Dom e Sáb";

        var names = new[] { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };
        var days = new List<string>();
        for (int i = 0; i < 7; i++)
            if ((mask & (1 << i)) != 0)
                days.Add(names[i]);
        return string.Join(", ", days);
    }
}
