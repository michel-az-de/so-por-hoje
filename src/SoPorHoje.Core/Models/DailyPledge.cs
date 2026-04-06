using SQLite;

namespace SoPorHoje.Core.Models;

public class DailyPledge
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public DateTime PledgeDate { get; set; }

    public DateTime PledgedAt { get; set; }

    /// <summary>Null = ainda não respondeu se cumpriu. True/False = respondeu.</summary>
    public bool? Fulfilled { get; set; }

    public string? Notes { get; set; }

    // Sync
    public string? RemoteId { get; set; }
    public bool IsSynced { get; set; }
}
