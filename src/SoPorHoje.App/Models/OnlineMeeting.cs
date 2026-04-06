namespace SoPorHoje.App.Models;

/// <summary>Reunião online de AA com informações de horário e link de acesso.</summary>
public class OnlineMeeting
{
    public int Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string MeetingUrl { get; set; } = string.Empty;

    /// <summary>Bitmask dias da semana: bit 0=Dom, 1=Seg, ..., 6=Sáb.</summary>
    public int DaysOfWeekMask { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Retorna true se a reunião está acontecendo agora.</summary>
    public bool IsLiveNow
    {
        get
        {
            var now = DateTime.Now;
            var dayBit = 1 << (int)now.DayOfWeek;
            if ((DaysOfWeekMask & dayBit) == 0) return false;

            var time = now.TimeOfDay;
            return time >= StartTime && time <= EndTime;
        }
    }

    /// <summary>Minutos até o próximo início, ou null se já passou hoje ou ao vivo.</summary>
    public int? MinutesUntilStart
    {
        get
        {
            if (IsLiveNow) return null;

            var now = DateTime.Now;
            var dayBit = 1 << (int)now.DayOfWeek;
            if ((DaysOfWeekMask & dayBit) != 0 && now.TimeOfDay < StartTime)
            {
                return (int)(StartTime - now.TimeOfDay).TotalMinutes;
            }

            return null;
        }
    }
}
