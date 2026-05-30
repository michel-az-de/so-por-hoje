using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.DTOs;

namespace SoPorHoje.Api.Endpoints;

public static class ReflectionEndpoints
{
    public static void MapReflectionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/reflections/today", async (AppDbContext db) =>
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
            var dateKey = today.ToString("MM-dd");

            var reflection = await db.Reflections.FirstOrDefaultAsync(r => r.DateKey == dateKey);
            if (reflection is null)
                return Results.NotFound(new { error = $"Reflexão não encontrada para a data {dateKey}" });

            return Results.Ok(new ReflectionDto(
                reflection.DateKey,
                reflection.Title,
                reflection.Quote,
                reflection.Text,
                reflection.Reference
            ));
        })
        .WithName("GetTodayReflection")
        .WithSummary("Retorna a reflexão do dia atual (horário de Brasília)")
        .WithTags("Reflections");

        app.MapGet("/api/reflections", async (AppDbContext db, int page = 1, int pageSize = 50) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            var total = await db.Reflections.CountAsync();
            var items = await db.Reflections
                .OrderBy(r => r.DateKey)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReflectionDto(r.DateKey, r.Title, r.Quote, r.Text, r.Reference))
                .ToListAsync();

            return Results.Ok(new PagedReflectionsResponse(items, page, pageSize, total));
        })
        .WithName("GetReflections")
        .WithSummary("Lista reflexões paginadas (seed completo do app)")
        .WithTags("Reflections");
    }
}
