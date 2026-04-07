namespace SoPorHoje.Api.DTOs;

/// <summary>
/// Resposta do endpoint de verificação de saúde da API.
/// </summary>
/// <param name="Status">Estado geral: "healthy" (banco acessível) ou "degraded" (falha no banco).</param>
/// <param name="Database">Estado do banco de dados: "ok" ou "error".</param>
/// <param name="Scraper">Informação da última execução do scraper ou "never_run" se ainda não executou.</param>
public record HealthResponse(string Status, string Database, string Scraper);
