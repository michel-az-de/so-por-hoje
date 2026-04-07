namespace SoPorHoje.Api.DTOs;

/// <summary>
/// User sobriety profile included in a push request.
/// / Perfil de sobriedade do usuário incluído em uma requisição de push.
/// </summary>
/// <param name="SobrietyDate">
/// Date of the last drink — sobriety start date in ISO 8601 format (e.g. "2024-06-15").
/// / Data do último gole — início da sobriedade no formato ISO 8601 (ex: "2024-06-15").
/// </param>
/// <param name="PersonalReason">
/// Optional personal motivation text. Not required; preserved as typed by the user.
/// / Texto motivacional pessoal opcional. Não obrigatório; preservado como digitado pelo usuário.
/// </param>
public record SyncProfileDto(
    string SobrietyDate,
    string? PersonalReason
);

/// <summary>
/// A single "just for today" daily pledge.
/// / Um compromisso diário "só por hoje".
/// </summary>
/// <param name="PledgeDate">Date of the pledge in ISO 8601 format (e.g. "2026-04-07"). / Data do compromisso no formato ISO 8601.</param>
/// <param name="PledgedAt">UTC timestamp when the pledge was made on the device. / Timestamp UTC de quando o compromisso foi feito no dispositivo.</param>
/// <param name="Fulfilled">
/// <c>null</c> = unanswered; <c>true</c> = kept; <c>false</c> = broken.
/// / <c>null</c> = não respondido; <c>true</c> = cumprido; <c>false</c> = não cumprido.
/// </param>
/// <param name="Notes">Optional free-text note added by the user. / Nota de texto livre opcional adicionada pelo usuário.</param>
public record SyncPledgeDto(
    string PledgeDate,
    DateTimeOffset PledgedAt,
    bool? Fulfilled,
    string? Notes
);

/// <summary>
/// A sobriety chip/milestone earned by the user.
/// / Uma ficha/marco de sobriedade conquistada pelo usuário.
/// </summary>
/// <param name="ChipRequiredDays">
/// Number of sober days required for this chip (e.g. 1, 90, 180, 270, 365…).
/// / Número de dias de sobriedade exigidos para esta ficha (ex: 1, 90, 180, 270, 365…).
/// </param>
/// <param name="EarnedAt">UTC timestamp when the chip was earned. / Timestamp UTC de quando a ficha foi conquistada.</param>
public record SyncChipEventDto(
    int ChipRequiredDays,
    DateTimeOffset EarnedAt
);

/// <summary>
/// A sobriety reset event (relapse).
/// / Um evento de reset de sobriedade (recaída).
/// </summary>
/// <param name="PreviousSobrietyDate">Sobriety start date before the reset (ISO 8601). / Data de início de sobriedade antes do reset (ISO 8601).</param>
/// <param name="NewSobrietyDate">New sobriety start date after the reset (ISO 8601). / Nova data de início de sobriedade após o reset (ISO 8601).</param>
/// <param name="DaysAccumulated">Number of sober days accumulated before the reset. / Número de dias sóbrios acumulados antes do reset.</param>
/// <param name="OccurredAt">UTC timestamp when the reset was recorded. / Timestamp UTC de quando o reset foi registrado.</param>
public record SyncResetEventDto(
    string PreviousSobrietyDate,
    string NewSobrietyDate,
    int DaysAccumulated,
    DateTimeOffset OccurredAt
);

/// <summary>
/// Request body for the sync push endpoint.
/// / Corpo da requisição para o endpoint de push de sincronização.
/// </summary>
/// <param name="DeviceId">Device UUID identifying the user. / UUID do dispositivo que identifica o usuário.</param>
/// <param name="Profile">Updated sobriety profile (optional). / Perfil de sobriedade atualizado (opcional).</param>
/// <param name="Pledges">List of daily pledges to upsert (optional). / Lista de compromissos diários para upsert (opcional).</param>
/// <param name="ChipEvents">List of earned chip events (optional). / Lista de fichas conquistadas (opcional).</param>
/// <param name="ResetEvents">List of sobriety reset events (optional). / Lista de eventos de reset de sobriedade (opcional).</param>
public record SyncPushRequest(
    string DeviceId,
    SyncProfileDto? Profile,
    List<SyncPledgeDto>? Pledges,
    List<SyncChipEventDto>? ChipEvents,
    List<SyncResetEventDto>? ResetEvents
);

/// <summary>
/// Response returned after a successful sync push.
/// / Resposta retornada após um push de sincronização bem-sucedido.
/// </summary>
/// <param name="SyncedPledgeIds">Server UUIDs assigned to the upserted pledges. / UUIDs do servidor atribuídos aos pledges upsertados.</param>
/// <param name="SyncedChipEventIds">Server UUIDs assigned to the upserted chip events. / UUIDs do servidor atribuídos aos chip events upsertados.</param>
/// <param name="ServerTimestamp">Server UTC timestamp at the time of the response. / Timestamp UTC do servidor no momento da resposta.</param>
public record SyncPushResponse(
    List<Guid> SyncedPledgeIds,
    List<Guid> SyncedChipEventIds,
    DateTimeOffset ServerTimestamp
);
