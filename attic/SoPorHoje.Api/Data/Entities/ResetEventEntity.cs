using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("reset_events")]
public class ResetEventEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("previous_sobriety_date")]
    public DateOnly PreviousSobrietyDate { get; set; }

    [Column("new_sobriety_date")]
    public DateOnly NewSobrietyDate { get; set; }

    [Column("days_accumulated")]
    public int DaysAccumulated { get; set; }

    [Column("occurred_at")]
    public DateTimeOffset OccurredAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public UserEntity User { get; set; } = null!;
}
