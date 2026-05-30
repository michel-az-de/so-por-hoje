using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoPorHoje.Api.Data.Entities;

[Table("reflections")]
public class ReflectionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>Chave de data no formato "MM-dd", ex: "01-01" a "12-31".</summary>
    [Column("date_key")]
    [MaxLength(5)]
    public string DateKey { get; set; } = null!;

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("quote")]
    public string Quote { get; set; } = null!;

    [Column("text")]
    public string Text { get; set; } = null!;

    [Column("reference")]
    public string Reference { get; set; } = null!;
}
