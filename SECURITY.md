# Política de Segurança e Privacidade

## Privacidade por desenho

O **Só Por Hoje** foi feito para proteger quem o usa. Por desenho:

- **Sem dados pessoais.** Não pedimos nome, e-mail, telefone ou login.
- **Sem servidor.** O app é *offline-first*: o SQLite no aparelho é a fonte da
  verdade. Hoje não há back-end recebendo dados de usuários.
- **Sem rastreadores.** Nenhum analytics de terceiros, nenhum identificador de
  publicidade. A identidade é um **UUID anônimo gerado no próprio aparelho**.
- **Conteúdo verificável.** Reflexões e textos são versionados e abertos.

Como os dados ficam apenas no aparelho, desinstalar o app remove tudo. Faça
backups por sua conta se quiser preservar seu histórico.

## Versões suportadas

O projeto está em desenvolvimento ativo. Correções de segurança são aplicadas
sobre a branch `main` e a última versão publicada.

## Como relatar uma vulnerabilidade

**Não abra uma issue pública** para falhas de segurança.

- Use o **"Report a vulnerability"** em *Security › Advisories* deste
  repositório (canal privado do GitHub), **ou**
- escreva para **seguranca@soporhoje.org**
  *(ajuste este endereço quando o domínio estiver configurado)*.

Inclua passos para reproduzir, impacto e, se possível, uma sugestão de correção.
Faremos o possível para responder em até **7 dias** e manter você informado até
a resolução. Pedimos divulgação responsável: dê-nos tempo para corrigir antes de
tornar a falha pública.

## Escopo

- App Android (`SoPorHoje.App`) e as camadas `Core`/`Data`.
- Fora de escopo: código legado em `attic/` (servidor/scraper, não publicado).

Obrigado por ajudar a manter a comunidade segura.
