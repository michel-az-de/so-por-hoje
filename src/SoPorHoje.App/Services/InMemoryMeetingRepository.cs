using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.Services;

/// <summary>
/// Repositório em memória com dados de reuniões online de AA Brasil.
/// Usado como fallback quando não há dados sincronizados do servidor.
/// </summary>
public class InMemoryMeetingRepository : SoPorHoje.Core.Interfaces.IMeetingRepository
{
    private readonly List<OnlineMeeting> _meetings = new()
    {
        new OnlineMeeting
        {
            Id = 1,
            GroupName = "Grupo Serenidade Online",
            MeetingUrl = "https://meet.jit.si/grupo-serenidade-aa",
            DaysOfWeekMask = 62,   // Seg–Sex
            StartTimeTicks = new TimeSpan(7, 0, 0).Ticks,
            EndTimeTicks = new TimeSpan(8, 0, 0).Ticks,
        },
        new OnlineMeeting
        {
            Id = 2,
            GroupName = "Grupo Esperança Brasil",
            MeetingUrl = "https://meet.jit.si/grupo-esperanca-aa",
            DaysOfWeekMask = 127,  // Todos os dias
            StartTimeTicks = new TimeSpan(12, 0, 0).Ticks,
            EndTimeTicks = new TimeSpan(13, 0, 0).Ticks,
        },
        new OnlineMeeting
        {
            Id = 3,
            GroupName = "Grupo Recuperação 24h",
            MeetingUrl = "https://meet.jit.si/grupo-recuperacao24",
            DaysOfWeekMask = 65,   // Dom + Sáb
            StartTimeTicks = new TimeSpan(10, 0, 0).Ticks,
            EndTimeTicks = new TimeSpan(11, 30, 0).Ticks,
        },
        new OnlineMeeting
        {
            Id = 4,
            GroupName = "Grupo Novo Amanhecer",
            MeetingUrl = "https://meet.jit.si/grupo-novo-amanhecer",
            DaysOfWeekMask = 62,   // Seg–Sex
            StartTimeTicks = new TimeSpan(19, 30, 0).Ticks,
            EndTimeTicks = new TimeSpan(20, 30, 0).Ticks,
        },
        new OnlineMeeting
        {
            Id = 5,
            GroupName = "Grupo Alvorada SP",
            MeetingUrl = "https://meet.jit.si/grupo-alvorada-sp",
            DaysOfWeekMask = 126,  // Seg–Sáb
            StartTimeTicks = new TimeSpan(21, 0, 0).Ticks,
            EndTimeTicks = new TimeSpan(22, 0, 0).Ticks,
        },
    };

    public Task<List<OnlineMeeting>> GetAllAsync()
        => Task.FromResult(_meetings.ToList());

    public Task<List<OnlineMeeting>> GetLiveNowAsync()
        => Task.FromResult(_meetings.Where(m => m.IsLiveNow).ToList());

    public Task<OnlineMeeting?> GetNextAsync()
    {
        var next = _meetings
            .Where(m => m.MinutesUntilStart.HasValue)
            .OrderBy(m => m.MinutesUntilStart)
            .FirstOrDefault();
        return Task.FromResult(next);
    }

    public Task UpsertAsync(List<OnlineMeeting> meetings)
    {
        foreach (var meeting in meetings)
        {
            var idx = _meetings.FindIndex(m => m.Id == meeting.Id);
            if (idx >= 0)
                _meetings[idx] = meeting;
            else
                _meetings.Add(meeting);
        }
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetLastScrapeTimeAsync()
        => Task.FromResult<DateTime?>(null);
}
