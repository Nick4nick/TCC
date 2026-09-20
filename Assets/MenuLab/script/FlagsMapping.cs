/// <summary>
/// Converte entre os enums "de slot" (CategoriaPrato/RefeicaoPrato, usados no FoodDropZone)
/// e os enums [Flags] "do alimento" (Funcao/Refeicao, usados no CategorizacaoAlimento),
/// e verifica se um alimento é compatível com um slot.
/// </summary>
public static class FlagsMapping
{
    public static Funcao ToFuncao(CategoriaPrato categoria)
    {
        switch (categoria)
        {
            case CategoriaPrato.Acompanhamento: return Funcao.Acompanhamento;
            case CategoriaPrato.Bebida: return Funcao.Bebida;
            case CategoriaPrato.Complemento: return Funcao.Complemento;
            case CategoriaPrato.Guarnicao: return Funcao.Guarnicao;
            case CategoriaPrato.Principal: return Funcao.Principal;
            case CategoriaPrato.Salada: return Funcao.Salada;
            case CategoriaPrato.Sobremesa: return Funcao.Sobremesa;
            default: return Funcao.None;
        }
    }

    public static Refeicao ToRefeicao(RefeicaoPrato refeicao)
    {
        switch (refeicao)
        {
            case RefeicaoPrato.Almoco: return Refeicao.Almoco;
            case RefeicaoPrato.CafeDaManha: return Refeicao.CafeDaManha;
            case RefeicaoPrato.Jantar: return Refeicao.Jantar;
            default: return Refeicao.None;
        }
    }

    /// <summary>A função do alimento (drag) deve bater com a categoria do drop.</summary>
    public static bool FuncaoMatches(Funcao alimentoFuncao, CategoriaPrato categoriaDoDrop)
        => (alimentoFuncao & ToFuncao(categoriaDoDrop)) != 0;

    public static bool RefeicaoMatches(Refeicao alimentoRefeicao, RefeicaoPrato refeicaoDoDrop)
        => (alimentoRefeicao & ToRefeicao(refeicaoDoDrop)) != 0;

    public static string GetRefeicaoNome(RefeicaoPrato refeicao)
    {
        switch (refeicao)
        {
            case RefeicaoPrato.CafeDaManha: return "Café da manhã";
            case RefeicaoPrato.Almoco: return "Almoço";
            case RefeicaoPrato.Jantar: return "Jantar";
            default: return refeicao.ToString();
        }
    }

    public static string GetCategoriaHeading(CategoriaPrato categoria)
    {
        switch (categoria)
        {
            case CategoriaPrato.Acompanhamento: return "Acompanhamento";
            case CategoriaPrato.Bebida: return "Bebida";
            case CategoriaPrato.Complemento: return "Complemento";
            case CategoriaPrato.Guarnicao: return "Guarnição";
            case CategoriaPrato.Principal: return "Principal";
            case CategoriaPrato.Salada: return "Salada";
            case CategoriaPrato.Sobremesa: return "Sobremesa";
            default: return categoria.ToString();
        }
    }
}
