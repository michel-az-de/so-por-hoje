# SoPorHoje.Scraper

Biblioteca de scraping de reuniões online de A.A. do site intergrupos-aa.org.br.

## Estratégia de Scraping

1. **API JSON interna** — tenta endpoints de API REST do site diretamente (mais confiável)
2. **HTML scraping** — usa HtmlAgilityPack para parsear links de reunião (Zoom/Meet/Teams)
3. **Playwright (não implementado por padrão)** — para SPAs que requerem JavaScript

## Uso

```csharp
var scraper = new IntergruposScraper(httpClient, logger);
var meetings = await scraper.ScrapeAsync(cancellationToken);
```

## Rate Limiting

- Máximo 1 request a cada 10 segundos
- User-Agent identificável: `SoPorHoje-App/1.0 (+soporhoje)`
- Timeout de 30 segundos por request

## Estrutura de DaysOfWeekMask

Bitmask onde cada bit representa um dia da semana (começando no domingo):

| Bit | Dia       |
|-----|-----------|
| 0   | Domingo   |
| 1   | Segunda   |
| 2   | Terça     |
| 3   | Quarta    |
| 4   | Quinta    |
| 5   | Sexta     |
| 6   | Sábado    |

Exemplo: `127` = todos os dias (1111111 em binário)
