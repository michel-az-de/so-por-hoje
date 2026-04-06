using Microsoft.Extensions.Logging;
using SQLite;
using SoPorHoje.Core.Constants;
using SoPorHoje.Core.Models;

namespace SoPorHoje.Data.Local;

public class SoPorHojeDatabase
{
    private readonly SQLiteAsyncConnection _db;
    private readonly ILogger<SoPorHojeDatabase> _logger;
    private bool _initialized;

    public SoPorHojeDatabase(string dbPath, ILogger<SoPorHojeDatabase> logger)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _logger = logger;
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitializeAsync();
        return _db;
    }

    private async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            await _db.CreateTableAsync<UserProfile>();
            await _db.CreateTableAsync<DailyPledge>();
            await _db.CreateTableAsync<DailyReflection>();
            await _db.CreateTableAsync<SobrietyChip>();
            await _db.CreateTableAsync<ChipEarnedEvent>();
            await _db.CreateTableAsync<OnlineMeeting>();
            await _db.CreateTableAsync<ResetEvent>();

            await SeedChipsAsync();
            _initialized = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }

    private async Task SeedChipsAsync()
    {
        var count = await _db.Table<SobrietyChip>().CountAsync();
        if (count > 0) return;

        foreach (var chip in ChipDefinitions.BrazilianChips)
        {
            await _db.InsertAsync(chip);
        }

        _logger.LogInformation("Seeded {Count} sobriety chips", ChipDefinitions.BrazilianChips.Count);
    }
}
