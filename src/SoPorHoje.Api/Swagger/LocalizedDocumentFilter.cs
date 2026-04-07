using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoPorHoje.Api.Swagger;

/// <summary>
/// Applies language-specific summaries, descriptions, and tag metadata to the
/// generated OpenAPI document. Supports "v1-en" (English) and "v1-ptbr" (Português).
/// </summary>
internal sealed class LocalizedDocumentFilter : IDocumentFilter
{
    // operationId → (EN summary, EN description, PT summary, PT description)
    private static readonly Dictionary<string, (string Es, string Ed, string Ps, string Pd)> Operations = new()
    {
        ["AuthAnonymous"] = (
            "Create or retrieve anonymous user",
            """
            Creates a new anonymous user account for the given `deviceId`, or returns the
            existing user if one is already registered for that device.

            **Business rules:**
            - No personal data is collected — only a UUID generated on the device.
            - The same `deviceId` always maps to the same `userId` (idempotent).
            - Call this once at app launch to obtain the `userId` needed for sync operations.
            """,
            "Criar ou recuperar usuário anônimo",
            """
            Cria uma nova conta anônima para o `deviceId` informado, ou retorna o usuário
            existente caso já esteja cadastrado para aquele dispositivo.

            **Regras de negócio:**
            - Nenhum dado pessoal é coletado — apenas um UUID gerado no dispositivo.
            - O mesmo `deviceId` sempre mapeia ao mesmo `userId` (operação idempotente).
            - Chame uma vez ao iniciar o app para obter o `userId` necessário nas operações de sync.
            """
        ),
        ["SyncPush"] = (
            "Push local data to server",
            """
            Receives data collected offline on the device and persists it on the server.

            **Payload sections:**
            - `profile` — sobriety date and optional personal motivation text.
            - `pledges` — daily "just for today" commitments with optional fulfillment flag.
            - `chipEvents` — sobriety milestones (chips) earned by the user.
            - `resetEvents` — sobriety counter resets (relapse events).

            **Business rules:**
            - Follows upsert semantics: a pledge for the same `pledgeDate` is updated, not duplicated.
            - A chip event with the same `chipRequiredDays` and `earnedAt` is not duplicated.
            - A reset event with the same `occurredAt` is not duplicated.
            - If `deviceId` does not exist yet, a new user is created automatically.
            - The server returns the UUIDs assigned to synced pledges and chip events.
            """,
            "Enviar dados locais ao servidor",
            """
            Recebe os dados coletados offline no dispositivo e os persiste no servidor.

            **Seções do payload:**
            - `profile` — data de sobriedade e texto motivacional pessoal opcional.
            - `pledges` — compromissos diários "só por hoje" com flag de cumprimento opcional.
            - `chipEvents` — marcos de sobriedade (fichas) conquistados pelo usuário.
            - `resetEvents` — resets do contador de sobriedade (eventos de recaída).

            **Regras de negócio:**
            - Segue semântica de upsert: um compromisso para o mesmo `pledgeDate` é atualizado, não duplicado.
            - Um chip event com mesmo `chipRequiredDays` e `earnedAt` não é duplicado.
            - Um reset event com mesmo `occurredAt` não é duplicado.
            - Se o `deviceId` não existir, um novo usuário é criado automaticamente.
            - O servidor retorna os UUIDs atribuídos aos pledges e chip events sincronizados.
            """
        ),
        ["SyncPull"] = (
            "Pull updated data from server",
            """
            Returns meetings updated since the given timestamp. The response always includes
            the server's current timestamp so the client can use it as the next `since` value.

            **Query parameters:**
            - `since` (optional) — ISO 8601 UTC timestamp for incremental sync (e.g. `2026-01-01T00:00:00Z`).
              Omit for a full sync of all active meetings.

            **Business rules:**
            - Only active meetings are returned.
            - Meetings are ordered by group name.
            - The `isLiveNow` field reflects Brasília time at the moment of the request.
            """,
            "Buscar dados atualizados do servidor",
            """
            Retorna reuniões atualizadas desde o timestamp informado. A resposta sempre inclui
            o timestamp atual do servidor para que o cliente possa usá-lo como próximo valor de `since`.

            **Parâmetros de query:**
            - `since` (opcional) — timestamp UTC em ISO 8601 para sync incremental (ex: `2026-01-01T00:00:00Z`).
              Omita para sincronização completa de todas as reuniões ativas.

            **Regras de negócio:**
            - Apenas reuniões ativas são retornadas.
            - Reuniões são ordenadas por nome do grupo.
            - O campo `isLiveNow` reflete o horário de Brasília no momento da requisição.
            """
        ),
        ["DeleteUser"] = (
            "Permanently delete user data (GDPR/LGPD)",
            """
            Permanently and irreversibly deletes **all** data associated with the user identified
            by `deviceId`: profile, pledges, chip events, and reset events.

            **Business rules:**
            - This operation is irreversible — there is no soft-delete or recovery.
            - Cascade delete ensures all child records are removed from the database.
            - Returns `204 No Content` whether the user existed or not (idempotent).
            - Fulfills GDPR (EU) and LGPD (Brazil) right-to-erasure requirements.
            """,
            "Deletar permanentemente os dados do usuário (LGPD/GDPR)",
            """
            Exclui **permanente e irreversivelmente** todos os dados do usuário identificado
            pelo `deviceId`: perfil, compromissos, fichas e eventos de reset.

            **Regras de negócio:**
            - Esta operação é irreversível — não há soft-delete nem recuperação.
            - A exclusão em cascata garante que todos os registros filhos sejam removidos do banco.
            - Retorna `204 No Content` independente de o usuário existir ou não (idempotente).
            - Atende ao direito de exclusão da LGPD (Brasil) e GDPR (UE).
            """
        ),
        ["GetMeetings"] = (
            "List all active AA online meetings",
            """
            Returns all currently active Alcoholics Anonymous online meetings scraped from
            intergrupos-aa.org.br. Each meeting includes a real-time `isLiveNow` flag.

            **Business rules:**
            - Meetings are collected every 30 minutes by a background scraper.
            - A meeting is deactivated (`isActive = false`) when it no longer appears in the scraper results.
            - Results are ordered by start time, then group name.
            - `isLiveNow` is calculated using the `DaysOfWeekMask` bitmask and Brasília time.

            **DaysOfWeekMask encoding:**
            `Sunday=1, Monday=2, Tuesday=4, Wednesday=8, Thursday=16, Friday=32, Saturday=64`
            — e.g. `127` means every day of the week.
            """,
            "Listar todas as reuniões online de A.A. ativas",
            """
            Retorna todas as reuniões online de Alcoólicos Anônimos ativas, coletadas de
            intergrupos-aa.org.br. Cada reunião inclui o campo `isLiveNow` em tempo real.

            **Regras de negócio:**
            - As reuniões são coletadas a cada 30 minutos por um scraper em background.
            - Uma reunião é desativada (`isActive = false`) quando deixa de aparecer nos resultados do scraper.
            - Resultados ordenados por horário de início, depois por nome do grupo.
            - `isLiveNow` é calculado usando o bitmask `DaysOfWeekMask` e o horário de Brasília.

            **Codificação do DaysOfWeekMask:**
            `Domingo=1, Segunda=2, Terça=4, Quarta=8, Quinta=16, Sexta=32, Sábado=64`
            — ex: `127` significa todos os dias da semana.
            """
        ),
        ["GetLiveMeetings"] = (
            "List meetings happening right now (Brasília time)",
            """
            Returns only meetings currently in progress, based on Brasília time (America/Sao_Paulo).

            **Business rules:**
            - A meeting is "live" if today's weekday bit is set in `DaysOfWeekMask` AND the
              current Brasília time is between `startTime` (inclusive) and `endTime` (inclusive).
            - Results are ordered by start time.
            - Returns an empty list (not 404) when no meetings are live.
            """,
            "Listar reuniões acontecendo agora (horário de Brasília)",
            """
            Retorna apenas as reuniões que estão acontecendo agora, com base no horário de
            Brasília (America/Sao_Paulo).

            **Regras de negócio:**
            - Uma reunião está "ao vivo" se o bit do dia da semana atual está definido em `DaysOfWeekMask`
              E o horário atual de Brasília está entre `startTime` (inclusivo) e `endTime` (inclusivo).
            - Resultados ordenados por horário de início.
            - Retorna lista vazia (não 404) quando nenhuma reunião está ao vivo.
            """
        ),
        ["GetTodayReflection"] = (
            "Get today's daily reflection (Brasília time)",
            """
            Returns the Alcoholics Anonymous daily reflection for today's date in Brasília time.

            **Business rules:**
            - The `dateKey` is in `MM-dd` format (e.g. `04-07` for April 7th), independent of year.
            - There are 365 reflections — one for each day of the year (leap days are excluded).
            - Returns `404` if no reflection exists for today's date key (should not happen in production).
            - Reflections are seeded from `data/daily_reflections_pt-br.json` at startup.
            """,
            "Obter a reflexão diária de hoje (horário de Brasília)",
            """
            Retorna a reflexão diária de Alcoólicos Anônimos para a data de hoje no horário de Brasília.

            **Regras de negócio:**
            - O `dateKey` está no formato `MM-dd` (ex: `04-07` para 7 de abril), independente do ano.
            - Existem 365 reflexões — uma para cada dia do ano (dias bissextos são excluídos).
            - Retorna `404` se nenhuma reflexão for encontrada para o dateKey de hoje (não deve ocorrer em produção).
            - As reflexões são inseridas no banco via `data/daily_reflections_pt-br.json` na inicialização.
            """
        ),
        ["GetReflections"] = (
            "List paginated reflections (app initial seed)",
            """
            Returns a paginated list of all 365 daily reflections, ordered by `dateKey` (chronological).

            **Business rules:**
            - Designed for the mobile app's first launch data seed.
            - `pageSize` is clamped to `[1, 100]`; defaults to 50.
            - `page` starts at 1; values below 1 are treated as 1.
            - `totalCount` in the response allows the client to calculate total pages.
            - `dateKey` format is `MM-dd` (e.g. `01-01` = January 1st).
            """,
            "Listar reflexões paginadas (seed inicial do app)",
            """
            Retorna uma lista paginada das 365 reflexões diárias, ordenadas por `dateKey` (cronológica).

            **Regras de negócio:**
            - Projetado para o seed de dados no primeiro lançamento do app mobile.
            - `pageSize` é limitado a `[1, 100]`; padrão 50.
            - `page` começa em 1; valores abaixo de 1 são tratados como 1.
            - `totalCount` na resposta permite ao cliente calcular o total de páginas.
            - Formato do `dateKey`: `MM-dd` (ex: `01-01` = 1° de janeiro).
            """
        ),
        ["Health"] = (
            "API health check",
            """
            Returns the overall health status of the API and its dependencies.

            **Response fields:**
            - `status` — `"healthy"` if the database is reachable, `"degraded"` otherwise.
            - `database` — `"ok"` or `"error"`.
            - `scraper` — ISO 8601 timestamp of the last successful scraper run, or `"never_run"`.

            Used by load balancers, uptime monitors, and Docker health checks.
            """,
            "Verificação de saúde da API",
            """
            Retorna o estado geral de saúde da API e suas dependências.

            **Campos da resposta:**
            - `status` — `"healthy"` se o banco está acessível, `"degraded"` caso contrário.
            - `database` — `"ok"` ou `"error"`.
            - `scraper` — timestamp ISO 8601 da última execução bem-sucedida do scraper, ou `"never_run"`.

            Utilizado por balanceadores de carga, monitores de uptime e health checks do Docker.
            """
        ),
    };

    // tag name → (EN description, PT-BR description)
    private static readonly Dictionary<string, (string En, string Pt)> Tags = new()
    {
        ["Auth"] = (
            "Anonymous authentication — create or recover a user account by device UUID, with no personal data collected.",
            "Autenticação anônima — cria ou recupera uma conta de usuário pelo UUID do dispositivo, sem coleta de dados pessoais."
        ),
        ["Sync"] = (
            "Data synchronisation — push offline-collected data to the server and pull updated meetings.",
            "Sincronização de dados — envia dados coletados offline ao servidor e busca reuniões atualizadas."
        ),
        ["Users"] = (
            "User management — permanent data deletion (GDPR/LGPD right-to-erasure).",
            "Gerenciamento de usuários — exclusão permanente de dados (direito de exclusão LGPD/GDPR)."
        ),
        ["Meetings"] = (
            "AA online meetings scraped from intergrupos-aa.org.br — full list and live filter.",
            "Reuniões online de A.A. coletadas de intergrupos-aa.org.br — lista completa e filtro ao vivo."
        ),
        ["Reflections"] = (
            "Daily Alcoholics Anonymous reflections — today's reflection and paginated full list.",
            "Reflexões diárias de Alcoólicos Anônimos — reflexão de hoje e lista paginada completa."
        ),
        ["Health"] = (
            "Health checks — API status, database connectivity, and scraper state.",
            "Health checks — status da API, conectividade com o banco e estado do scraper."
        ),
    };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        bool isEn = context.DocumentName == "v1-en";

        // Apply tag descriptions
        swaggerDoc.Tags ??= [];
        foreach (var (tagName, (enDesc, ptDesc)) in Tags)
        {
            var existing = swaggerDoc.Tags.FirstOrDefault(t => t.Name == tagName);
            if (existing is not null)
                existing.Description = isEn ? enDesc : ptDesc;
            else
                swaggerDoc.Tags.Add(new OpenApiTag { Name = tagName, Description = isEn ? enDesc : ptDesc });
        }

        // Apply operation-level descriptions
        foreach (var pathItem in swaggerDoc.Paths.Values)
        {
            foreach (var (_, operation) in pathItem.Operations)
            {
                if (operation.OperationId is null) continue;
                if (!Operations.TryGetValue(operation.OperationId, out var loc)) continue;

                operation.Summary = isEn ? loc.Es : loc.Ps;
                operation.Description = isEn ? loc.Ed : loc.Pd;
            }
        }
    }
}
