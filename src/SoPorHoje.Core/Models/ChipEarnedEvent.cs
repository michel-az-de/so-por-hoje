using SQLite;

namespace SoPorHoje.Core.Models;

public class ChipEarnedEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int ChipRequiredDays { get; set; }
    public DateTime EarnedAt { get; set; }
    public bool CelebrationShown { get; set; }

    public string? RemoteId { get; set; }
    public bool IsSynced { get; set; }
}
