namespace SoPorHoje.App.Constants;

/// <summary>
/// Conteúdo original do programa "Só Por Hoje": A Trilha, Combinados,
/// As Promessas, a prece "Só Por Hoje", Meditações e o Check HALT.
///
/// Texto autoral sob licença CC BY-SA 4.0 (ver CONTENT_LICENSE), exceto:
///  - "Só Por Hoje" (prece atribuída a Sibyl F. Partridge, 1916 — domínio público);
///  - "Oração de São Francisco" (tradicional — domínio público).
/// Não contém literatura protegida de A.A./N.A.
/// </summary>
public static class ProgramContent
{
    /// <summary>A Trilha — princípios originais de recuperação (Título, Texto).</summary>
    public static readonly IReadOnlyList<(string Title, string Text)> Trilha = new[]
    {
        ("Honestidade",
         "Começo admitindo a verdade sobre onde estou. Sem honestidade comigo mesmo, nenhuma mudança se sustenta."),
        ("Abertura",
         "Aceito que não preciso fazer isto sozinho. Estar aberto à ajuda — de pessoas e do que faça sentido para mim — abre caminhos que o orgulho fecha."),
        ("Entrega",
         "Solto o controle sobre o que não posso mudar. Brigar com a realidade me esgota; aceitá-la devolve a minha energia para o que importa."),
        ("Autoconhecimento",
         "Olho para dentro com coragem e sem me destruir. Reconhecer meus padrões é o que me permite escolher diferente."),
        ("Responsabilidade",
         "Assumo a minha parte, sem culpa que paralisa e sem desculpas que adiam. O que é meu, eu posso mudar."),
        ("Reparação",
         "Onde causei dano, reparo no que for possível — começando por não causar mais. Cuidar das minhas relações é cuidar de mim."),
        ("Presença",
         "Vivo um dia de cada vez. O hoje é o único lugar onde a recuperação acontece de verdade."),
        ("Constância",
         "Repito o cuidado todos os dias, inclusive nos dias sem vontade. A constância, e não a perfeição, é o que me mantém de pé."),
        ("Compaixão",
         "Trato a mim e aos outros com a gentileza que gostaria de receber. Cair faz parte do caminho — o que importa é voltar."),
        ("Sentido",
         "Transformo a minha experiência em algo que ajuda — a mim e a quem caminha comigo. Servir dá sentido à jornada."),
    };

    /// <summary>Combinados — compromissos diários comigo mesmo.</summary>
    public static readonly IReadOnlyList<string> Combinados = new[]
    {
        "Só por hoje, não bebo — aconteça o que acontecer. Se o dia ficar grande demais, encurto para a próxima hora.",
        "Só por hoje, peço ajuda quando precisar. Pedir não é fraqueza; é como eu me protejo.",
        "Só por hoje, cuido do básico: como, bebo água, durmo e respiro. Um corpo cuidado sustenta uma mente mais firme.",
        "Só por hoje, falo com alguém de confiança. A conexão me tira do isolamento.",
        "Só por hoje, evito decisões importantes quando estou alterado — com fome, raiva, solidão ou cansaço.",
        "Só por hoje, sou honesto comigo sobre como realmente estou.",
        "Só por hoje, faço ao menos uma coisa que cuida de mim, por menor que seja.",
        "Só por hoje, perdoo a mim mesmo por não ser perfeito e recomeço de onde estou.",
        "Só por hoje, lembro por que comecei e por quem — inclusive por mim.",
    };

    /// <summary>As Promessas — o que a recuperação devolve (texto original).</summary>
    public static readonly IReadOnlyList<string> Promises = new[]
    {
        "Aos poucos, a vergonha dá lugar ao respeito por mim mesmo.",
        "A névoa passa, e eu volto a sentir as coisas como elas são.",
        "Descubro uma liberdade que não conhecia: a de não precisar fugir.",
        "O medo do amanhã perde força quando aprendo a cuidar do hoje.",
        "Reencontro pessoas e relações que a dependência havia afastado.",
        "A minha dor passada vira experiência capaz de ajudar outra pessoa.",
        "Volto a confiar em mim — uma escolha de cada vez.",
        "A paz deixa de ser uma palavra distante e começa a ser um lugar onde eu vivo.",
        "Percebo que sou capaz de coisas que achei ter perdido para sempre.",
        "Um dia, sem perceber, descubro que estou vivendo — não apenas resistindo.",
    };

    /// <summary>Prece "Só Por Hoje" (Sibyl F. Partridge, 1916 — domínio público).</summary>
    public static readonly IReadOnlyList<string> JustForToday = new[]
    {
        "Só por hoje, tentarei viver somente neste dia, sem querer resolver o problema de toda a minha vida de uma vez.",
        "Só por hoje, serei feliz. Isto pressupõe que é verdade o que Abraham Lincoln disse: 'A maioria das pessoas é tão feliz quanto se resolve a ser.'",
        "Só por hoje, me ajustarei ao que é, e não tentarei ajustar todas as coisas ao meu desejo.",
        "Só por hoje, cuidarei do meu físico. Farei algum exercício, cuidarei da minha alimentação e não abusarei do meu corpo nem o negligenciarei.",
        "Só por hoje, tentarei fortalecer a minha mente. Aprenderei algo útil. Não serei um preguiçoso mental. Lerei algo que requeira esforço, reflexão e concentração.",
        "Só por hoje, exercitarei a minha alma de três maneiras: farei a alguém algum bem sem que ele o saiba; farei pelo menos duas coisas que não gosto de fazer — só por exercício; e não mostrarei a ninguém que meus sentimentos foram feridos.",
        "Só por hoje, serei agradável. Terei o melhor aspecto que puder; falarei com calma; agirei com cortesia; não criticarei ninguém e não tentarei regular nem melhorar ninguém.",
        "Só por hoje, terei um plano. Anotarei o que espero fazer. Pode ser que não o cumpra exatamente, mas terei um plano — evitarei dois males: a pressa e a indecisão.",
        "Só por hoje, terei um momento de tranquilidade e contemplação. Relaxarei. Neste momento, verei a minha vida com uma perspectiva mais ampla.",
    };

    /// <summary>Meditações — textos originais + São Francisco (domínio público). (Nome, Texto).</summary>
    public static readonly IReadOnlyList<(string Name, string Text)> Meditacoes = new[]
    {
        ("Calma",
         "Hoje eu respiro e devolvo ao mundo o que não me cabe carregar. Cuido da minha parte e solto o resto. Onde não posso agir, eu aceito; onde posso, eu ajo."),
        ("Um dia de cada vez",
         "Hoje eu não preciso resolver a vida inteira. Basta cuidar deste dia. Quando ele terminar, terá sido suficiente."),
        ("Gratidão",
         "Antes de dormir, eu reconheço três coisas boas de hoje, por menores que sejam. A gratidão treina os olhos para enxergar o que dá certo."),
        ("Oração de São Francisco",
         "Senhor, fazei-me instrumento de Vossa paz.\nOnde houver ódio, que eu leve o amor;\nonde houver ofensa, que eu leve o perdão;\nonde houver discórdia, que eu leve a união;\nonde houver dúvida, que eu leve a fé;\nonde houver erro, que eu leve a verdade;\nonde houver desespero, que eu leve a esperança;\nonde houver tristeza, que eu leve a alegria;\nonde houver trevas, que eu leve a luz.\nÓ Mestre, fazei que eu procure mais consolar que ser consolado,\ncompreender que ser compreendido,\namar que ser amado.\nPois é dando que se recebe,\né perdoando que se é perdoado,\ne é morrendo que se vive para a vida eterna."),
    };

    /// <summary>Check HALT — (Letra, Palavra, Pergunta, Dica, Emoji).</summary>
    public static readonly IReadOnlyList<(string Letter, string Word, string Question, string Tip, string Emoji)> HaltCheck = new[]
    {
        ("H", "Fome", "Você está com fome?",
         "Coma algo nutritivo agora. A fome baixa a guarda e aumenta a irritabilidade e a vulnerabilidade.",
         "🍎"),
        ("A", "Raiva", "Você está com raiva ou angustiado?",
         "Respire fundo. Fale com alguém de confiança. Evite decisões importantes agora — a raiva distorce o julgamento.",
         "😤"),
        ("L", "Solidão", "Você está se sentindo solitário?",
         "Procure um grupo de apoio ou ligue para alguém próximo. Conexão é o oposto da dependência.",
         "😔"),
        ("T", "Cansaço", "Você está cansado ou exausto?",
         "Descanse. Sono e cuidado com o corpo fazem parte da recuperação. Amanhã é um novo dia — só por hoje.",
         "😴"),
    };
}
