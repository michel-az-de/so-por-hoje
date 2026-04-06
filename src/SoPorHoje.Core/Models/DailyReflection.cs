using SQLite;

namespace SoPorHoje.Core.Models;

/// <summary>
/// Reflexão diária oficial do A.A. — 365 entradas, uma por dia.
/// Seed via JSON com schema:
/// { "date": "2025-01-01", "title": "...", "quote": "...", "text": "...", "content": "REFERÊNCIA" }
/// O campo "date" mapeia para DateKey no formato "MM-dd" (ignorar ano).
/// O campo "content" do JSON é a referência bibliográfica → mapeia para Reference.
/// </summary>
public class DailyReflection
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Chave no formato "MM-dd" (ex: "01-01" para 1 de janeiro).</summary>
    [Indexed(Unique = true)]
    public string DateKey { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    /// <summary>Citação da literatura do A.A.</summary>
    public string Quote { get; set; } = string.Empty;

    /// <summary>Texto reflexivo escrito por membros.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Referência bibliográfica (ex: "ALCOÓLICOS ANÔNIMOS, p. 25").</summary>
    public string Reference { get; set; } = string.Empty;
}
