using SQLite;

namespace SoPorHoje.Core.Models;

/// <summary>
/// Reflexão diária — conteúdo original do projeto (CC BY-SA 4.0), uma por dia do ano.
/// Semeada a partir de JSON com o schema de data/reflections/SCHEMA.md:
/// { "date": "MM-DD", "title", "quote", "text", "theme", "license", "author" }.
/// O campo "date" mapeia para DateKey no formato "MM-dd" (o ano é ignorado).
/// </summary>
public class DailyReflection
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Chave no formato "MM-dd" (ex: "01-01" para 1 de janeiro).</summary>
    [Indexed(Unique = true)]
    public string DateKey { get; set; } = string.Empty;

    /// <summary>Tema curto da reflexão (ex: "Aceitação").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Frase curta de abertura — original ou de domínio público (com atribuição).</summary>
    public string Quote { get; set; } = string.Empty;

    /// <summary>A reflexão do dia (tom acolhedor, 2 a 5 frases).</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Slug do tema para agrupar/buscar (ex: "aceitacao").</summary>
    public string Theme { get; set; } = string.Empty;

    /// <summary>Licença do conteúdo (ex: "CC-BY-SA-4.0").</summary>
    public string License { get; set; } = string.Empty;

    /// <summary>Autoria do conteúdo (ex: "Projeto Só Por Hoje").</summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Campo legado para atribuição de fonte de domínio público, quando aplicável.
    /// Não é usado pelo schema atual de reflexões autorais.
    /// </summary>
    public string Reference { get; set; } = string.Empty;
}
