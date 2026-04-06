namespace SoPorHoje.Data.Remote.DTOs;

public class SyncPullResponse
{
    public List<MeetingDto> Meetings { get; set; } = new();
    public List<ReflectionDto> Reflections { get; set; } = new();
    public DateTime ServerTime { get; set; }
}
