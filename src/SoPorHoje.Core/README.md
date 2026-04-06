# SoPorHoje.Core

Biblioteca de domínio central do app "Só Por Hoje". Contém apenas lógica de negócio pura — sem dependências de UI ou infraestrutura.

## Estrutura

| Pasta | Conteúdo |
|-------|----------|
| `Models/` | Entidades do domínio mapeadas para SQLite |
| `Enums/` | Enumerações tipadas (ex: `DayOfWeekFlag`) |
| `Constants/` | Fichas de sobriedade (`ChipDefinitions`) e textos do A.A. (`AAContent`) |
| `Interfaces/` | Contratos para repositories e serviços |

## Modelos Principais

- **UserProfile** — Perfil do usuário com data de sobriedade e `SoberDays` calculado.
- **DailyPledge** — Compromisso diário "Só Por Hoje" com histórico de cumprimento.
- **DailyReflection** — 365 reflexões oficiais do A.A., indexadas por `"MM-dd"`.
- **SobrietyChip** — Fichas de sobriedade brasileiras (Amarela a Rosa Gravata).
- **ChipEarnedEvent** — Registro de conquista de ficha + flag de celebração exibida.
- **OnlineMeeting** — Reunião online com bitmask de dias e lógica `IsLiveNow`.
- **ResetEvent** — Histórico de recaídas para análise de padrões.

## Dependências

- `sqlite-net-pcl` — apenas pelos atributos `[PrimaryKey]`, `[Indexed]`, `[Ignore]`.
