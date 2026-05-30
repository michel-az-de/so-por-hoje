namespace SoPorHoje.Api.DTOs;

public record MeetingDto(
    int Id,
    string GroupName,
    int DaysOfWeekMask,
    string StartTime,
    string EndTime,
    string MeetingUrl,
    string? Platform,
    string Source,
    bool IsLiveNow
);
