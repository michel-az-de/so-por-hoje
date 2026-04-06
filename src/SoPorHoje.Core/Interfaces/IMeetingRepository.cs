using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IMeetingRepository
{
    Task<List<OnlineMeeting>> GetAllAsync();
    Task<List<OnlineMeeting>> GetLiveNowAsync();
    Task<OnlineMeeting?> GetNextAsync();
    Task UpsertAsync(List<OnlineMeeting> meetings);
    Task<DateTime?> GetLastScrapeTimeAsync();
}
