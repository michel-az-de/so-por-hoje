using SoPorHoje.App.Models;

namespace SoPorHoje.App.Interfaces;

/// <summary>Repositório de reuniões online de AA.</summary>
public interface IMeetingRepository
{
    Task<IReadOnlyList<OnlineMeeting>> GetAllAsync();
    Task<IReadOnlyList<OnlineMeeting>> GetLiveNowAsync();
    Task<OnlineMeeting?> GetNextAsync();
    Task UpsertAsync(OnlineMeeting meeting);
}
