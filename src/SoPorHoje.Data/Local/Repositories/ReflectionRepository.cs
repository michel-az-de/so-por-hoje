using System.Text.Json;
using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Data.Local.Repositories;

public class ReflectionRepository : IReflectionRepository
{
    private readonly SoPorHojeDatabase _database;
    private readonly ILogger<ReflectionRepository> _logger;

    public ReflectionRepository(SoPorHojeDatabase database, ILogger<ReflectionRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<DailyReflection?> GetTodaysReflectionAsync()
    {
        var key = DateTime.Now.ToString("MM-dd");
        return await GetByDateKeyAsync(key);
    }

    public async Task<DailyReflection?> GetByDateKeyAsync(string dateKey)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<DailyReflection>()
                .Where(r => r.DateKey == dateKey)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get reflection for date key {DateKey}", dateKey);
            throw;
        }
    }

    public async Task SeedFromJsonAsync(Stream jsonStream)
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            var count = await db.Table<DailyReflection>().CountAsync();
            if (count > 0)
            {
                _logger.LogInformation("Reflections already seeded ({Count} entries). Skipping.", count);
                return;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var entries = await JsonSerializer.DeserializeAsync<List<ReflectionJsonEntry>>(jsonStream, options);
            if (entries == null || entries.Count == 0)
            {
                _logger.LogWarning("No reflection entries found in JSON stream");
                return;
            }

            var reflections = entries.Select(e => new DailyReflection
            {
                DateKey = DateTime.ParseExact(e.Date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM-dd"),
                Title = e.Title ?? string.Empty,
                Quote = e.Quote ?? string.Empty,
                Text = e.Text ?? string.Empty,
                Reference = e.Content ?? string.Empty,
            }).ToList();

            await db.InsertAllAsync(reflections);
            _logger.LogInformation("Seeded {Count} daily reflections", reflections.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed reflections from JSON");
            throw;
        }
    }

    public async Task<int> GetCountAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<DailyReflection>().CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get reflections count");
            throw;
        }
    }

    private sealed class ReflectionJsonEntry
    {
        public string Date { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Quote { get; set; }
        public string? Text { get; set; }
        public string? Content { get; set; }
    }
}
