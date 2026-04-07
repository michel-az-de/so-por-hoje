using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.DTOs;
using SoPorHoje.Api.Services;

namespace SoPorHoje.Api.Endpoints;

public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this WebApplication app)
    {
        app.MapPost("/api/sync/push", async (SyncPushRequest request, SyncService sync) =>
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return Results.BadRequest(new { error = "deviceId é obrigatório" });

            var response = await sync.PushAsync(request);
            return Results.Ok(response);
        })
        .WithName("SyncPush")
        .WithSummary("Recebe dados locais do app e persiste no servidor")
        .WithTags("Sync")
        .Produces<SyncPushResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapGet("/api/sync/pull", async (AppDbContext db, string? since) =>
        {
            DateTimeOffset? sinceTs = null;
            if (!string.IsNullOrWhiteSpace(since) && DateTimeOffset.TryParse(since, out var parsed))
                sinceTs = parsed;

            var meetings = await db.Meetings
                .Where(m => m.IsActive && (sinceTs == null || m.UpdatedAt >= sinceTs))
                .OrderBy(m => m.GroupName)
                .ToListAsync();

            var now = GetBrasiliaTime();
            var meetingDtos = meetings.Select(m => MapMeetingDto(m, now)).ToList();

            return Results.Ok(new SyncPullResponse(meetingDtos, false, DateTimeOffset.UtcNow));
        })
        .WithName("SyncPull")
        .WithSummary("Retorna dados atualizados desde o timestamp informado")
        .WithTags("Sync")
        .Produces<SyncPullResponse>(StatusCodes.Status200OK);

        app.MapDelete("/api/users/{deviceId}", async (string deviceId, SyncService sync) =>
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                return Results.BadRequest(new { error = "deviceId é obrigatório" });

            await sync.DeleteUserAsync(deviceId);
            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .WithSummary("Remove permanentemente todos os dados do usuário")
        .WithTags("Users")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static MeetingDto MapMeetingDto(Data.Entities.MeetingEntity m, TimeOnly now)
    {
        var isLiveNow = IsLiveNow(m.DaysOfWeekMask, m.StartTime, m.EndTime, now);
        return new MeetingDto(
            m.Id, m.GroupName, m.DaysOfWeekMask,
            m.StartTime.ToString("HH:mm:ss"),
            m.EndTime.ToString("HH:mm:ss"),
            m.MeetingUrl, m.Platform, m.Source, isLiveNow);
    }

    private static bool IsLiveNow(int mask, TimeOnly start, TimeOnly end, TimeOnly now)
    {
        var brasiliaDay = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")).DayOfWeek;

        int bit = brasiliaDay switch
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

        if ((mask & bit) == 0) return false;
        return now >= start && now <= end;
    }

    internal static TimeOnly GetBrasiliaTime()
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        var local = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
        return TimeOnly.FromTimeSpan(local.TimeOfDay);
    }
}
