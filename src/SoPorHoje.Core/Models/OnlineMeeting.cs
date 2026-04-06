using SQLite;

namespace SoPorHoje.Core.Models;

public class OnlineMeeting
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string GroupName { get; set; } = string.Empty;

    /// <summary>Bitmask: Dom=1, Seg=2, Ter=4, Qua=8, Qui=16, Sex=32, Sáb=64.</summary>
    public int DaysOfWeekMask { get; set; }

    public long StartTimeTicks { get; set; }
    public long EndTimeTicks { get; set; }

    [Ignore] public TimeSpan StartTime => TimeSpan.FromTicks(StartTimeTicks);
    [Ignore] public TimeSpan EndTime => TimeSpan.FromTicks(EndTimeTicks);

    public string MeetingUrl { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Source { get; set; } = "intergrupos-aa.org.br";
    public DateTime LastScrapedAt { get; set; }

    [Ignore]
    public bool IsLiveNow
    {
        get
        {
            var now = DateTime.Now;
            var todayBit = 1 << (int)now.DayOfWeek;
            if ((DaysOfWeekMask & todayBit) == 0) return false;
            var t = now.TimeOfDay;
            return t >= StartTime && t < EndTime;
        }
    }

    [Ignore]
    public int? MinutesUntilStart
    {
        get
        {
            var now = DateTime.Now;
            var todayBit = 1 << (int)now.DayOfWeek;
            if ((DaysOfWeekMask & todayBit) == 0) return null;
            var diff = (int)(StartTime - now.TimeOfDay).TotalMinutes;
            return diff > 0 ? diff : null;
        }
    }
}
