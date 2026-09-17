using UnityEngine;

public class CategorizacaoAlimento : MonoBehaviour
{
    [Header("Nome do alimento")]
    public FoodName Nome;

    [Header("Função")]
    public Funcao Funcao;

    [Header("Refeição")]
    public Refeicao Refeicao;

    [Header("Nutrientes")]
    public Nutriente Nutrientes;

    [Header("Características")]
    public Estado Estado;
    public Metodo Metodo;
    public Textura Textura;
    public Sabor Sabor;
    public Cor Cor;

    [Header("Grupo")]
    public Grupo Grupo;
}
