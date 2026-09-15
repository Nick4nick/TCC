using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    [Header("Painel")]
    [SerializeField] private GameObject painel;

    [Header("Campos de texto")]
    [SerializeField] private TMP_Text mensage;
    [SerializeField] private TMP_Text btnEsquerdo;
    [SerializeField] private TMP_Text btnDireito;

    [Header("Botões")]
    [SerializeField] private Button botaoEsquerda;
    [SerializeField] private Button botaoDireita;

    private void Awake()
    {
        Instance = this;
        painel.SetActive(false);
    }

    public void Abrir(
        string texto1, string texto2, string texto3,
        Action<string, string, string> acaoBotaoEsquerda,
        Action<string, string, string> acaoBotaoDireita)
    {
        mensage.text = texto1;
        btnEsquerdo.text = texto2;
        btnDireito.text = texto3;

        botaoEsquerda.onClick.RemoveAllListeners();
        botaoDireita.onClick.RemoveAllListeners();

        botaoEsquerda.onClick.AddListener(() =>
            acaoBotaoEsquerda?.Invoke(mensage.text, btnEsquerdo.text, btnDireito.text));

        botaoDireita.onClick.AddListener(() =>
            acaoBotaoDireita?.Invoke(mensage.text, btnEsquerdo.text, btnDireito.text));

        painel.SetActive(true);
    }

    public void Fechar()
    {
        painel.SetActive(false);
    }
}
