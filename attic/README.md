# attic — código arquivado

Esta pasta guarda código que **não faz parte da direção atual** do projeto
(app offline-first, conteúdo estático versionado), mas que foi preservado para
referência. Nada aqui é compilado pela solução `SoPorHoje.sln` nem roda no CI.

## O que está aqui

- **`SoPorHoje.Api/`** — servidor ASP.NET Core + PostgreSQL. A direção pública é
  *offline-first* com feed estático (JSON versionado/CDN); um servidor mínimo só
  será considerado quando houver necessidade real (ex.: reuniões enviadas pela
  comunidade, com moderação). Ver o plano de continuidade.
- **`SoPorHoje.Scraper/`** — coletor de reuniões que dependia de `intergrupos-aa.org.br`.
  Removido do caminho do build por ser acoplado a uma fonte de A.A. e por contrariar
  o princípio de independência. Será substituído por um feed estático de grupos de
  apoio abertos, mantido via Pull Request.
  - `_archived-tests/` — testes e fixtures do scraper (`IntergruposScraperTests.cs`,
    `sample_meetings.html`), movidos junto.

## Como restaurar

O histórico do git preserva tudo. Para trazer um projeto de volta, mova a pasta
para `src/` e re-adicione o projeto à solução (`dotnet sln add`).
