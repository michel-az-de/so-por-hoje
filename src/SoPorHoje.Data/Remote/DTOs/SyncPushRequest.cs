using SoPorHoje.Core.Models;

namespace SoPorHoje.Data.Remote.DTOs;

public class SyncPushRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public UserProfile? Profile { get; set; }
    public List<DailyPledge> Pledges { get; set; } = new();
    public List<ChipEarnedEvent> ChipEvents { get; set; } = new();
    public List<ResetEvent> ResetEvents { get; set; } = new();
}

public class SyncPushResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> SyncedPledgeRemoteIds { get; set; } = new();
    public List<string> SyncedChipEventRemoteIds { get; set; } = new();
    public List<string> SyncedResetEventRemoteIds { get; set; } = new();
}
