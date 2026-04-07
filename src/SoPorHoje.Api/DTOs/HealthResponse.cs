namespace SoPorHoje.Api.DTOs;

/// <summary>
/// Response from the API health-check endpoint.
/// / Resposta do endpoint de verificação de saúde da API.
/// </summary>
/// <param name="Status">
/// Overall status: "healthy" (database reachable) or "degraded" (database failure).
/// / Estado geral: "healthy" (banco acessível) ou "degraded" (falha no banco).
/// </param>
/// <param name="Database">
/// Database connectivity status: "ok" or "error".
/// / Estado da conexão com o banco de dados: "ok" ou "error".
/// </param>
/// <param name="Scraper">
/// Timestamp of the last scraper run (ISO-8601) or "never_run" if it has not executed yet.
/// / Horário da última execução do scraper (ISO-8601) ou "never_run" se ainda não executou.
/// </param>
public record HealthResponse(string Status, string Database, string Scraper);
