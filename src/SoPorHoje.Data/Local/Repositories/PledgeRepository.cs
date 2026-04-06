using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Data.Local.Repositories;

public class PledgeRepository : IPledgeRepository
{
    private readonly SoPorHojeDatabase _database;
    private readonly ILogger<PledgeRepository> _logger;

    public PledgeRepository(SoPorHojeDatabase database, ILogger<PledgeRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<DailyPledge?> GetTodaysPledgeAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var today = DateTime.Today;
            return await db.Table<DailyPledge>()
                .Where(p => p.PledgeDate == today)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get today's pledge");
            throw;
        }
    }

    public async Task SavePledgeAsync(DailyPledge pledge)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            if (pledge.Id == 0)
                await db.InsertAsync(pledge);
            else
                await db.UpdateAsync(pledge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save pledge");
            throw;
        }
    }

    public async Task<List<DailyPledge>> GetHistoryAsync(int days = 30)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var cutoff = DateTime.Today.AddDays(-days);
            return await db.Table<DailyPledge>()
                .Where(p => p.PledgeDate >= cutoff)
                .OrderByDescending(p => p.PledgeDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pledge history");
            throw;
        }
    }

    public async Task<int> GetStreakAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var pledges = await db.Table<DailyPledge>()
                .OrderByDescending(p => p.PledgeDate)
                .ToListAsync();

            var pledgeDates = new HashSet<DateTime>(pledges.Select(p => p.PledgeDate.Date));
            var streak = 0;
            var date = DateTime.Today;

            while (pledgeDates.Contains(date))
            {
                streak++;
                date = date.AddDays(-1);
            }

            return streak;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate streak");
            throw;
        }
    }

    public async Task<int> GetTotalPledgesAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<DailyPledge>().CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get total pledges count");
            throw;
        }
    }
}
