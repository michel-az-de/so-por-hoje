using SQLite;

namespace SoPorHoje.Core.Models;

/// <summary>Registro de recaída — quando o usuário reseta o contador.</summary>
public class ResetEvent
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime PreviousSobrietyDate { get; set; }
    public DateTime NewSobrietyDate { get; set; }
    public int DaysAccumulated { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? RemoteId { get; set; }
    public bool IsSynced { get; set; }
}
