using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Avalia o cardápio montado em um conjunto de FoodDropZone (uma refeição por vez)
/// e gera o texto de feedback + a nota (Adequação nutricional, Adequação ao público, Variedade).
///
/// Ordem de verificação por alimento: refeição -> função -> (agregados do cardápio:) nutrientes -> características (cor/sabor/textura/método) -> grupo.
/// </summary>
public static class MenuFeedbackEvaluator
{
    private const float DescontoInadequado = 3f;
    private const float DescontoAtencao = 0.5f;

    public static (string FeedbackText, MenuScoreResult Score) Evaluate(
        IReadOnlyList<FoodDropZone> dropZones, RefeicaoPrato refeicao, int faseIndex)
    {
        MenuScoreResult score = new MenuScoreResult();
        StringBuilder sb = new StringBuilder();
        bool anyIssueAtAll = false;

        List<FoodDropZone> filled = dropZones.Where(z => z != null && z.AlimentoAtual != null).ToList();

        sb.AppendLine($"<b>{FlagsMapping.GetRefeicaoNome(refeicao)}</b>");

        // Caso especial: o mesmo alimento em TODOS os campos do cardápio.
        if (filled.Count > 1 && filled.All(z => z.AlimentoAtual.Nome == filled[0].AlimentoAtual.Nome))
        {
            string nomeUnico = GetNome(filled[0].AlimentoAtual);
            sb.AppendLine();
            sb.AppendLine($"<b>Opção inadequada:</b> Você selecionou {nomeUnico} em todo o cardápio. Um cardápio adequado deve apresentar variedade de cores, sabores, texturas e métodos de preparo. Procure combinar diferentes alimentos para tornar a refeição mais variada.");
            score.ZerarTudo();
            return (sb.ToString().TrimEnd(), score);
        }

        // Agrupa por Categoria na ordem em que aparecem entre os drops preenchidos.
        List<CategoriaPrato> categorias = new List<CategoriaPrato>();
        foreach (FoodDropZone z in filled)
            if (!categorias.Contains(z.Categoria))
                categorias.Add(z.Categoria);

        bool anyFuncaoOuRefeicaoIssue = false;

        foreach (CategoriaPrato categoria in categorias)
        {
            sb.AppendLine();
            sb.AppendLine($"<b>{FlagsMapping.GetCategoriaHeading(categoria)}:</b>");

            foreach (FoodDropZone zone in filled.Where(z => z.Categoria == categoria))
            {
                CategorizacaoAlimento alimento = zone.AlimentoAtual;
                string nome = GetNome(alimento);
                bool itemTemProblema = false;

                // Casos especiais de cada fase (Inadequado)
                string faseInadequadoMsg = CheckFaseEspecificaInadequado(faseIndex, alimento, nome, out bool zeraPublico);
                if (faseInadequadoMsg != null)
                {
                    itemTemProblema = true;
                    anyIssueAtAll = true;
                    sb.AppendLine(faseInadequadoMsg);
                    if (zeraPublico)
                        score.ZerarPublico();
                    else
                        score.DescontarPublico(DescontoInadequado);
                }

                // Casos especiais de cada fase (Atenção)
                foreach ((string texto, bool afetaNutricional, bool afetaPublico) in CheckFaseEspecificaAtencao(faseIndex, alimento, nome))
                {
                    itemTemProblema = true;
                    anyIssueAtAll = true;
                    sb.AppendLine(texto);
                    if (afetaNutricional)
                        score.DescontarNutricional(DescontoAtencao);
                    if (afetaPublico)
                        score.DescontarPublico(DescontoAtencao);
                }

                // 1) Refeição
                bool refeicaoOk = FlagsMapping.RefeicaoMatches(alimento.Refeicao, refeicao);
                if (!refeicaoOk)
                {
                    itemTemProblema = true;
                    anyIssueAtAll = true;
                    anyFuncaoOuRefeicaoIssue = true;
                    sb.AppendLine($"<b>Opção inadequada:</b> {nome} não é adequado para o {FlagsMapping.GetRefeicaoNome(refeicao)}. Escolha uma opção que seja apropriada para esta refeição.");
                    score.DescontarPublico(DescontoInadequado);
                }
                else
                {
                    // 2) Função
                    bool funcaoOk = FlagsMapping.FuncaoMatches(alimento.Funcao, categoria);
                    if (!funcaoOk)
                    {
                        itemTemProblema = true;
                        anyIssueAtAll = true;
                        anyFuncaoOuRefeicaoIssue = true;
                        string funcaoCorreta = EnumDisplayHelper.GetFlagsDisplayName(alimento.Funcao);
                        string funcaoSelecionada = FlagsMapping.GetCategoriaHeading(categoria);
                        sb.AppendLine($"<b>Atenção:</b> {nome} pode fazer parte do {FlagsMapping.GetRefeicaoNome(refeicao)}, porém sua função é {funcaoCorreta}, e não {funcaoSelecionada}. Considere escolher um alimento que desempenhe a função de {funcaoSelecionada}.");
                        score.DescontarPublico(DescontoInadequado);
                        score.DescontarNutricional(DescontoInadequado);
                    }
                    else if (!itemTemProblema)
                    {
                        sb.AppendLine($"<b>Boa escolha:</b> {nome} é adequado para {FlagsMapping.GetCategoriaHeading(categoria)} e contribui para a composição da refeição.");
                    }
                }
            }
        }

        if (!anyFuncaoOuRefeicaoIssue)
        {
            sb.AppendLine();
            sb.AppendLine("<b>Boa escolha:</b> Os alimentos estão distribuídos de acordo com suas funções no cardápio, contribuindo para uma composição adequada da refeição.");
        }

        // 3) Nutrientes -> características (cor/sabor/textura/método) -> grupo, no escopo de todo o cardápio.
        sb.AppendLine();
        sb.AppendLine("<b>Todo o cardápio:</b>");

        anyIssueAtAll |= CheckRepeticao(filled, score, sb);
        anyIssueAtAll |= CheckNutrientes(filled, score, sb);
        anyIssueAtAll |= CheckCor(filled, score, sb);
        anyIssueAtAll |= CheckSabor(filled, score, sb);
        anyIssueAtAll |= CheckTextura(filled, score, sb);
        anyIssueAtAll |= CheckMetodo(filled, refeicao, score, sb);
        anyIssueAtAll |= CheckGrupo(filled, refeicao, score, sb);

        if (!anyIssueAtAll)
        {
            sb.AppendLine("<b>Cardápio adequado:</b> As escolhas realizadas são adequadas para os componentes da refeição e apresentam boa variedade. Continue assim!");
            sb.AppendLine();
            sb.AppendLine("<b>Refeição adequada:</b> O cardápio apresenta escolhas adequadas para a refeição e o público, com boa variedade de grupos alimentares, métodos de preparo, cores, sabores, texturas e nutrientes. Excelente trabalho!");
        }

        return (sb.ToString().TrimEnd(), score);
    }

    // ---------- Casos especiais de cada fase ----------

    private static string CheckFaseEspecificaInadequado(int faseIndex, CategorizacaoAlimento alimento, string nome, out bool zeraPublico)
    {
        zeraPublico = false;

        switch (faseIndex)
        {
            case 0: // Fase 1 - café da manhã na creche (4 a 5 anos)
                if ((alimento.Grupo & Grupo.Acucares) != 0)
                {
                    return $"<b>Opção inadequada:</b> A clientela é composta por crianças de 4 a 5 anos. {nome} pertence ao grupo \"Açúcares\", sendo uma opção com alto teor de açúcar e que deve ser evitada ou limitada no café da manhã infantil.";
                }
                break;

            case 1: // Fase 2 - almoço hospitalar, intolerância à lactose
                if ((alimento.Grupo & Grupo.LeiteEDerivados) != 0)
                {
                    zeraPublico = true;
                    return $"<b>Opção inadequada:</b> A clientela possui intolerância à lactose. {nome} contém lactose e/ou derivados do leite, por isso não é adequado para este público.";
                }
                break;

            case 3: // Fase 4 - jantar institucional, 65 anos ou mais
                bool metodoFritura = (alimento.Metodo & Metodo.Frito) != 0;
                bool texturaInadequada = (alimento.Textura & (Textura.Crocante | Textura.Firme)) != 0;
                bool grupoAcucarado = (alimento.Grupo & Grupo.Acucares) != 0;

                if (metodoFritura || texturaInadequada || grupoAcucarado)
                {
                    List<string> clausulas = new List<string>();
                    if (metodoFritura)
                        clausulas.Add("feito pelo método: Frito");
                    if (texturaInadequada)
                        clausulas.Add("com textura crocante e/ou firme");
                    if (grupoAcucarado)
                        clausulas.Add("muito açucarado");

                    zeraPublico = true;
                    return $"<b>Opção inadequada:</b> {nome} é um alimento {string.Join(", ", clausulas)}, por isso é pouco recomendado para a clientela.";
                }
                break;
        }

        return null;
    }

    private static List<(string Texto, bool AfetaNutricional, bool AfetaPublico)> CheckFaseEspecificaAtencao(int faseIndex, CategorizacaoAlimento alimento, string nome)
    {
        List<(string, bool, bool)> resultados = new List<(string, bool, bool)>();

        switch (faseIndex)
        {
            case 0: // Fase 1
                Sabor sabor = alimento.Sabor & (Sabor.Doce | Sabor.LevementeDoce);
                if (sabor != 0)
                {
                    string saborNome = EnumDisplayHelper.GetFlagsDisplayName(sabor);
                    resultados.Add(($"<b>Atenção:</b> {nome} possui sabor {saborNome}, o que pode contribuir para uma refeição com excesso de alimentos de sabor adocicado. Para crianças de 4 a 5 anos, é importante evitar o excesso de açúcar e priorizar uma alimentação variada.", true, true));
                }
                break;

            case 2: // Fase 3
                if (alimento.Nome == FoodName.Refrigerante)
                {
                    resultados.Add(($"<b>Atenção:</b> {nome} é aceitável, mas pouco recomendado.", true, false));
                }
                break;
        }

        return resultados;
    }

    // ---------- Checagens de todo o cardápio ----------

    private static bool CheckRepeticao(List<FoodDropZone> filled, MenuScoreResult score, StringBuilder sb)
    {
        bool any = false;

        var grupos = filled.GroupBy(z => (z.Categoria, z.AlimentoAtual.Nome));
        foreach (var grupo in grupos)
        {
            if (grupo.Count() < 2)
                continue;

            any = true;
            string nome = GetNome(grupo.First().AlimentoAtual);
            sb.AppendLine($"<b>Atenção:</b> {nome} foi selecionado mais de uma vez no cardápio. Procure variar os alimentos para tornar a refeição mais diversificada.");
            score.DescontarNutricional(DescontoAtencao);
            score.DescontarPublico(DescontoAtencao);
            score.DescontarVariedade(DescontoAtencao);
        }

        return any;
    }

    private static bool CheckNutrientes(List<FoodDropZone> filled, MenuScoreResult score, StringBuilder sb)
    {
        var grupos = filled
            .Where(z => z.AlimentoAtual.Nutrientes != Nutriente.None)
            .GroupBy(z => z.AlimentoAtual.Nutrientes)
            .Where(g => g.Count() >= 2)
            .ToList();

        if (grupos.Count == 0)
        {
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta uma boa diversidade de nutrientes, contribuindo para uma composição nutricional equilibrada.");
            return false;
        }

        foreach (var grupo in grupos)
        {
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            sb.AppendLine($"<b>Atenção:</b> {EnumDisplayHelper.JoinNames(nomes)} possuem nutrientes semelhantes. Procure variar as escolhas para proporcionar uma maior diversidade de nutrientes na refeição.");
            score.DescontarNutricional(DescontoAtencao);
            score.DescontarVariedade(DescontoAtencao);
        }

        return true;
    }

    private static bool CheckCor(List<FoodDropZone> filled, MenuScoreResult score, StringBuilder sb)
    {
        var grupos = filled
            .Where(z => z.AlimentoAtual.Cor != Cor.None)
            .GroupBy(z => z.AlimentoAtual.Cor)
            .Where(g => g.Count() >= 2)
            .ToList();

        if (grupos.Count == 0)
        {
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta uma boa variedade de cores, tornando o cardápio mais diversificado e visualmente atrativo.");
            return false;
        }

        foreach (var grupo in grupos)
        {
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            sb.AppendLine($"<b>Atenção:</b> {EnumDisplayHelper.JoinNames(nomes)} apresentam cores iguais ou semelhantes. Experimente combinar alimentos de cores diferentes para tornar o cardápio mais variado visualmente.");
            score.DescontarVariedade(DescontoAtencao);
        }

        return true;
    }

    private static bool CheckSabor(List<FoodDropZone> filled, MenuScoreResult score, StringBuilder sb)
    {
        var grupos = filled
            .Where(z => z.AlimentoAtual.Sabor != Sabor.None)
            .GroupBy(z => z.AlimentoAtual.Sabor)
            .Where(g => g.Count() >= 2)
            .ToList();

        if (grupos.Count == 0)
        {
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta variedade de sabores, proporcionando uma composição mais diversificada.");
            return false;
        }

        foreach (var grupo in grupos)
        {
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            string saborNome = EnumDisplayHelper.GetFlagsDisplayName(grupo.Key);
            sb.AppendLine($"<b>Atenção:</b> {EnumDisplayHelper.JoinNames(nomes)} apresentam predominância de sabor {saborNome}. Considere incluir uma preparação com sabor diferente para aumentar a variedade da refeição.");
            score.DescontarVariedade(DescontoAtencao);
        }

        return true;
    }

    private static bool CheckTextura(List<FoodDropZone> filled, MenuScoreResult score, StringBuilder sb)
    {
        var grupos = filled
            .Where(z => z.AlimentoAtual.Textura != Textura.None)
            .GroupBy(z => z.AlimentoAtual.Textura)
            .Where(g => g.Count() >= 2)
            .ToList();

        if (grupos.Count == 0)
        {
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta variedade de texturas, contribuindo para uma experiência alimentar mais diversificada.");
            return false;
        }

        foreach (var grupo in grupos)
        {
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            string texturaNome = EnumDisplayHelper.GetFlagsDisplayName(grupo.Key);
            sb.AppendLine($"<b>Atenção:</b> {EnumDisplayHelper.JoinNames(nomes)} apresentam textura {texturaNome}. Incluir uma preparação com textura diferente pode contribuir para uma maior variedade de texturas no cardápio.");
            score.DescontarVariedade(DescontoAtencao);
        }

        return true;
    }

    private static bool CheckMetodo(List<FoodDropZone> filled, RefeicaoPrato refeicao, MenuScoreResult score, StringBuilder sb)
    {
        bool any = false;
        HashSet<FoodDropZone> cobertosPorPredominancia = new HashSet<FoodDropZone>();

        // 3 ou mais no cardápio inteiro com o mesmo método.
        var gruposGerais = filled
            .Where(z => z.AlimentoAtual.Metodo != Metodo.None)
            .GroupBy(z => z.AlimentoAtual.Metodo)
            .Where(g => g.Count() >= 3)
            .ToList();

        foreach (var grupo in gruposGerais)
        {
            any = true;
            foreach (FoodDropZone z in grupo)
                cobertosPorPredominancia.Add(z);

            string metodoNome = EnumDisplayHelper.GetFlagsDisplayName(grupo.Key);
            sb.AppendLine($"<b>Atenção:</b> O cardápio apresenta predominância do método de preparo {metodoNome}. Variar os métodos de preparo pode tornar a refeição mais diversificada.");
            score.DescontarVariedade(DescontoAtencao);
        }

        // 2 no mesmo função (categoria) e mesma refeição com o mesmo método.
        var gruposPorCategoria = filled
            .Where(z => z.AlimentoAtual.Metodo != Metodo.None && !cobertosPorPredominancia.Contains(z))
            .GroupBy(z => (z.Categoria, z.AlimentoAtual.Metodo))
            .Where(g => g.Count() >= 2)
            .ToList();

        foreach (var grupo in gruposPorCategoria)
        {
            any = true;
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            string categoriaNome = FlagsMapping.GetCategoriaHeading(grupo.Key.Categoria);
            sb.AppendLine($"<b>Atenção:</b> {EnumDisplayHelper.JoinNames(nomes)}, presentes em {categoriaNome}, utilizam o mesmo método de preparo. Variar as formas de preparo pode tornar o cardápio mais diversificado.");
            score.DescontarVariedade(DescontoAtencao);
        }

        if (!any)
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta variedade nos métodos de preparo, contribuindo para um cardápio mais diversificado.");

        return any;
    }

    private static bool CheckGrupo(List<FoodDropZone> filled, RefeicaoPrato refeicao, MenuScoreResult score, StringBuilder sb)
    {
        bool any = false;
        HashSet<FoodDropZone> cobertosPorPredominancia = new HashSet<FoodDropZone>();

        // 3 ou mais no cardápio inteiro do mesmo grupo alimentar.
        var gruposGerais = filled
            .Where(z => z.AlimentoAtual.Grupo != Grupo.None)
            .GroupBy(z => z.AlimentoAtual.Grupo)
            .Where(g => g.Count() >= 3)
            .ToList();

        foreach (var grupo in gruposGerais)
        {
            any = true;
            foreach (FoodDropZone z in grupo)
                cobertosPorPredominancia.Add(z);

            string grupoNome = EnumDisplayHelper.GetFlagsDisplayName(grupo.Key);
            sb.AppendLine($"<b>Atenção:</b> O cardápio apresenta várias opções do grupo {grupoNome}. Considere incluir alimentos de outros grupos para aumentar a variedade da refeição.");
            score.DescontarVariedade(DescontoAtencao);
        }

        // 2 na mesma função (categoria) e mesma refeição do mesmo grupo.
        var gruposPorCategoria = filled
            .Where(z => z.AlimentoAtual.Grupo != Grupo.None && !cobertosPorPredominancia.Contains(z))
            .GroupBy(z => (z.Categoria, z.AlimentoAtual.Grupo))
            .Where(g => g.Count() >= 2)
            .ToList();

        foreach (var grupo in gruposPorCategoria)
        {
            any = true;
            List<string> nomes = grupo.Select(z => GetNome(z.AlimentoAtual)).Distinct().ToList();
            sb.AppendLine($"<b>Atenção:</b> A(s) opção(ões) {EnumDisplayHelper.JoinNames(nomes)}, da refeição {FlagsMapping.GetRefeicaoNome(refeicao)} pertencem ao mesmo grupo alimentar. Embora os métodos de preparo sejam diferentes, considere utilizar outra fonte proteica para aumentar a variedade do cardápio.");
            score.DescontarVariedade(DescontoAtencao);
            score.DescontarNutricional(DescontoAtencao);
        }

        if (!any)
            sb.AppendLine("<b>Boa escolha:</b> A refeição apresenta uma boa variedade de grupos alimentares, contribuindo para uma composição mais diversificada.");

        return any;
    }

    // ---------- Textos de nota ----------

    public static string BuildCriteriaScoreText(MenuScoreResult score)
    {
        return
            $"Adequação nutricional: {Mathf.RoundToInt(score.Nutricional):00}\n" +
            $"Adequação ao público: {Mathf.RoundToInt(score.Publico):00}\n" +
            $"Variedade: {Mathf.RoundToInt(score.Variedade):00}";
    }

    public static string BuildNotaTotalText(MenuScoreResult score) => BuildNotaTotalText(score.NotaFinal);

    public static string BuildNotaTotalText(float notaFinal)
    {
        return $"Nota total: {Mathf.RoundToInt(notaFinal):00}";
    }

    public static string BuildFase5ScoreText(float cafeDaManhaFinal, float almocoFinal, float jantarFinal)
    {
        return
            $"Café da manhã: {Mathf.RoundToInt(cafeDaManhaFinal):00}\n" +
            $"Almoço: {Mathf.RoundToInt(almocoFinal):00}\n" +
            $"Jantar: {Mathf.RoundToInt(jantarFinal):00}";
    }

    private static string GetNome(CategorizacaoAlimento alimento) => EnumDisplayHelper.GetDisplayName(alimento.Nome);
}
