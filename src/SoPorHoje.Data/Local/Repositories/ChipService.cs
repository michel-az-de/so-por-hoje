using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Constants;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Data.Local.Repositories;

public class ChipService : IChipService
{
    private readonly SoPorHojeDatabase _database;
    private readonly ILogger<ChipService> _logger;
    private static readonly List<SobrietyChip> Chips = ChipDefinitions.BrazilianChips
        .OrderBy(c => c.RequiredDays)
        .ToList();

    public ChipService(SoPorHojeDatabase database, ILogger<ChipService> logger)
    {
        _database = database;
        _logger = logger;
    }

    public List<SobrietyChip> GetAllChips() => Chips;

    public SobrietyChip GetCurrentChip(int soberDays)
    {
        return Chips.LastOrDefault(c => soberDays >= c.RequiredDays) ?? Chips[0];
    }

    public SobrietyChip? GetNextChip(int soberDays)
    {
        return Chips.FirstOrDefault(c => c.RequiredDays > soberDays);
    }

    public double GetProgressToNext(int soberDays)
    {
        var current = GetCurrentChip(soberDays);
        var next = GetNextChip(soberDays);
        if (next == null) return 1.0;

        var range = next.RequiredDays - current.RequiredDays;
        var progress = soberDays - current.RequiredDays;
        return range == 0 ? 1.0 : Math.Clamp((double)progress / range, 0.0, 1.0);
    }

    public int GetDaysUntilNext(int soberDays)
    {
        var next = GetNextChip(soberDays);
        return next == null ? 0 : Math.Max(0, next.RequiredDays - soberDays);
    }

    public async Task<List<ChipEarnedEvent>> GetUncelebratedAsync(int soberDays)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var earnedChips = Chips.Where(c => soberDays >= c.RequiredDays).ToList();
            var celebratedDays = (await db.Table<ChipEarnedEvent>()
                .Where(e => e.CelebrationShown)
                .ToListAsync())
                .Select(e => e.ChipRequiredDays)
                .ToHashSet();

            var uncelebrated = new List<ChipEarnedEvent>();
            foreach (var chip in earnedChips.Where(c => !celebratedDays.Contains(c.RequiredDays)))
            {
                var existing = await db.Table<ChipEarnedEvent>()
                    .Where(e => e.ChipRequiredDays == chip.RequiredDays)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    var newEvent = new ChipEarnedEvent
                    {
                        ChipRequiredDays = chip.RequiredDays,
                        EarnedAt = DateTime.UtcNow,
                        CelebrationShown = false,
                    };
                    await db.InsertAsync(newEvent);
                    uncelebrated.Add(newEvent);
                }
                else if (!existing.CelebrationShown)
                {
                    uncelebrated.Add(existing);
                }
            }

            return uncelebrated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get uncelebrated chips");
            throw;
        }
    }

    public async Task MarkCelebratedAsync(int chipRequiredDays)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var evt = await db.Table<ChipEarnedEvent>()
                .Where(e => e.ChipRequiredDays == chipRequiredDays)
                .FirstOrDefaultAsync();

            if (evt != null)
            {
                evt.CelebrationShown = true;
                await db.UpdateAsync(evt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark chip as celebrated for {Days} days", chipRequiredDays);
            throw;
        }
    }

    public int GetEarnedCount(int soberDays) => Chips.Count(c => soberDays >= c.RequiredDays);
}
