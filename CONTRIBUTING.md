# Contribuindo com o Só Por Hoje

Obrigado por querer ajudar 💛 O **Só Por Hoje** é um projeto comunitário,
gratuito e independente de apoio à recuperação. Toda contribuição — código,
conteúdo, tradução, design ou um simples relato — é bem-vinda.

Antes de tudo, leia e respeite o nosso [Código de Conduta](CODE_OF_CONDUCT.md).
Este é um espaço sensível: muita gente que usa (e constrói) este app está em
recuperação. Acolhimento e ausência de julgamento vêm primeiro.

## Princípios que guiam o projeto

- **Independente** — sem vínculo com A.A., N.A. ou qualquer organização.
- **Offline primeiro** — o app funciona sem internet; os dados ficam no aparelho.
- **Anônimo** — sem login, e-mail ou dados pessoais.
- **Conteúdo original** — nada de literatura protegida (ver "Conteúdo" abaixo).

## Formas de contribuir

| Tipo | Como |
|------|------|
| 🐛 Relatar bug | Abra uma [issue](../../issues/new/choose) |
| 💡 Sugerir ideia | Abra uma issue de proposta |
| 🌅 Reflexões diárias | PR em `data/reflections/` (ver abaixo) |
| 🤝 Reuniões abertas | PR no feed de reuniões (ver abaixo) |
| 💻 Código | PR (ver "Desenvolvimento") |
| 🌍 Tradução · ♿ Acessibilidade | Issue ou PR |

## Desenvolvimento

**Requisitos:** .NET 9 SDK e o workload MAUI/Android.

```bash
dotnet workload install maui-android
```

**Build e testes:**

```bash
# Testes (Core + Data) — o mesmo que o CI roda
dotnet test src/SoPorHoje.Tests/SoPorHoje.Tests.csproj

# App Android
dotnet build src/SoPorHoje.App/SoPorHoje.App.csproj
```

Estrutura: `SoPorHoje.App` (MAUI/MVVM com CommunityToolkit.Mvvm),
`SoPorHoje.Core` (modelos de domínio), `SoPorHoje.Data` (SQLite offline-first),
`SoPorHoje.Tests` (xUnit + auditoria de acessibilidade). Código legado de
servidor/scraper vive em `attic/`, fora do build.

**Estilo de código:** siga o padrão dos arquivos vizinhos. ViewModels usam
`[ObservableProperty]`/`[RelayCommand]`; toda tela precisa de `SemanticProperties`
para leitores de tela; alvos de toque ≥ 44×44.

## Conteúdo: reflexões diárias

As reflexões são **conteúdo original** sob **CC BY-SA 4.0**.

1. Edite o arquivo do mês em `data/reflections/<idioma>/MM-mes.json`.
2. Siga o schema e as **regras de conteúdo** em
   [`data/reflections/SCHEMA.md`](data/reflections/SCHEMA.md) — apenas material
   **original ou de domínio público**. **Nunca** copie textos de A.A./N.A. nem
   de outras obras protegidas, e não cite páginas de livros protegidos.
3. Tom acolhedor, sem julgamento, espiritualmente aberto, sem jargão de programa.
4. A validação de schema roda no CI; PRs fora do schema falham automaticamente.

## Conteúdo: reuniões abertas

Reuniões de grupos de apoio **abertos** são publicadas como feed estático
versionado e passam por **moderação** antes de entrar. Envie por PR informando:
nome do grupo, dias e horário (com fuso), link da sala e uma forma de verificação.
Não publicamos reuniões de organizações fechadas ou protegidas.

## Proveniência: sign-off (DCO)

Para proteger o projeto e seus usuários, adotamos o **Developer Certificate of
Origin**. Acrescente `-s` aos seus commits:

```bash
git commit -s -m "sua mensagem"
```

Isso adiciona um `Signed-off-by:` afirmando que você tem o direito de contribuir
aquele código ou conteúdo sob as licenças do projeto.

## Licença das contribuições

Ao contribuir, você concorda em licenciar:

- **código** sob [MIT](LICENSE);
- **conteúdo** (textos, reflexões) sob [CC BY-SA 4.0](CONTENT_LICENSE).

## Processo de Pull Request

1. Faça um fork e crie uma branch descritiva.
2. Mantenha o PR pequeno e focado; descreva o "porquê".
3. Garanta o **CI verde** (build + testes + validação de schema).
4. Para mudanças de UI, descreva o impacto em acessibilidade.

Dúvidas? Abra uma issue. Obrigado por caminhar junto. *Só por hoje.*
