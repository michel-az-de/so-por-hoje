namespace SoPorHoje.Api.DTOs;

public record SyncProfileDto(
    string SobrietyDate,
    string? PersonalReason
);

public record SyncPledgeDto(
    string PledgeDate,
    DateTimeOffset PledgedAt,
    bool? Fulfilled,
    string? Notes
);

public record SyncChipEventDto(
    int ChipRequiredDays,
    DateTimeOffset EarnedAt
);

public record SyncResetEventDto(
    string PreviousSobrietyDate,
    string NewSobrietyDate,
    int DaysAccumulated,
    DateTimeOffset OccurredAt
);

public record SyncPushRequest(
    string DeviceId,
    SyncProfileDto? Profile,
    List<SyncPledgeDto>? Pledges,
    List<SyncChipEventDto>? ChipEvents,
    List<SyncResetEventDto>? ResetEvents
);

public record SyncPushResponse(
    List<Guid> SyncedPledgeIds,
    List<Guid> SyncedChipEventIds,
    DateTimeOffset ServerTimestamp
);
