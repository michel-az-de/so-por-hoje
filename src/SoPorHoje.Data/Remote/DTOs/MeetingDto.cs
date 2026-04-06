namespace SoPorHoje.Data.Remote.DTOs;

public class MeetingDto
{
    public string RemoteId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int DaysOfWeekMask { get; set; }
    public long StartTimeTicks { get; set; }
    public long EndTimeTicks { get; set; }
    public string MeetingUrl { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime LastScrapedAt { get; set; }
}

public class ReflectionDto
{
    public string DateKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}
