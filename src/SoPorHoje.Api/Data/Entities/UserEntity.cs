using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("users")]
public class UserEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("device_id")]
    [MaxLength(255)]
    public string DeviceId { get; set; } = null!;

    [Column("sobriety_date")]
    public DateOnly SobrietyDate { get; set; }

    [Column("personal_reason")]
    public string? PersonalReason { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<PledgeEntity> Pledges { get; set; } = [];
    public ICollection<ChipEventEntity> ChipEvents { get; set; } = [];
    public ICollection<ResetEventEntity> ResetEvents { get; set; } = [];
}
