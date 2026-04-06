# so-por-hoje
Projeto para ajudar membros de Alcoólicos Anônimos em sua recuperação diária.

## Sobre

**Só Por Hoje** é um companheiro diário para membros de A.A. no Brasil. Este repositório contém o backend API REST em ASP.NET Core 8 com scraper de reuniões online.

## Estrutura

```
src/
├── SoPorHoje.Api/       # API REST (ASP.NET Core 8 Minimal API + EF Core + PostgreSQL)
├── SoPorHoje.Scraper/   # Scraper de reuniões (intergrupos-aa.org.br)
└── data/
    └── daily_reflections_pt-br.json   # 365 reflexões diárias em português
```

## Setup Rápido com Docker

```bash
cd src
docker-compose -f SoPorHoje.Api/docker-compose.yml up --build
```

A API estará disponível em `http://localhost:5000`.  
Swagger UI em `http://localhost:5000/swagger`.

## Setup para Desenvolvimento Local

**Pré-requisitos:** .NET 8 SDK, PostgreSQL 16

```bash
# 1. Criar banco de dados
createdb soporhoje

# 2. Aplicar migrations
cd src/SoPorHoje.Api
dotnet ef database update

# 3. Rodar a API
dotnet run
```

## Endpoints da API

### Autenticação Anônima

```bash
# Criar/recuperar usuário anônimo pelo deviceId
curl -X POST http://localhost:5000/api/auth/anonymous \
  -H "Content-Type: application/json" \
  -d '{"deviceId": "meu-device-uuid-123"}'
# Resposta: {"userId": "...", "isNew": true}
```

### Sincronização

```bash
# Push — enviar dados do app para o servidor
curl -X POST http://localhost:5000/api/sync/push \
  -H "Content-Type: application/json" \
  -d '{
    "deviceId": "meu-device-uuid-123",
    "profile": {
      "sobrietyDate": "2024-06-15",
      "personalReason": "Pela minha família"
    },
    "pledges": [
      {"pledgeDate": "2026-04-06", "pledgedAt": "2026-04-06T08:30:00Z", "fulfilled": null}
    ],
    "chipEvents": [],
    "resetEvents": []
  }'

# Pull — buscar dados atualizados desde um timestamp
curl "http://localhost:5000/api/sync/pull?since=2026-01-01T00:00:00Z"
```

### Reuniões

```bash
# Todas as reuniões ativas
curl http://localhost:5000/api/meetings

# Reuniões acontecendo agora (horário de Brasília)
curl http://localhost:5000/api/meetings/live
```

### Reflexões

```bash
# Reflexão do dia (horário de Brasília)
curl http://localhost:5000/api/reflections/today

# Lista paginada (seed completo para o app)
curl "http://localhost:5000/api/reflections?page=1&pageSize=50"
```

### Administração

```bash
# Health check
curl http://localhost:5000/api/health

# Deletar todos os dados de um usuário (GDPR)
curl -X DELETE http://localhost:5000/api/users/meu-device-uuid-123
```

## Privacidade

- IDs de usuário são UUIDs anônimos gerados no device — sem email/senha
- Nenhum dado pessoal identificável é coletado
- Delete permanente disponível via `DELETE /api/users/{deviceId}`
- Sem analytics de terceiros

## Scraper de Reuniões

O scraper coleta reuniões do site intergrupos-aa.org.br a cada 30 minutos:

1. Tenta endpoints de API JSON internos do site
2. Fallback: scraping HTML com HtmlAgilityPack
3. Rate limit: máximo 1 request a cada 10 segundos
4. Retorna lista vazia se o site estiver indisponível (nunca crasha)

Ver [SoPorHoje.Scraper/README.md](src/SoPorHoje.Scraper/README.md) para mais detalhes.

