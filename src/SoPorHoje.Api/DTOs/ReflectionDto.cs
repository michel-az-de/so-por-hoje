namespace SoPorHoje.Api.DTOs;

/// <summary>
/// A single daily reflection entry from the AA "Daily Reflections" book.
/// / Uma entrada de reflexão diária do livro "Reflexões Diárias" de A.A.
/// </summary>
/// <param name="DateKey">
/// Date key in MM-dd format (e.g. "04-07" for April 7th), year-independent.
/// / Chave de data no formato MM-dd (ex: "04-07" para 7 de abril), independente do ano.
/// </param>
/// <param name="Title">Reflection title in uppercase (e.g. "ACEITAÇÃO"). / Título da reflexão em maiúsculas (ex: "ACEITAÇÃO").</param>
/// <param name="Quote">Inspirational quote from AA literature. / Citação inspiracional da literatura de A.A.</param>
/// <param name="Text">Main reflection text (meditation for the day). / Texto principal da reflexão (meditação do dia).</param>
/// <param name="Reference">Source reference from AA literature. / Referência bibliográfica da literatura de A.A.</param>
public record ReflectionDto(
    string DateKey,
    string Title,
    string Quote,
    string Text,
    string Reference
);

/// <summary>
/// Paginated list of reflections.
/// / Lista paginada de reflexões.
/// </summary>
/// <param name="Items">Reflection items on the current page. / Itens de reflexão na página atual.</param>
/// <param name="Page">Current page number (1-based). / Número da página atual (base 1).</param>
/// <param name="PageSize">Number of items per page. / Número de itens por página.</param>
/// <param name="TotalCount">Total number of reflections in the database (365). / Total de reflexões no banco de dados (365).</param>
public record PagedReflectionsResponse(
    List<ReflectionDto> Items,
    int Page,
    int PageSize,
    int TotalCount
);
