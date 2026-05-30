using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Data.Entities;
using SoPorHoje.Scraper;

namespace SoPorHoje.Api.Services;

/// <summary>
/// Executa o scraper de reuniões a cada 30 minutos e persiste os resultados no banco.
/// </summary>
public class ScraperHostedService(
    IServiceScopeFactory scopeFactory,
    IntergruposScraper scraper,
    ILogger<ScraperHostedService> logger) : BackgroundService
{
    private static DateTimeOffset? _lastRunAt;

    public static DateTimeOffset? LastRunAt => _lastRunAt;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("ScraperHostedService iniciado");

        // Executa imediatamente na primeira vez
        await RunScraperAsync(ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), ct);
                await RunScraperAsync(ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro no ciclo do ScraperHostedService — continuando");
            }
        }

        logger.LogInformation("ScraperHostedService encerrado");
    }

    private async Task RunScraperAsync(CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Iniciando ciclo de scraping");
            var scraped = await scraper.ScrapeAsync(ct);
            _lastRunAt = DateTimeOffset.UtcNow;

            if (scraped.Count == 0)
            {
                logger.LogWarning("Scraper retornou lista vazia — banco não alterado");
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await UpsertMeetingsAsync(db, scraped, ct);
            logger.LogInformation("Ciclo de scraping concluído — {Count} reuniões processadas", scraped.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar o scraper");
        }
    }

    private static async Task UpsertMeetingsAsync(AppDbContext db, List<ScrapedMeeting> scraped, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var scrapedUrls = scraped.Select(s => s.MeetingUrl).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Desativar reuniões que não aparecem mais no scraping
        var toDeactivate = await db.Meetings
            .Where(m => m.IsActive && !scrapedUrls.Contains(m.MeetingUrl))
            .ToListAsync(ct);

        foreach (var m in toDeactivate)
        {
            m.IsActive = false;
            m.UpdatedAt = now;
        }

        // Upsert das reuniões scraped
        foreach (var s in scraped)
        {
            var existing = await db.Meetings
                .FirstOrDefaultAsync(m => m.MeetingUrl == s.MeetingUrl, ct);

            if (existing is null)
            {
                db.Meetings.Add(new MeetingEntity
                {
                    GroupName = s.GroupName,
                    DaysOfWeekMask = s.DaysOfWeekMask,
                    StartTime = TimeOnly.FromTimeSpan(s.StartTime),
                    EndTime = TimeOnly.FromTimeSpan(s.EndTime),
                    MeetingUrl = s.MeetingUrl,
                    Platform = s.Platform,
                    IsActive = true,
                    LastScrapedAt = now,
                });
            }
            else
            {
                existing.GroupName = s.GroupName;
                existing.DaysOfWeekMask = s.DaysOfWeekMask;
                existing.StartTime = TimeOnly.FromTimeSpan(s.StartTime);
                existing.EndTime = TimeOnly.FromTimeSpan(s.EndTime);
                existing.Platform = s.Platform;
                existing.IsActive = true;
                existing.LastScrapedAt = now;
                existing.UpdatedAt = now;
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
