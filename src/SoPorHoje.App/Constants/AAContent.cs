namespace SoPorHoje.App.Constants;

/// <summary>
/// Conteúdo estático do programa de AA: passos, tradições, promessas,
/// meditações "Só Por Hoje", orações e check HALT.
/// </summary>
public static class AAContent
{
    /// <summary>12 Passos — (Title, Text).</summary>
    public static readonly IReadOnlyList<(string Title, string Text)> TwelveSteps = new[]
    {
        ("Impotência",
         "Admitimos que éramos impotentes perante o álcool — que tínhamos perdido o domínio sobre nossas vidas."),
        ("Esperança",
         "Viemos a acreditar que um Poder Superior a nós mesmos poderia devolver-nos a sanidade."),
        ("Decisão",
         "Tomamos a decisão de entregar nossa vontade e nossa vida aos cuidados de Deus, na forma em que O concebíamos."),
        ("Inventário Moral",
         "Fizemos um minucioso e destemido inventário moral de nós mesmos."),
        ("Admissão",
         "Admitimos perante Deus, perante nós mesmos e perante outro ser humano, a natureza exata de nossas falhas."),
        ("Disponibilidade",
         "Prontificamo-nos inteiramente a permitir que Deus removesse todos esses defeitos de caráter."),
        ("Humildade",
         "Humildemente rogamos a Ele que nos livrasse de nossas imperfeições."),
        ("Lista de Prejuízos",
         "Fizemos uma relação de todas as pessoas que tínhamos prejudicado e nos dispusemos a reparar os danos a elas causados."),
        ("Reparação de Danos",
         "Fizemos reparações diretas dos danos causados a tais pessoas, sempre que possível, exceto quando fazê-las significasse prejudicá-las ou a outrem."),
        ("Inventário Contínuo",
         "Continuamos fazendo o inventário pessoal e, quando estávamos errados, nós o admitíamos prontamente."),
        ("Meditação e Oração",
         "Procuramos, através da prece e da meditação, melhorar nosso contato consciente com Deus, na forma em que O concebíamos, rogando apenas o conhecimento de Sua vontade em relação a nós e o poder de realizar essa vontade."),
        ("Despertar Espiritual",
         "Tendo experimentado um despertar espiritual, graças a estes passos, procuramos transmitir esta mensagem aos alcoólicos e praticar estes princípios em todas as nossas atividades."),
    };

    /// <summary>12 Tradições — textos completos.</summary>
    public static readonly IReadOnlyList<string> TwelveTraditions = new[]
    {
        "Nosso bem-estar comum deve ter precedência; a recuperação pessoal depende da unidade de A.A.",
        "Para os propósitos de nosso grupo existe apenas uma autoridade fundamental — um Deus amoroso, tal como Ele possa se expressar na consciência de nosso grupo. Nossos líderes são apenas servidores de confiança; eles não governam.",
        "O único requisito para ser membro de A.A. é o desejo de parar de beber.",
        "Cada grupo deve ser autônomo, exceto em assuntos que afetem outros grupos ou A.A. como um todo.",
        "Cada grupo tem apenas um objetivo primordial — transmitir sua mensagem ao alcoólico que ainda sofre.",
        "Um grupo de A.A. nunca deve endossar, financiar ou emprestar o nome de A.A. a qualquer empreendimento ou estabelecimento a ele relacionado, para que problemas de dinheiro, propriedade e prestígio não nos desviem de nosso objetivo primordial.",
        "Todo grupo de A.A. deve ser completamente autossuficiente, recusando contribuições externas.",
        "Alcoólicos Anônimos deve sempre manter-se não profissional, mas nossos centros de serviço podem empregar pessoal especializado.",
        "A.A., como tal, não deve jamais ser organizado; podemos, contudo, criar juntas ou comitês de serviço que sejam diretamente responsáveis perante aqueles a quem servem.",
        "A.A. não tem opinião sobre questões alheias; portanto o nome de A.A. nunca deve ser envolvido em controvérsias públicas.",
        "Nossas relações com o público devem basear-se na atração em vez da promoção; precisamos sempre manter anonimato pessoal na imprensa, no rádio e no cinema.",
        "O anonimato é o alicerce espiritual de todas as nossas tradições, lembrando-nos sempre de colocar os princípios acima das personalidades.",
    };

    /// <summary>11 Promessas do A.A. (capítulo "Aos Agnósticos").</summary>
    public static readonly IReadOnlyList<string> Promises = new[]
    {
        "Se trabalharmos este programa, conheceremos uma nova liberdade e uma nova felicidade.",
        "Não nos arrependeremos do passado nem desejaremos fechar a porta às suas recordações.",
        "Compreenderemos a palavra serenidade e conheceremos a paz.",
        "Por mais aviltante que tenha sido nossa experiência, veremos como ela pode beneficiar outros.",
        "Esse sentimento de inutilidade e lamentação própria desaparecerá.",
        "Perderemos o interesse em coisas egoístas e ganharemos interesse nas nossas semelhantes.",
        "A autocomiseração desaparecerá.",
        "Perderemos o medo das pessoas e da insegurança econômica.",
        "Saberemos lidar intuitivamente com situações que antes nos confundiam.",
        "De repente, perceberemos que Deus está fazendo por nós o que não poderíamos fazer por nós mesmos.",
        "Essas são promessas extravagantes? Acreditamos que não. Elas estão se materializando entre nós — às vezes rapidamente, às vezes devagar. Sempre se materializarão se trabalharmos para isso.",
    };

    /// <summary>9 meditações "Só Por Hoje".</summary>
    public static readonly IReadOnlyList<string> JustForToday = new[]
    {
        "Só por hoje, tentarei viver somente neste dia, sem querer resolver o problema de toda a minha vida de uma vez.",
        "Só por hoje, serei feliz. Isto pressupõe que é verdade o que Abraham Lincoln disse: 'A maioria das pessoas é tão feliz quanto se resolve a ser.'",
        "Só por hoje, me ajustarei ao que é, e não tentarei ajustar todas as coisas ao meu desejo.",
        "Só por hoje, cuidarei de meu físico. Farei algum exercício, cuidarei de minha alimentação, não abusarei do meu corpo e não o negligenciarei, de modo que ele seja uma perfeita máquina para as minhas necessidades.",
        "Só por hoje, tentarei fortalecer minha mente. Aprenderei algo útil. Não serei um preguiçoso mental. Lerei algo que requeira esforço, reflexão e concentração.",
        "Só por hoje, exercitarei minha alma de três maneiras: farei a alguém algum bem sem que ele o saiba; farei pelo menos duas coisas que não gosto de fazer — só por exercício; e não mostrarei a ninguém que meus sentimentos foram feridos — eles podem estar certos, e é bom assim.",
        "Só por hoje, serei agradável. Terei um aspecto tão bem cuidado quanto possível; falarei com voz baixa; agirei cortesmente; não criticarei ninguém; não encontrarei defeito em coisa alguma e não tentarei regular nem melhorar ninguém.",
        "Só por hoje, terei um programa. Anotarei o que espero fazer a cada hora. Pode ser que não o cumpra exatamente, mas terei um programa — evitarei dois males: a pressa e a indecisão.",
        "Só por hoje, terei um momento de tranquilidade e contemplação. Relaxarei. Neste momento, verei a minha vida com uma perspectiva mais ampla.",
    };

    /// <summary>4 orações de AA — (Name, Text).</summary>
    public static readonly IReadOnlyList<(string Name, string Text)> Prayers = new[]
    {
        ("Oração da Serenidade",
         "Deus, concedei-me a serenidade necessária para aceitar as coisas que não posso modificar,\ncoragem para modificar aquelas que posso\ne sabedoria para distinguir umas das outras."),
        ("Oração do 3º Passo",
         "Deus, entrego-me a Ti — para que construas comigo e faças de mim o que quiseres. Livra-me da escravidão do ego para que possa melhor fazer a Tua vontade. Tira minhas dificuldades para que a vitória sobre elas seja o testemunho para aqueles que Eu possa ajudar, de Teu Poder, Teu Amor e Teu Caminho. Que possa sempre fazer Tua vontade."),
        ("Oração do 7º Passo",
         "Meu Criador, estou agora disposto a que tenhas tudo de mim — tanto o bom quanto o mau. Rogo que retires de mim cada defeito de caráter que se coloque como obstáculo a minha utilidade para Ti e para meus semelhantes. Concede-me força à medida que parta para fazer Teu trabalho."),
        ("Oração de São Francisco",
         "Senhor, fazei-me instrumento de Vossa paz.\nOnde houver ódio, que eu leve o amor;\nonde houver ofensa, que eu leve o perdão;\nonde houver discórdia, que eu leve a união;\nonde houver dúvida, que eu leve a fé;\nonde houver erro, que eu leve a verdade;\nonde houver desespero, que eu leve a esperança;\nonde houver tristeza, que eu leve a alegria;\nonde houver trevas, que eu leve a luz.\nÓ Mestre, fazei que eu procure mais consolar que ser consolado,\ncompreender que ser compreendido,\namar que ser amado.\nPois é dando que se recebe,\né perdoando que se é perdoado,\ne é morrendo que se vive para a vida eterna."),
    };

    /// <summary>Check HALT — (Letter, Word, Question, Tip, Emoji).</summary>
    public static readonly IReadOnlyList<(string Letter, string Word, string Question, string Tip, string Emoji)> HaltCheck = new[]
    {
        ("H", "Fome", "Você está com fome?",
         "Coma algo nutritivo agora. Fome baixa a guarda e pode aumentar a irritabilidade e a vulnerabilidade.",
         "🍎"),
        ("A", "Raiva", "Você está com raiva ou angustiado?",
         "Respire fundo. Ligue para alguém do programa. Não tome decisões importantes agora — a raiva distorce o julgamento.",
         "😤"),
        ("L", "Solidão", "Você está se sentindo solitário?",
         "Vá a uma reunião. Ligue para um companheiro ou padrinho. Conexão é o oposto da dependência.",
         "😔"),
        ("T", "Cansaço", "Você está cansado ou exausto?",
         "Descanse. Sono e recuperação física são parte do programa. Amanhã é um novo dia — só por hoje.",
         "😴"),
    };
}
