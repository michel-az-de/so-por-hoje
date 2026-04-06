using SQLite;

namespace SoPorHoje.Core.Models;

public class UserProfile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Data do último gole — marco zero da sobriedade.</summary>
    public DateTime SobrietyDate { get; set; }

    /// <summary>Motivo pessoal para parar de beber (âncora emocional).</summary>
    public string? PersonalReason { get; set; }

    /// <summary>Nome opcional — nunca obrigatório (anonimato).</summary>
    public string? DisplayName { get; set; }

    /// <summary>UUID anônimo para sync com servidor. Gerado no device, sem email.</summary>
    public string DeviceId { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Ignore]
    public int SoberDays => Math.Max(0, (int)(DateTime.Today - SobrietyDate.Date).TotalDays);
}
