using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Data.Local.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SoPorHojeDatabase _database;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(SoPorHojeDatabase database, ILogger<UserRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<UserProfile?> GetProfileAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<UserProfile>().FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile");
            throw;
        }
    }

    public async Task SaveProfileAsync(UserProfile profile)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            profile.UpdatedAt = DateTime.UtcNow;
            if (profile.Id == 0)
                await db.InsertAsync(profile);
            else
                await db.UpdateAsync(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user profile");
            throw;
        }
    }

    public async Task<bool> HasProfileAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<UserProfile>().CountAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if profile exists");
            throw;
        }
    }

    public async Task ResetSobrietyAsync(DateTime newDate)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var profile = await db.Table<UserProfile>().FirstOrDefaultAsync();
            if (profile == null) return;

            var resetEvent = new ResetEvent
            {
                PreviousSobrietyDate = profile.SobrietyDate,
                NewSobrietyDate = newDate,
                DaysAccumulated = profile.SoberDays,
                OccurredAt = DateTime.UtcNow,
            };

            await db.InsertAsync(resetEvent);

            profile.SobrietyDate = newDate;
            profile.UpdatedAt = DateTime.UtcNow;
            await db.UpdateAsync(profile);

            _logger.LogInformation("Sobriety reset. Previous date: {Previous}, New date: {New}",
                resetEvent.PreviousSobrietyDate, newDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset sobriety");
            throw;
        }
    }
}
