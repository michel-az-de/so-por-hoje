using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Services;

namespace SoPorHoje.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", async (AppDbContext db) =>
        {
            string dbStatus;
            try
            {
                await db.Database.CanConnectAsync();
                dbStatus = "ok";
            }
            catch
            {
                dbStatus = "error";
            }

            var lastRun = ScraperHostedService.LastRunAt;

            return Results.Ok(new
            {
                status = dbStatus == "ok" ? "healthy" : "degraded",
                database = dbStatus,
                scraper = lastRun.HasValue
                    ? $"last_run: {lastRun.Value:yyyy-MM-ddTHH:mm:ssZ}"
                    : "never_run",
            });
        })
        .WithName("Health")
        .WithSummary("Verifica saúde da API e do banco de dados")
        .WithTags("Health");
    }
}
