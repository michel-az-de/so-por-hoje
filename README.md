# Só Por Hoje — Backend API

> **English** · [Português](#português)

---

## English

### About

**Só Por Hoje** ("Just For Today") is a daily recovery companion for Alcoholics Anonymous members in Brazil.  
This repository contains the backend REST API and the online-meeting scraper.

### Architecture

```
src/
├── SoPorHoje.Api/        # REST API — ASP.NET Core 8 Minimal API + EF Core + PostgreSQL
├── SoPorHoje.Scraper/    # Meeting scraper — intergrupos-aa.org.br (every 30 min)
├── SoPorHoje.Core/       # Shared domain models, interfaces, constants, enums
├── SoPorHoje.Data/       # SQLite repositories + Refit API client (mobile app layer)
├── SoPorHoje.Tests/      # xUnit tests — unit, integration, accessibility (128 tests)
└── data/
    └── daily_reflections_pt-br.json   # 365 daily reflections (Portuguese)
```

**Technology stack**

| Layer | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core 8 Minimal API |
| Database | PostgreSQL 16 + Entity Framework Core 8 |
| Background jobs | `IHostedService` — scraper runs every 30 min |
| Authentication | Anonymous UUID — no email, no password, no PII |
| Swagger | Swashbuckle 6 — bilingual UI (EN / pt-br) |
| Mobile layer | sqlite-net-pcl + Refit |
| Tests | xUnit + FluentAssertions + Moq |

---

### Quick Start with Docker

```bash
cd src
docker-compose -f SoPorHoje.Api/docker-compose.yml up --build
```

- API: `http://localhost:5000`  
- Swagger UI (English): `http://localhost:5000/swagger` → select **"Só Por Hoje API — English"**  
- Swagger UI (Português): `http://localhost:5000/swagger` → select **"Só Por Hoje API — Português"**

---

### Local Development Setup

**Prerequisites:** .NET 8 SDK, PostgreSQL 16

```bash
# 1. Create the database
createdb soporhoje

# 2. Apply migrations
cd src/SoPorHoje.Api
dotnet ef database update

# 3. Run the API
dotnet run
```

**Run tests:**

```bash
cd src
dotnet test SoPorHoje.Tests/SoPorHoje.Tests.csproj
```

---

### Business Rules

#### Authentication
- Users are identified by a **device UUID** generated on the client — no email or password is ever required.
- Calling `POST /api/auth/anonymous` is idempotent: the same `deviceId` always returns the same `userId`.

#### Offline-first Sync
- The mobile app stores all data in **SQLite** on the device — the server is a backup/sync layer only.
- **Push** (`POST /api/sync/push`): device sends locally-collected pledges, chip events, and sobriety resets to the server. Upsert semantics — no duplicates.
- **Pull** (`GET /api/sync/pull`): device fetches updated meetings. Pass `?since=<ISO8601>` for incremental sync.

#### Daily Pledges
- Each day the user makes a "just for today" pledge to stay sober.
- `fulfilled` is a three-state field: `null` = unanswered, `true` = kept, `false` = broken.
- One pledge per user per calendar date (unique constraint enforced on the server).

#### Sobriety Chips — Brazilian AA Standard

| Days sober | Chip | Label |
|---|---|---|
| 1 | 🌅 Amarela | Ingresso |
| 90 | 🌊 Azul | 3 Meses |
| 180 | 🌸 Rosa | 6 Meses |
| 270 | 🔥 Vermelha | 9 Meses |
| 365 | 🌿 Verde | 1 Ano |
| 730 | 🌳 Verde Gravata | 2 Anos |
| 1 825 | ⭐ Branca Gravata | 5 Anos |
| 3 650 | 🏆 Amarela Gravata | 10 Anos |
| 5 475 | 💎 Azul Gravata | 15 Anos |
| 7 300 | 👑 Rosa Gravata | 20 Anos |

#### DaysOfWeekMask Bitmask

Meetings use a bitmask to encode which days they occur:

| Bit | Value | Day |
|---|---|---|
| 0 | 1 | Sunday |
| 1 | 2 | Monday |
| 2 | 4 | Tuesday |
| 3 | 8 | Wednesday |
| 4 | 16 | Thursday |
| 5 | 32 | Friday |
| 6 | 64 | Saturday |

Example: `127` = all days (1+2+4+8+16+32+64).

#### Timezones
All API datetimes are **UTC** on the wire. Display conversion to **Brasília time** (`America/Sao_Paulo`) happens on the client and in the `isLiveNow` / `reflections/today` logic on the server.

#### Meeting Scraper
- Scrapes `intergrupos-aa.org.br` every **30 minutes** via `ScraperHostedService`.
- Tries the site's internal JSON API first; falls back to HTML scraping with HtmlAgilityPack.
- Rate limit: max 1 request / 10 seconds.
- If the site is unavailable, returns an empty list — never crashes the API.
- A meeting is **deactivated** (not deleted) when it no longer appears in scraper results.

#### Privacy / LGPD / GDPR
- No PII is ever collected — only anonymous UUIDs generated on the device.
- `DELETE /api/users/{deviceId}` permanently and irreversibly erases all user data (cascade delete).

---

### API Reference

#### Auth

```bash
# Create or retrieve anonymous user
curl -X POST http://localhost:5000/api/auth/anonymous \
  -H "Content-Type: application/json" \
  -d '{"deviceId": "my-device-uuid-123"}'
# → {"userId": "...", "isNew": true}
```

#### Sync

```bash
# Push local data
curl -X POST http://localhost:5000/api/sync/push \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "my-device-uuid-123",
    "profile": { "sobrietyDate": "2024-06-15", "personalReason": "For my family" },
    "pledges": [
      { "pledgeDate": "2026-04-07", "pledgedAt": "2026-04-07T08:00:00Z", "fulfilled": null }
    ],
    "chipEvents": [],
    "resetEvents": []
  }'

# Pull updated meetings (incremental)
curl "http://localhost:5000/api/sync/pull?since=2026-01-01T00:00:00Z"

# Full pull (no since)
curl "http://localhost:5000/api/sync/pull"
```

#### Meetings

```bash
# All active meetings
curl http://localhost:5000/api/meetings

# Meetings live right now (Brasília time)
curl http://localhost:5000/api/meetings/live
```

#### Reflections

```bash
# Today's reflection (Brasília time)
curl http://localhost:5000/api/reflections/today

# Paginated list (app seed)
curl "http://localhost:5000/api/reflections?page=1&pageSize=50"
```

#### Admin / Health

```bash
# Health check
curl http://localhost:5000/api/health

# Permanently delete all user data (GDPR/LGPD)
curl -X DELETE http://localhost:5000/api/users/my-device-uuid-123
```

---

---

## Português

### Sobre

**Só Por Hoje** é um companheiro diário de recuperação para membros de Alcoólicos Anônimos no Brasil.  
Este repositório contém a API REST de backend e o scraper de reuniões online.

### Arquitetura

```
src/
├── SoPorHoje.Api/        # API REST — ASP.NET Core 8 Minimal API + EF Core + PostgreSQL
├── SoPorHoje.Scraper/    # Scraper de reuniões — intergrupos-aa.org.br (a cada 30 min)
├── SoPorHoje.Core/       # Modelos de domínio, interfaces, constantes e enums compartilhados
├── SoPorHoje.Data/       # Repositórios SQLite + cliente Refit (camada do app mobile)
├── SoPorHoje.Tests/      # Testes xUnit — unit, integração, acessibilidade (128 testes)
└── data/
    └── daily_reflections_pt-br.json   # 365 reflexões diárias em português
```

**Stack de tecnologia**

| Camada | Tecnologia |
|---|---|
| Runtime | .NET 8 / ASP.NET Core 8 Minimal API |
| Banco de dados | PostgreSQL 16 + Entity Framework Core 8 |
| Jobs em background | `IHostedService` — scraper a cada 30 min |
| Autenticação | UUID anônimo — sem email, sem senha, sem dados pessoais |
| Swagger | Swashbuckle 6 — UI bilíngue (EN / pt-br) |
| Camada mobile | sqlite-net-pcl + Refit |
| Testes | xUnit + FluentAssertions + Moq |

---

### Início Rápido com Docker

```bash
cd src
docker-compose -f SoPorHoje.Api/docker-compose.yml up --build
```

- API: `http://localhost:5000`  
- Swagger UI (Inglês): `http://localhost:5000/swagger` → selecione **"Só Por Hoje API — English"**  
- Swagger UI (Português): `http://localhost:5000/swagger` → selecione **"Só Por Hoje API — Português"**

---

### Setup para Desenvolvimento Local

**Pré-requisitos:** .NET 8 SDK, PostgreSQL 16

```bash
# 1. Criar o banco de dados
createdb soporhoje

# 2. Aplicar migrations
cd src/SoPorHoje.Api
dotnet ef database update

# 3. Rodar a API
dotnet run
```

**Executar os testes:**

```bash
cd src
dotnet test SoPorHoje.Tests/SoPorHoje.Tests.csproj
```

---

### Regras de Negócio

#### Autenticação
- Os usuários são identificados por um **UUID de dispositivo** gerado no cliente — nunca é solicitado email ou senha.
- Chamar `POST /api/auth/anonymous` é idempotente: o mesmo `deviceId` sempre retorna o mesmo `userId`.

#### Sincronização Offline-first
- O app mobile armazena todos os dados em **SQLite** no dispositivo — o servidor é uma camada de backup/sync apenas.
- **Push** (`POST /api/sync/push`): o dispositivo envia pledges, fichas e resets coletados localmente. Semântica de upsert — sem duplicatas.
- **Pull** (`GET /api/sync/pull`): o dispositivo busca reuniões atualizadas. Passe `?since=<ISO8601>` para sync incremental.

#### Compromissos Diários (Pledges)
- A cada dia o usuário faz um compromisso "só por hoje" de manter a sobriedade.
- `fulfilled` é um campo de três estados: `null` = não respondido, `true` = cumprido, `false` = não cumprido.
- Um pledge por usuário por data calendário (constraint de unicidade no servidor).

#### Fichas de Sobriedade — Padrão Brasileiro de A.A.

| Dias sóbrio | Ficha | Rótulo |
|---|---|---|
| 1 | 🌅 Amarela | Ingresso |
| 90 | 🌊 Azul | 3 Meses |
| 180 | 🌸 Rosa | 6 Meses |
| 270 | 🔥 Vermelha | 9 Meses |
| 365 | 🌿 Verde | 1 Ano |
| 730 | 🌳 Verde Gravata | 2 Anos |
| 1.825 | ⭐ Branca Gravata | 5 Anos |
| 3.650 | 🏆 Amarela Gravata | 10 Anos |
| 5.475 | 💎 Azul Gravata | 15 Anos |
| 7.300 | 👑 Rosa Gravata | 20 Anos |

#### Bitmask DaysOfWeekMask

As reuniões usam um bitmask para codificar em quais dias ocorrem:

| Bit | Valor | Dia |
|---|---|---|
| 0 | 1 | Domingo |
| 1 | 2 | Segunda-feira |
| 2 | 4 | Terça-feira |
| 3 | 8 | Quarta-feira |
| 4 | 16 | Quinta-feira |
| 5 | 32 | Sexta-feira |
| 6 | 64 | Sábado |

Exemplo: `127` = todos os dias (1+2+4+8+16+32+64).

#### Fusos Horários
Todos os datetimes da API são **UTC** no protocolo. A conversão para o **horário de Brasília** (`America/Sao_Paulo`) ocorre no cliente e na lógica de `isLiveNow` / `reflexoes/today` no servidor.

#### Scraper de Reuniões
- Faz scraping do `intergrupos-aa.org.br` a cada **30 minutos** via `ScraperHostedService`.
- Tenta primeiro a API JSON interna do site; fallback para scraping HTML com HtmlAgilityPack.
- Rate limit: máximo 1 request a cada 10 segundos.
- Se o site estiver indisponível, retorna lista vazia — nunca derruba a API.
- Uma reunião é **desativada** (não deletada) quando deixa de aparecer nos resultados do scraper.

#### Privacidade / LGPD / GDPR
- Nenhum dado pessoal identificável é coletado — apenas UUIDs anônimos gerados no dispositivo.
- `DELETE /api/users/{deviceId}` apaga permanente e irreversivelmente todos os dados do usuário (exclusão em cascata).

---

### Referência da API

#### Autenticação

```bash
# Criar ou recuperar usuário anônimo
curl -X POST http://localhost:5000/api/auth/anonymous \
  -H "Content-Type: application/json" \
  -d '{"deviceId": "meu-device-uuid-123"}'
# → {"userId": "...", "isNew": true}
```

#### Sincronização

```bash
# Push — enviar dados locais ao servidor
curl -X POST http://localhost:5000/api/sync/push \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "meu-device-uuid-123",
    "profile": { "sobrietyDate": "2024-06-15", "personalReason": "Pela minha família" },
    "pledges": [
      { "pledgeDate": "2026-04-07", "pledgedAt": "2026-04-07T08:00:00Z", "fulfilled": null }
    ],
    "chipEvents": [],
    "resetEvents": []
  }'

# Pull — buscar reuniões atualizadas (incremental)
curl "http://localhost:5000/api/sync/pull?since=2026-01-01T00:00:00Z"

# Pull completo (sem since)
curl "http://localhost:5000/api/sync/pull"
```

#### Reuniões

```bash
# Todas as reuniões ativas
curl http://localhost:5000/api/meetings

# Reuniões acontecendo agora (horário de Brasília)
curl http://localhost:5000/api/meetings/live
```

#### Reflexões

```bash
# Reflexão de hoje (horário de Brasília)
curl http://localhost:5000/api/reflections/today

# Lista paginada (seed do app)
curl "http://localhost:5000/api/reflections?page=1&pageSize=50"
```

#### Admin / Health

```bash
# Health check
curl http://localhost:5000/api/health

# Deletar permanentemente todos os dados do usuário (LGPD/GDPR)
curl -X DELETE http://localhost:5000/api/users/meu-device-uuid-123
```

---

### Variáveis de Configuração

| Variável | Descrição | Padrão |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | String de conexão PostgreSQL | `Host=localhost;Database=soporhoje;...` |
| `Cors:AllowedOrigins` | Origens CORS permitidas (array JSON) | `["*"]` |
| `ReflectionsDataPath` | Caminho absoluto para o JSON de reflexões | Auto-detectado |

Ver `src/SoPorHoje.Api/appsettings.json` para exemplo completo.

---

### Scraper

Ver [SoPorHoje.Scraper/README.md](src/SoPorHoje.Scraper/README.md) para detalhes sobre a estratégia de scraping.

