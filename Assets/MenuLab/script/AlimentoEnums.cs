using System;
using UnityEngine;

public enum FoodName
{
    [InspectorName("Abóbora cozida")] AboboraCozida,
    [InspectorName("Abobrinha")] Abobrinha,
    [InspectorName("Abobrinha refogada")] AbobrinhaRefogada,
    [InspectorName("Acelga")] Acelga,
    [InspectorName("Água")] Agua,
    [InspectorName("Aipim cozido")] AipimCozido,
    [InspectorName("Alface")] Alface,
    [InspectorName("Arroz branco")] ArrozBranco,
    [InspectorName("Arroz com legumes")] ArrozComLegumes,
    [InspectorName("Arroz integral")] ArrozIntegral,
    [InspectorName("Banana")] Banana,
    [InspectorName("Batata frita")] BatataFrita,
    [InspectorName("Batata palha")] BatataPalha,
    [InspectorName("Beterraba")] Beterraba,
    [InspectorName("Bife acebolado")] BifeAcebolado,
    [InspectorName("Biscoito recheado")] BiscoitoRecheado,
    [InspectorName("Bolo simples")] BoloSimples,
    [InspectorName("Café com leite")] CafeComLeite,
    [InspectorName("Carne assada")] CarneAssada,
    [InspectorName("Carne cozida")] CarneCozida,
    [InspectorName("Carne grelhada")] CarneGrelhada,
    [InspectorName("Cenoura")] Cenoura,
    [InspectorName("Cenoura ralada")] CenouraRalada,
    [InspectorName("Cenoura refogada")] CenouraRefogada,
    [InspectorName("Chá")] Cha,
    [InspectorName("Churros recheado com chocolate")] ChurrosRecheadoComChocolate,
    [InspectorName("Cuscuz")] Cuscuz,
    [InspectorName("Estrogonofe")] Estrogonofe,
    [InspectorName("Feijão carioca")] FeijaoCarioca,
    [InspectorName("Feijão preto")] FeijaoPreto,
    [InspectorName("Frango ao molho branco")] FrangoAoMolhoBranco,
    [InspectorName("Frango assado")] FrangoAssado,
    [InspectorName("Frango ensopado")] FrangoEnsopado,
    [InspectorName("Frango frito")] FrangoFrito,
    [InspectorName("Frango grelhado")] FrangoGrelhado,
    [InspectorName("Gelatina")] Gelatina,
    [InspectorName("Geleia")] Geleia,
    [InspectorName("Iogurte")] Iogurte,
    [InspectorName("Laranja")] Laranja,
    [InspectorName("Lasanha")] Lasanha,
    [InspectorName("Legumes ao molho de queijo")] LegumesAoMolhoDeQueijo,
    [InspectorName("Legumes cozidos")] LegumesCozidos,
    [InspectorName("Legumes refogados")] LegumesRefogados,
    [InspectorName("Leite")] Leite,
    [InspectorName("Lentilha")] Lentilha,
    [InspectorName("Maçã")] Maca,
    [InspectorName("Macarrão")] Macarrao,
    [InspectorName("Mamão")] Mamao,
    [InspectorName("Manteiga")] Manteiga,
    [InspectorName("Margarina")] Margarina,
    [InspectorName("Massa ao alho e óleo")] MassaAoAlhoEOleo,
    [InspectorName("Mel")] Mel,
    [InspectorName("Minestra")] Minestra,
    [InspectorName("Omelete de legumes")] OmeleteDeLegumes,
    [InspectorName("Pão de queijo")] PaoDeQueijo,
    [InspectorName("Pão francês")] PaoFrances,
    [InspectorName("Pão integral")] PaoIntegral,
    [InspectorName("Peixe assado")] PeixeAssado,
    [InspectorName("Peixe frito")] PeixeFrito,
    [InspectorName("Pepino")] Pepino,
    [InspectorName("Polenta")] Polenta,
    [InspectorName("Pudim")] Pudim,
    [InspectorName("Purê de batata")] PureDeBatata,
    [InspectorName("Purê de batata com leite")] PureDeBatataComLeite,
    [InspectorName("Queijo")] Queijo,
    [InspectorName("Refrigerante")] Refrigerante,
    [InspectorName("Repolho")] Repolho,
    [InspectorName("Requeijão")] Requeijao,
    [InspectorName("Rúcula")] Rucula,
    [InspectorName("Sagu de vinho")] SaguDeVinho,
    [InspectorName("Salada de batata")] SaladaDeBatata,
    [InspectorName("Salada de frutas")] SaladaDeFrutas,
    [InspectorName("Sopa de legumes")] SopaDeLegumes,
    [InspectorName("Sorvete")] Sorvete,
    [InspectorName("Suco de abacaxi")] SucoDeAbacaxi,
    [InspectorName("Suco de laranja")] SucoDeLaranja,
    [InspectorName("Suco de maracujá")] SucoDeMaracuja,
    [InspectorName("Tapioca")] Tapioca,
    [InspectorName("Tomate")] Tomate,
    [InspectorName("Vitamina de frutas")] VitaminaDeFrutas,
}

[Flags]
public enum Funcao
{
    None = 0,
    Acompanhamento = 1 << 0,
    Bebida = 1 << 1,
    Complemento = 1 << 2,
    Guarnicao = 1 << 3,
    Principal = 1 << 4,
    Salada = 1 << 5,
    Sobremesa = 1 << 6,
}

[Flags]
public enum Refeicao
{
    None = 0,
    Almoco = 1 << 0,
    [InspectorName("Café da manhã")] CafeDaManha = 1 << 1,
    Jantar = 1 << 2,
}

[Flags]
public enum Nutriente
{
    None = 0,
    [InspectorName("Água")] Agua = 1 << 0,
    [InspectorName("Cálcio")] Calcio = 1 << 1,
    Carboidratos = 1 << 2,
    Energia = 1 << 3,
    Ferro = 1 << 4,
    Fibras = 1 << 5,
    Gordura = 1 << 6,
    Hidratacao = 1 << 7,
    Minerais = 1 << 8,
    [InspectorName("Proteína")] Proteina = 1 << 9,
    [InspectorName("Vitaminas e minerais")] VitaminasEMinerais = 1 << 10,
}

[Flags]
public enum Estado
{
    None = 0,
    Cremoso = 1 << 0,
    [InspectorName("Com molho")] ComMolho = 1 << 1,
    [InspectorName("Líquido")] Liquido = 1 << 2,
    Semissolido = 1 << 3,
    [InspectorName("Sólido")] Solido = 1 << 4,
    Viscoso = 1 << 5,
}

[Flags]
public enum Metodo
{
    None = 0,
    Assado = 1 << 0,
    Batido = 1 << 1,
    Chapeado = 1 << 2,
    Congelado = 1 << 3,
    Cozido = 1 << 4,
    Cru = 1 << 5,
    Ensopado = 1 << 6,
    [InspectorName("Extraído")] Extraido = 1 << 7,
    Fermentado = 1 << 8,
    Frito = 1 << 9,
    Grelhado = 1 << 10,
    Industrializado = 1 << 11,
    [InspectorName("Infusão")] Infusao = 1 << 12,
    Natural = 1 << 13,
    Pronto = 1 << 14,
    Preparado = 1 << 15,
    [InspectorName("Preparado na chapa")] PreparadoNaChapa = 1 << 16,
    Refogado = 1 << 17,
    Vapor = 1 << 18,
}

[Flags]
public enum Textura
{
    None = 0,
    Borrachuda = 1 << 0,
    Crocante = 1 << 1,
    Cremosa = 1 << 2,
    Firme = 1 << 3,
    Gaseificada = 1 << 4,
    Gelatinosa = 1 << 5,
    Granulada = 1 << 6,
    [InspectorName("Líquida")] Liquida = 1 << 7,
    Macia = 1 << 8,
    Suculenta = 1 << 9,
    Viscosa = 1 << 10,
}

[Flags]
public enum Sabor
{
    None = 0,
    [InspectorName("Ácido")] Acido = 1 << 0,
    [InspectorName("Cítrico")] Citrico = 1 << 1,
    Doce = 1 << 2,
    [InspectorName("Levemente doce")] LevementeDoce = 1 << 3,
    Marcante = 1 << 4,
    Neutro = 1 << 5,
    Retrogosto = 1 << 6,
    Salgado = 1 << 7,
    Suave = 1 << 8,
}

[Flags]
public enum Cor
{
    None = 0,
    Amarelo = 1 << 0,
    [InspectorName("Amarelo claro")] AmareloClaro = 1 << 1,
    Bege = 1 << 2,
    Branco = 1 << 3,
    Clara = 1 << 4,
    Dourado = 1 << 5,
    Laranja = 1 << 6,
    Marrom = 1 << 7,
    [InspectorName("Marrom claro")] MarromClaro = 1 << 8,
    Preto = 1 << 9,
    Roxo = 1 << 10,
    [InspectorName("Variável")] Variavel = 1 << 11,
    Verde = 1 << 12,
    [InspectorName("Verde claro")] VerdeClaro = 1 << 13,
    Vermelho = 1 << 14,
    Transparente = 1 << 15,
}

[Flags]
public enum Grupo
{
    None = 0,
    [InspectorName("Açúcares")] Acucares = 1 << 0,
    Aves = 1 << 1,
    Bebidas = 1 << 2,
    Carnes = 1 << 3,
    Cereais = 1 << 4,
    Doces = 1 << 5,
    [InspectorName("Farináceos")] Farinaceos = 1 << 6,
    Frutas = 1 << 7,
    Gorduras = 1 << 8,
    [InspectorName("Hortaliças")] Hortalicas = 1 << 9,
    Industrializadas = 1 << 10,
    Leguminosas = 1 << 11,
    [InspectorName("Leite e derivados")] LeiteEDerivados = 1 << 12,
    Molho = 1 << 13,
    Ovos = 1 << 14,
    Pescados = 1 << 15,
    Sobremesas = 1 << 16,
    [InspectorName("Tubérculos")] Tuberculos = 1 << 17,
}
