# Reflexões diárias — formato e regras

As reflexões diárias do **Só Por Hoje** são **conteúdo original**, sob licença
**CC BY-SA 4.0** (ver [`CONTENT_LICENSE`](../../CONTENT_LICENSE)). Uma reflexão
por dia do ano.

## Organização

- Um arquivo por mês: `data/reflections/<idioma>/MM-<mes>.json`
  (ex.: `data/reflections/pt-BR/01-janeiro.json`).
- Cada arquivo é um **array JSON** de reflexões.

## Schema de cada reflexão

| campo     | tipo   | obrigatório | descrição |
|-----------|--------|-------------|-----------|
| `date`    | string | sim | Dia do ano no formato `MM-DD` (sem ano — repete todo ano). |
| `title`   | string | sim | Tema curto (ex.: `"Aceitação"`). |
| `quote`   | string | sim | Frase curta de abertura. **Original** ou de domínio público (com atribuição). |
| `text`    | string | sim | A reflexão (2 a 5 frases), tom acolhedor. |
| `theme`   | string | sim | Slug do tema (ex.: `aceitacao`) para agrupar/buscar. |
| `license` | string | sim | Sempre `CC-BY-SA-4.0`. |
| `author`  | string | sim | Autoria (ex.: `Projeto Só Por Hoje` ou seu usuário). |

## Regras de conteúdo

1. **Somente conteúdo original** ou de domínio público. Nunca copie textos de
   A.A./N.A. ou de outras obras protegidas, nem cite páginas de livros protegidos.
2. Sem vínculo com nenhuma organização específica.
3. Tom acolhedor, livre de julgamento. Espiritualmente aberto, sem impor religião.
4. Evite jargão de programas específicos (ex.: "padrinho", "Poder Superior", "os passos").
5. Linguagem inclusiva e acessível.

## Exemplo

```json
{
  "date": "01-01",
  "title": "Aceitação",
  "quote": "Aceitar não é desistir; é parar de lutar contra o que já é.",
  "text": "Hoje eu escolho ver a vida como ela é, e não como eu gostaria que fosse...",
  "theme": "aceitacao",
  "license": "CC-BY-SA-4.0",
  "author": "Projeto Só Por Hoje"
}
```

## Status

Primeiro lote (janeiro, dias **1–14**) disponível como template de curadoria.
Faltam os demais dias — contribuições bem-vindas via Pull Request.
