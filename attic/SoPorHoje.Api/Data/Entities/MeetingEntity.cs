using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("meetings")]
public class MeetingEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("group_name")]
    [MaxLength(500)]
    public string GroupName { get; set; } = null!;

    [Column("days_of_week_mask")]
    public int DaysOfWeekMask { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("meeting_url")]
    public string MeetingUrl { get; set; } = null!;

    [Column("platform")]
    [MaxLength(100)]
    public string? Platform { get; set; }

    [Column("source")]
    [MaxLength(255)]
    public string Source { get; set; } = "intergrupos-aa.org.br";

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_scraped_at")]
    public DateTimeOffset? LastScrapedAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
