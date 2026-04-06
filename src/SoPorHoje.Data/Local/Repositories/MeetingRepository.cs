using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Data.Local.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly SoPorHojeDatabase _database;
    private readonly ILogger<MeetingRepository> _logger;

    public MeetingRepository(SoPorHojeDatabase database, ILogger<MeetingRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<List<OnlineMeeting>> GetAllAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<OnlineMeeting>().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all meetings");
            throw;
        }
    }

    public async Task<List<OnlineMeeting>> GetLiveNowAsync()
    {
        var all = await GetAllAsync();
        return all.Where(m => m.IsLiveNow).ToList();
    }

    public async Task<OnlineMeeting?> GetNextAsync()
    {
        var all = await GetAllAsync();
        return all
            .Where(m => m.MinutesUntilStart.HasValue)
            .OrderBy(m => m.MinutesUntilStart!.Value)
            .FirstOrDefault();
    }

    public async Task UpsertAsync(List<OnlineMeeting> meetings)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            foreach (var meeting in meetings)
            {
                if (meeting.Id == 0)
                    await db.InsertAsync(meeting);
                else
                    await db.UpdateAsync(meeting);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert meetings");
            throw;
        }
    }

    public async Task<DateTime?> GetLastScrapeTimeAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var meetings = await db.Table<OnlineMeeting>()
                .OrderByDescending(m => m.LastScrapedAt)
                .FirstOrDefaultAsync();
            return meetings?.LastScrapedAt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get last scrape time");
            throw;
        }
    }
}
