using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("chip_events")]
public class ChipEventEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("chip_required_days")]
    public int ChipRequiredDays { get; set; }

    [Column("earned_at")]
    public DateTimeOffset EarnedAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public UserEntity User { get; set; } = null!;
}
