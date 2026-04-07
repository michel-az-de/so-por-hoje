# Só Por Hoje 🕊️

Companheiro diário para membros de Alcoólicos Anônimos no Brasil.

## Funcionalidades

- 📊 Contador de dias sóbrios com fichas do padrão brasileiro de A.A.
- ✋ Compromisso diário ("Só por hoje, eu não vou beber")
- 📖 365 reflexões diárias em pt-BR
- 🔑 Motivo pessoal como âncora emocional
- 🆘 Kit de emergência: respiração guiada, frases de enfrentamento, HALT, CVV
- 🤝 Reuniões online ao vivo (dados do intergrupos-aa.org.br)
- 📖 Programa completo: 12 Passos, 12 Tradições, Promessas, Só Por Hoje, Orações
- 🏅 Sistema de fichas com celebrações
- 🔒 100% privado — dados no dispositivo, sync opcional e anônimo

## Stack

- **App**: .NET MAUI 8 (Android + iOS), MVVM com CommunityToolkit
- **Backend**: ASP.NET Core 8 Minimal API, EF Core + PostgreSQL
- **Banco local**: SQLite via sqlite-net-pcl
- **Scraper**: HtmlAgilityPack + Playwright (intergrupos-aa.org.br)

## Estrutura

```
SoPorHoje.sln
src/
├── SoPorHoje.Core/      # Models, Interfaces, Constants (net8.0)
├── SoPorHoje.Data/      # Repositories SQLite, SyncEngine (net8.0)
├── SoPorHoje.Api/       # API REST ASP.NET Core + EF Core + PostgreSQL (net8.0)
├── SoPorHoje.Scraper/   # Scraper de reuniões (net8.0)
├── SoPorHoje.App/       # .NET MAUI (net8.0-android;net8.0-ios)
└── SoPorHoje.Tests/     # xUnit tests (net8.0)
data/
└── daily_reflections_pt-br.json   # 365 reflexões diárias
docker-compose.yml
```

## Setup Local

### Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- Para o app: Visual Studio 2022+ com workload MAUI, ou VS Code + extensão MAUI

### Backend (API + PostgreSQL)

```bash
docker-compose up -d
# API disponível em http://localhost:5000
# Swagger UI em http://localhost:5000/swagger
```

Ou sem Docker:

```bash
# Requer PostgreSQL 16 local com banco "soporhoje"
cd src/SoPorHoje.Api
dotnet ef database update
dotnet run
```

### Testes (não exigem MAUI)

```bash
cd src
dotnet restore
dotnet build
dotnet test SoPorHoje.Tests/SoPorHoje.Tests.csproj
```

### App (requer workload MAUI)

```bash
# Android (emulador)
dotnet build src/SoPorHoje.App -t:Run -f net8.0-android

# iOS (macOS com Xcode)
dotnet build src/SoPorHoje.App -t:Run -f net8.0-ios
```

## API Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/health` | Health check |
| POST | `/api/auth/anonymous` | Criar/recuperar usuário anônimo |
| GET | `/api/reflections/today` | Reflexão do dia (horário Brasília) |
| GET | `/api/reflections` | Lista paginada de reflexões |
| GET | `/api/meetings` | Todas as reuniões ativas |
| GET | `/api/meetings/live` | Reuniões acontecendo agora |
| POST | `/api/sync/push` | Enviar dados do app ao servidor |
| GET | `/api/sync/pull` | Buscar atualizações do servidor |
| DELETE | `/api/users/{deviceId}` | Deletar dados (GDPR) |

## Privacidade

- IDs de usuário são UUIDs anônimos gerados no device — sem email/senha
- Nenhum dado pessoal identificável é coletado
- Dados locais armazenados em SQLite no dispositivo
- Delete permanente via `DELETE /api/users/{deviceId}`
- Sem analytics de terceiros

## Fichas de Sobriedade (padrão brasileiro)

| Ficha | Tempo | Emoji |
|-------|-------|-------|
| Amarela | Ingresso (1 dia) | 🌅 |
| Azul | 3 meses | 🌊 |
| Rosa | 6 meses | 🌸 |
| Vermelha | 9 meses | 🔥 |
| Verde | 1 ano | 🌿 |
| Verde Gravata | 2 anos | 🌳 |
| Branca Gravata | 5 anos | ⭐ |
| Amarela Gravata | 10 anos | 🏆 |
| Azul Gravata | 15 anos | 💎 |
| Rosa Gravata | 20 anos | 👑 |


