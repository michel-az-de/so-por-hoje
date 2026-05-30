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

- 🌅 **Hoje** — contador de tempo de recuperação, ficha/marco atual com
  progresso, reflexão do dia e o compromisso diário ("só por hoje, eu...")
- 🚦 **Check HALT** — uma checagem rápida nos momentos difíceis (Fome, Raiva,
  Solidão, Cansaço)
- 📖 **A Trilha** — princípios, combinados, promessas e meditações (conteúdo original)
- 🤝 **Apoio** — reuniões abertas e recursos de crise (CVV 188)

Em construção (veja o roadmap):

- 🏅 Tela de marcos e histórico
- 📓 Diário pessoal
- 🔁 Botão "recomecei hoje"
- 🌙 Modo escuro

## Como contribuir

Este é um projeto comunitário — toda ajuda é bem-vinda:

- **Código:** abra uma issue ou um Pull Request.
- **Reflexões diárias:** contribua com textos **originais** seguindo
  [`data/reflections/SCHEMA.md`](data/reflections/SCHEMA.md). Veja as regras de
  conteúdo lá (apenas material original ou de domínio público).

Veja o guia completo em [`CONTRIBUTING.md`](CONTRIBUTING.md) e o
[Código de Conduta](CODE_OF_CONDUCT.md). Para relatar falhas de segurança,
consulte a [Política de Segurança](SECURITY.md).

## Estrutura do repositório

```
src/
├── SoPorHoje.App/      # App mobile (.NET MAUI) — Android (iOS em breve)
├── SoPorHoje.Core/     # Modelos de domínio (sobriedade, fichas, reflexões)
├── SoPorHoje.Data/     # Camada de dados offline-first (SQLite)
└── SoPorHoje.Tests/    # Testes + auditoria de acessibilidade (WCAG)
data/
└── reflections/        # Reflexões diárias (conteúdo aberto, CC BY-SA)
attic/                  # Código legado fora do build (servidor/scraper)
```

## Licença

Licenciamento dividido entre código e conteúdo:

- **Código-fonte:** [MIT](LICENSE)
- **Conteúdo** (reflexões, textos): [CC BY-SA 4.0](CONTENT_LICENSE)

---

*Só por hoje.*
