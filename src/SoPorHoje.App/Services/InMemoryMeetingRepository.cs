using SoPorHoje.App.Interfaces;
using SoPorHoje.App.Models;

namespace SoPorHoje.App.Services;

/// <summary>
/// Repositório em memória com dados de reuniões online de AA Brasil.
/// Em produção, substituir por implementação com banco de dados local.
/// </summary>
public class InMemoryMeetingRepository : IMeetingRepository
{
    private readonly List<OnlineMeeting> _meetings = new()
    {
        new OnlineMeeting
        {
            Id = 1,
            GroupName = "Grupo Serenidade Online",
            MeetingUrl = "https://meet.jit.si/grupo-serenidade-aa",
            DaysOfWeekMask = 62,   // Seg–Sex
            StartTime = new TimeSpan(7, 0, 0),
            EndTime = new TimeSpan(8, 0, 0),
            IsActive = true,
        },
        new OnlineMeeting
        {
            Id = 2,
            GroupName = "Grupo Esperança Brasil",
            MeetingUrl = "https://meet.jit.si/grupo-esperanca-aa",
            DaysOfWeekMask = 127,  // Todos os dias
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            IsActive = true,
        },
        new OnlineMeeting
        {
            Id = 3,
            GroupName = "Grupo Recuperação 24h",
            MeetingUrl = "https://meet.jit.si/grupo-recuperacao24",
            DaysOfWeekMask = 65,   // Dom + Sáb (bit 0 + bit 6)
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 30, 0),
            IsActive = true,
        },
        new OnlineMeeting
        {
            Id = 4,
            GroupName = "Grupo Novo Amanhecer",
            MeetingUrl = "https://meet.jit.si/grupo-novo-amanhecer",
            DaysOfWeekMask = 62,   // Seg–Sex
            StartTime = new TimeSpan(19, 30, 0),
            EndTime = new TimeSpan(20, 30, 0),
            IsActive = true,
        },
        new OnlineMeeting
        {
            Id = 5,
            GroupName = "Grupo Alvorada SP",
            MeetingUrl = "https://meet.jit.si/grupo-alvorada-sp",
            DaysOfWeekMask = 126,  // Seg–Sáb
            StartTime = new TimeSpan(21, 0, 0),
            EndTime = new TimeSpan(22, 0, 0),
            IsActive = true,
        },
    };

    public Task<IReadOnlyList<OnlineMeeting>> GetAllAsync()
        => Task.FromResult<IReadOnlyList<OnlineMeeting>>(_meetings.Where(m => m.IsActive).ToList());

    public Task<IReadOnlyList<OnlineMeeting>> GetLiveNowAsync()
        => Task.FromResult<IReadOnlyList<OnlineMeeting>>(_meetings.Where(m => m.IsActive && m.IsLiveNow).ToList());

    public Task<OnlineMeeting?> GetNextAsync()
    {
        var next = _meetings
            .Where(m => m.IsActive && m.MinutesUntilStart.HasValue)
            .OrderBy(m => m.MinutesUntilStart)
            .FirstOrDefault();
        return Task.FromResult(next);
    }

    public Task UpsertAsync(OnlineMeeting meeting)
    {
        var existing = _meetings.FindIndex(m => m.Id == meeting.Id);
        if (existing >= 0)
            _meetings[existing] = meeting;
        else
            _meetings.Add(meeting);
        return Task.CompletedTask;
    }
}
