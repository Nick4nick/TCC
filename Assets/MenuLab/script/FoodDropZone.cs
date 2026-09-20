using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodDropZone : MonoBehaviour, IDropHandler
{
    public static event Action ContentChanged;


    [Header("Categoria")]
    [Tooltip("Categoria deste drop, usada apenas como informação/marcação")]
    [SerializeField] private CategoriaPrato _categoria;
    [Tooltip("Refeição deste drop, usada apenas como informação/marcação")]
    [SerializeField] private RefeicaoPrato _refeicao;

    [Header("Referências")]
    [Tooltip("Imagem que vai exibir o sprite do alimento recebido")]
    [SerializeField] private Image _foodImage;
    [Tooltip("Texto que fica inativo até receber um alimento, exibindo então o nome dele")]
    [SerializeField] private TMP_Text _foodNameText;

    public CategoriaPrato Categoria => _categoria;
    public RefeicaoPrato Refeicao => _refeicao;
    public CategorizacaoAlimento AlimentoAtual { get; private set; }

    private void Awake()
    {
        LimparDrop();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        FoodDragHandler drag = eventData.pointerDrag.GetComponent<FoodDragHandler>();
        if (drag == null)
            return;

        ReceberAlimento(drag);
    }

    private void ReceberAlimento(FoodDragHandler drag)
    {
        AlimentoAtual = drag.Dados;

        if (_foodImage != null)
        {
            _foodImage.sprite = drag.Sprite;
            _foodImage.enabled = drag.Sprite != null;
        }

        if (_foodNameText != null)
        {
            _foodNameText.text = drag.NomeExibido;
            _foodNameText.gameObject.SetActive(true);
        }

        SoundManager.Instance?.PlayDropSfx();

        ContentChanged?.Invoke();
    }

    public void CopyFrom(FoodDropZone other)
    {
        if (other == null)
            return;

        AlimentoAtual = other.AlimentoAtual;

        if (_foodImage != null && other._foodImage != null)
        {
            _foodImage.sprite = other._foodImage.sprite;
            _foodImage.enabled = other._foodImage.enabled;
        }

        if (_foodNameText != null && other._foodNameText != null)
        {
            _foodNameText.text = other._foodNameText.text;
            _foodNameText.gameObject.SetActive(other._foodNameText.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Limpa o alimento deste drop. Usado ao sair de uma fase sem finalizá-la, para que
    /// uma nova tentativa dessa fase não comece com o que foi deixado da vez anterior.
    /// </summary>
    public void Limpar()
    {
        LimparDrop();
    }

    private void LimparDrop()
    {
        AlimentoAtual = null;

        if (_foodImage != null)
        {
            _foodImage.sprite = null;
            _foodImage.enabled = false;
        }

        if (_foodNameText != null)
            _foodNameText.gameObject.SetActive(false);
    }
}
