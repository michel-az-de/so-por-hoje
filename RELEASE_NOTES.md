# Release Notes — Só Por Hoje

## Versão 1.0.0

**Data:** Abril 2026  
**Plataformas:** Android 5.0+ (API 21), iOS 14.2+

---

### Funcionalidades

#### 📊 Contador de Sobriedade
- Cálculo de dias sóbrios baseado na data de sobriedade informada pelo usuário
- Exibição formatada com dias e meses
- Nunca exibe valor negativo

#### 🏅 Fichas de Sobriedade
- Sistema completo de 10 fichas do padrão brasileiro de A.A.
- Barra de progresso para a próxima ficha
- Celebração animada ao conquistar nova ficha (exibida apenas uma vez)

#### ✋ Compromisso Diário
- Botão "Só por hoje, eu não vou beber" com persistência local
- Compromisso diário — reinicia a cada dia
- Streak de compromissos consecutivos

#### 📖 Reflexões Diárias
- 365 reflexões em português, uma por dia
- Acesso offline via banco SQLite local
- Seed automático na primeira abertura

#### 🔑 Motivo Pessoal
- Campo editável para registrar a âncora emocional
- Persistido localmente, sem servidor

#### 🆘 Kit de Emergência (SOS)
- Respiração guiada: 4 ciclos de inspire (4s) → segure (4s) → expire (6s)
- Frases de enfrentamento
- Verificação HALT (Fome, Raiva, Solidão, Cansaço)
- Acesso direto ao CVV 188
- Link para reunião ao vivo, quando disponível

#### 🤝 Reuniões Online
- Listagem de reuniões online de AA do Brasil
- Detecção de reuniões ao vivo em tempo real (atualização a cada 30s)
- Links de acesso direto
- Dados sincronizados do intergrupos-aa.org.br

#### 📖 Programa Completo
- 12 Passos de Alcoólicos Anônimos
- 12 Tradições
- Promessas do A.A.
- Meditações "Só Por Hoje"
- Orações (incluindo Oração da Serenidade)
- Verificação HALT expandida

#### 🔄 Sync Opcional
- Sincronização anônima com servidor via ID de dispositivo
- Funciona 100% offline
- Falha silenciosa quando sem internet

---

### Privacidade e Segurança

- Sem coleta de email, nome, telefone ou localização GPS
- ID anônimo por dispositivo (UUID)
- Dados locais armazenados em SQLite
- Delete permanente via `DELETE /api/users/{deviceId}`
- Sem analytics de terceiros

---

### Arquitetura

```
SoPorHoje.Core     → Models, Interfaces, Constants
SoPorHoje.Data     → SQLite Repositories, SyncEngine
SoPorHoje.Scraper  → Web scraper de reuniões
SoPorHoje.Api      → ASP.NET Core Minimal API + PostgreSQL
SoPorHoje.App      → .NET MAUI (Android + iOS)
SoPorHoje.Tests    → xUnit (128 testes)
```

---

### Build e Testes

```bash
# Restaurar e compilar (sem MAUI)
cd src && dotnet restore && dotnet build

# Todos os testes
cd src && dotnet test SoPorHoje.Tests/SoPorHoje.Tests.csproj

# Backend com Docker
docker-compose up -d
```

---

### Problemas Conhecidos

- O app MAUI requer workload .NET MAUI instalado (`dotnet workload install maui`)
- Ícones de tab bar (tab_home.png, tab_chips.png, tab_sos.png) precisam ser adicionados aos recursos de imagem do projeto App antes do build
- Fontes DMSans e SourceSerif4 precisam ser adicionadas à pasta `Resources/Fonts/` antes do build
- O sync com servidor é opcional e requer configuração de URL da API em `appsettings.json`
- O scraper de reuniões pode retornar lista vazia se o site do intergrupos estiver indisponível

---

### Próximos Passos (v1.1)

- [ ] Notificação diária (push) lembrando o compromisso
- [ ] Widget para tela de bloqueio (iOS) com contador de dias
- [ ] Tema escuro
- [ ] Exportar histórico de pledges em PDF
- [ ] Suporte a múltiplos idiomas (espanhol, inglês)
