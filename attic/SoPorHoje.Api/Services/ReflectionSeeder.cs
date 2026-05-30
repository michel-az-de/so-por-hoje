using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Data.Entities;

namespace SoPorHoje.Api.Services;

public static class ReflectionSeeder
{
    private record ReflectionJson(
        [property: JsonPropertyName("date")] string Date,
        [property: JsonPropertyName("language")] string Language,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("quote")] string Quote,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("content")] string Content
    );

    public static async Task SeedAsync(AppDbContext db, string jsonPath, ILogger logger, CancellationToken ct = default)
    {
        if (await db.Reflections.AnyAsync(ct))
        {
            logger.LogInformation("Tabela de reflexões já preenchida — seed ignorado");
            return;
        }

        if (!File.Exists(jsonPath))
        {
            logger.LogWarning("Arquivo de reflexões não encontrado em {Path} — seed ignorado", jsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath, ct);
        var items = JsonSerializer.Deserialize<List<ReflectionJson>>(json);

        if (items is null || items.Count == 0)
        {
            logger.LogWarning("Arquivo de reflexões vazio ou inválido");
            return;
        }

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Date) || item.Date.Length < 5) continue;

            // "2025-01-01" → "01-01"
            var dateKey = item.Date.Length >= 10 ? item.Date[5..] : item.Date;

            db.Reflections.Add(new ReflectionEntity
            {
                DateKey = dateKey,
                Title = item.Title,
                Quote = item.Quote,
                Text = item.Text,
                Reference = item.Content,
            });
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seed de {Count} reflexões concluído", items.Count);
    }
}
