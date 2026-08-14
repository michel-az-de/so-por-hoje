# Só Por Hoje

## Fontes

- Consulte o Atlas no projeto `so-por-hoje` e confirme estado no Git e CI.
- Arquitetura e execução: `README.md`.
- Fluxo: `docs/adr/0001-adocao-policy-v4.md`.

## Projeto

- Trunk: `main`.
- Backend .NET 8, ASP.NET Core e PostgreSQL; app MAUI offline-first.
- Preserve anonimato, ausência de PII, idempotência de sync e regras de recuperação.
- Migration, exclusão de dados e Docker exigem necessidade explícita, ambiente confirmado e rollback.

## Gates

Backend e domínio:

```powershell
dotnet build src/SoPorHoje.Tests/SoPorHoje.Tests.csproj --configuration Release
dotnet test src/SoPorHoje.Tests/SoPorHoje.Tests.csproj --configuration Release
```

Mudança MAUI compila explicitamente o target afetado e faz smoke da plataforma. Ambiente local não é deploy.

Uma tarefa usa issue, branch e PR. Preserve WIP e só conclua após aceite, CI, mergeabilidade, merge, limpeza e atualização do Atlas.
