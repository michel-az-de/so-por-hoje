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
                DateKey = NormalizeDateKey(e.Date),
                Title = e.Title ?? string.Empty,
                Quote = e.Quote ?? string.Empty,
                Text = e.Text ?? string.Empty,
                Theme = e.Theme ?? string.Empty,
                License = e.License ?? string.Empty,
                Author = e.Author ?? string.Empty,
                Reference = e.Content ?? e.Reference ?? string.Empty,
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

    public async Task<List<DailyReflection>> GetAllAsync()
    {
        try
        {
            var db = await _database.GetConnectionAsync();
            return await db.Table<DailyReflection>().OrderBy(r => r.DateKey).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all reflections");
            throw;
        }
    }

    private static string NormalizeDateKey(string raw)
    {
        var ci = System.Globalization.CultureInfo.InvariantCulture;
        if (DateTime.TryParseExact(raw, "yyyy-MM-dd", ci, System.Globalization.DateTimeStyles.None, out var full))
            return full.ToString("MM-dd");
        if (DateTime.TryParseExact(raw, "MM-dd", ci, System.Globalization.DateTimeStyles.None, out var md))
            return md.ToString("MM-dd");
        return raw;
    }

    private sealed class ReflectionJsonEntry
    {
        public string Date { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Quote { get; set; }
        public string? Text { get; set; }
        public string? Theme { get; set; }
        public string? License { get; set; }
        public string? Author { get; set; }
        // Campos legados (schema antigo) — tolerados na leitura.
        public string? Content { get; set; }
        public string? Reference { get; set; }
    }
}
