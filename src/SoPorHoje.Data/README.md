# SoPorHoje.Data

Camada de infraestrutura e dados do app "Só Por Hoje". Implementa os contratos definidos em `SoPorHoje.Core`.

## Estrutura

```
Local/
  SoPorHojeDatabase.cs       — Inicialização lazy do SQLite + seed de fichas
  Repositories/
    UserRepository.cs
    PledgeRepository.cs
    ReflectionRepository.cs
    MeetingRepository.cs
    ChipService.cs

Remote/
  IApiClient.cs              — Interface Refit para a API REST
  ApiClient.cs               — Factory helper
  DTOs/
    SyncPushRequest.cs
    SyncPullResponse.cs
    MeetingDto.cs

Sync/
  SyncEngine.cs              — Orquestrador de sync eventual (offline-first)
```

## Estratégia de Sync

| Entidade | Direção |
|----------|---------|
| `UserProfile` | Push only |
| `DailyPledge` | Push only |
| `ChipEarnedEvent` | Push only |
| `ResetEvent` | Push only |
| `OnlineMeeting` | Pull only |
| `DailyReflection` | Pull only |

O app funciona **100% offline**. O SQLite é sempre a source of truth. Sync é eventual e não-bloqueante.

## Registro de Dependências (DI)

```csharp
services.AddSingleton<SoPorHojeDatabase>(sp =>
    new SoPorHojeDatabase(dbPath, sp.GetRequiredService<ILogger<SoPorHojeDatabase>>()));

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPledgeRepository, PledgeRepository>();
services.AddScoped<IReflectionRepository, ReflectionRepository>();
services.AddScoped<IMeetingRepository, MeetingRepository>();
services.AddScoped<IChipService, ChipService>();

services.AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.soporhoje.app"));
services.AddScoped<ISyncService, SyncEngine>();
```
