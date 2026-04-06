namespace SoPorHoje.Api.DTOs;

public record SyncPullResponse(
    List<MeetingDto> Meetings,
    bool ReflectionsUpdated,
    DateTimeOffset ServerTimestamp
);
