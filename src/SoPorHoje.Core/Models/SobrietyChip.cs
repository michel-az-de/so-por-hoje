using SQLite;

namespace SoPorHoje.Core.Models;

/// <summary>
/// Fichas de sobriedade do padrão brasileiro de A.A.
/// Sequência: Amarela (ingresso), Azul (3m), Rosa (6m), Vermelha (9m),
/// Verde (1a), Verde Gravata (2a), Branca Gravata (5a), Amarela Gravata (10a),
/// Azul Gravata (15a), Rosa Gravata (20a).
/// </summary>
public class SobrietyChip
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int RequiredDays { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public bool IsEarned(int soberDays) => soberDays >= RequiredDays;
}
