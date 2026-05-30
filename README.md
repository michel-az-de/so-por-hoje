# Só Por Hoje

**Um companheiro diário, gratuito e offline, para quem está em recuperação — um dia de cada vez.**

🌐 soporhoje.org · 📱 Android (iOS em breve) · 🆓 Código aberto · 🔒 Sem login, sem rastreadores

---

## Sobre

**Só Por Hoje** é um aplicativo independente de apoio à recuperação. Nasceu de uma
jornada pessoal e está se tornando um projeto social, público e open source.

> **Não é vinculado a A.A., N.A. ou qualquer outra organização.** É um espaço
> próprio, livre e acolhedor, aberto a quem quiser usar e contribuir.

A ideia é simples: focar em **hoje**. Acompanhar o seu tempo, registrar um
compromisso diário, ler uma reflexão e ter ferramentas de apoio à mão quando o
dia aperta.

## Princípios

- **Offline primeiro.** Funciona sem internet. Seus dados ficam no seu aparelho.
- **Privacidade.** Sem e-mail, sem senha, sem dados pessoais, sem analytics de
  terceiros. A identidade é um ID anônimo gerado no próprio aparelho.
- **Gratuito e sem fins lucrativos.** Sem anúncios, sem cobrança.
- **Aberto e colaborativo.** Qualquer pessoa pode contribuir com código ou conteúdo.

## Funcionalidades

Já no app:

- 🚦 **Check HALT** — uma checagem rápida nos momentos difíceis (Fome, Raiva,
  Solidão, Cansaço)
- 🤝 **Grupos de apoio online** — encontre reuniões acontecendo agora

Em construção (veja o roadmap):

- ⏱️ Contador de tempo de recuperação
- 🌅 Reflexão do dia
- ✍️ Compromisso diário ("só por hoje, eu...")
- 🏅 Marcos de tempo
- 📓 Histórico e diário pessoal

## Como contribuir

Este é um projeto comunitário — toda ajuda é bem-vinda:

- **Código:** abra uma issue ou um Pull Request.
- **Reflexões diárias:** contribua com textos **originais** seguindo
  [`data/reflections/SCHEMA.md`](data/reflections/SCHEMA.md). Veja as regras de
  conteúdo lá (apenas material original ou de domínio público).

Um guia completo de contribuição (`CONTRIBUTING.md`) e o código de conduta estão
a caminho.

## Estrutura do repositório

```
src/
├── SoPorHoje.App/      # App mobile (.NET MAUI) — Android (iOS em breve)
├── SoPorHoje.Core/     # Modelos de domínio (sobriedade, fichas, reflexões)
├── SoPorHoje.Data/     # Camada de dados offline-first (SQLite)
├── SoPorHoje.Api/      # API/back-end (opcional, em revisão de arquitetura)
├── SoPorHoje.Scraper/  # Coletor de reuniões online
└── SoPorHoje.Tests/    # Testes + auditoria de acessibilidade (WCAG)
data/
└── reflections/        # Reflexões diárias (conteúdo aberto, CC BY-SA)
```

## Licença

Licenciamento dividido entre código e conteúdo:

- **Código-fonte:** [MIT](LICENSE)
- **Conteúdo** (reflexões, textos): [CC BY-SA 4.0](CONTENT_LICENSE)

---

*Só por hoje.*
