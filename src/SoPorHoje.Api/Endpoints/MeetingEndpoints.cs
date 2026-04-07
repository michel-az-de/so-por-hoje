using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.DTOs;

namespace SoPorHoje.Api.Endpoints;

public static class MeetingEndpoints
{
    public static void MapMeetingEndpoints(this WebApplication app)
    {
        app.MapGet("/api/meetings", async (AppDbContext db) =>
        {
            var meetings = await db.Meetings
                .Where(m => m.IsActive)
                .OrderBy(m => m.StartTime)
                .ThenBy(m => m.GroupName)
                .ToListAsync();

            var now = SyncEndpoints.GetBrasiliaTime();
            var dtos = meetings.Select(m => SyncEndpoints.MapMeetingDto(m, now));
            return Results.Ok(dtos);
        })
        .WithName("GetMeetings")
        .WithSummary("Lista todas as reuniões ativas")
        .WithTags("Meetings")
        .Produces<IEnumerable<MeetingDto>>(StatusCodes.Status200OK);

        app.MapGet("/api/meetings/live", async (AppDbContext db) =>
        {
            var now = SyncEndpoints.GetBrasiliaTime();
            var brasiliaDay = TimeZoneInfo.ConvertTime(
                DateTimeOffset.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")).DayOfWeek;

            int todayBit = brasiliaDay switch
            {
                DayOfWeek.Sunday    => 1 << 0,
                DayOfWeek.Monday    => 1 << 1,
                DayOfWeek.Tuesday   => 1 << 2,
                DayOfWeek.Wednesday => 1 << 3,
                DayOfWeek.Thursday  => 1 << 4,
                DayOfWeek.Friday    => 1 << 5,
                DayOfWeek.Saturday  => 1 << 6,
                _ => 0
            };

            var meetings = await db.Meetings
                .Where(m => m.IsActive
                    && (m.DaysOfWeekMask & todayBit) != 0
                    && m.StartTime <= now
                    && m.EndTime >= now)
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            var dtos = meetings.Select(m => SyncEndpoints.MapMeetingDto(m, now));
            return Results.Ok(dtos);
        })
        .WithName("GetLiveMeetings")
        .WithSummary("Retorna reuniões acontecendo agora (horário de Brasília)")
        .WithTags("Meetings")
        .Produces<IEnumerable<MeetingDto>>(StatusCodes.Status200OK);
    }
}
