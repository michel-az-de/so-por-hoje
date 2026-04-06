using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("pledges")]
public class PledgeEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("pledge_date")]
    public DateOnly PledgeDate { get; set; }

    [Column("pledged_at")]
    public DateTimeOffset PledgedAt { get; set; }

    [Column("fulfilled")]
    public bool? Fulfilled { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public UserEntity User { get; set; } = null!;
}
