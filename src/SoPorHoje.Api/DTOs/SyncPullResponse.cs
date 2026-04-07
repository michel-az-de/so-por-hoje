namespace SoPorHoje.Api.DTOs;

/// <summary>
/// Response returned by the sync pull endpoint.
/// / Resposta retornada pelo endpoint de pull de sincronização.
/// </summary>
/// <param name="Meetings">List of active meetings matching the filter. / Lista de reuniões ativas que correspondem ao filtro.</param>
/// <param name="ReflectionsUpdated">
/// Reserved for future use — always <c>false</c> in this version.
/// Use <c>GET /api/reflections</c> to seed reflections.
/// / Reservado para uso futuro — sempre <c>false</c> nesta versão.
/// Use <c>GET /api/reflections</c> para fazer seed das reflexões.
/// </param>
/// <param name="ServerTimestamp">Server UTC timestamp to use as the next <c>since</c> value. / Timestamp UTC do servidor para usar como próximo valor de <c>since</c>.</param>
public record SyncPullResponse(
    List<MeetingDto> Meetings,
    bool ReflectionsUpdated,
    DateTimeOffset ServerTimestamp
);
