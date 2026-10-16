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

    /// <summary>Quando bloqueado (ex: cópia exibida na tela de feedback), não aceita drops nem permite arrastar o alimento.</summary>
    public bool Bloqueado { get; private set; }

    private bool _inicializado;

    private void Awake()
    {
        Inicializar();
    }

    /// <summary>
    /// Separado do Awake para poder ser chamado antes dele: em clones ainda inativos
    /// (ex: cardápios da Fase 5 copiados para o feedback), o Awake só roda ao ativar e,
    /// sem essa proteção, limparia o que foi copiado via CopyFrom.
    /// </summary>
    private void Inicializar()
    {
        if (_inicializado)
            return;
        _inicializado = true;

        if (_foodImage != null)
        {
            DropZoneFoodDrag foodDrag = _foodImage.GetComponent<DropZoneFoodDrag>();
            if (foodDrag == null)
                foodDrag = _foodImage.gameObject.AddComponent<DropZoneFoodDrag>();
            foodDrag.Init(this, _foodImage);
        }

        LimparDrop();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Bloqueado || eventData.pointerDrag == null)
            return;

        DropZoneFoodDrag zoneDrag = eventData.pointerDrag.GetComponent<DropZoneFoodDrag>();
        if (zoneDrag != null)
        {
            ReceberDeOutroDrop(zoneDrag);
            return;
        }

        FoodDragHandler drag = eventData.pointerDrag.GetComponent<FoodDragHandler>();
        if (drag == null)
            return;

        ReceberAlimento(drag);
    }

    /// <summary>
    /// Recebe o alimento arrastado de outro drop. Se este drop já tiver um alimento,
    /// os dois trocam de lugar; senão o alimento só é movido.
    /// </summary>
    private void ReceberDeOutroDrop(DropZoneFoodDrag zoneDrag)
    {
        FoodDropZone origem = zoneDrag.Zone;
        if (origem == null || origem.AlimentoAtual == null)
            return;

        zoneDrag.DropAceito = true;

        if (origem == this)
            return;

        CategorizacaoAlimento alimento = AlimentoAtual;
        Sprite sprite = _foodImage != null ? _foodImage.sprite : null;
        string nome = _foodNameText != null ? _foodNameText.text : string.Empty;

        SetConteudo(origem.AlimentoAtual,
            origem._foodImage != null ? origem._foodImage.sprite : null,
            origem._foodNameText != null ? origem._foodNameText.text : string.Empty);

        if (alimento != null)
            origem.SetConteudo(alimento, sprite, nome);
        else
            origem.LimparDrop();

        SoundManager.Instance?.PlayDropSfx();

        ContentChanged?.Invoke();
    }

    private void SetConteudo(CategorizacaoAlimento alimento, Sprite sprite, string nome)
    {
        AlimentoAtual = alimento;

        if (_foodImage != null)
        {
            _foodImage.sprite = sprite;
            _foodImage.enabled = sprite != null;
        }

        if (_foodNameText != null)
        {
            _foodNameText.text = nome;
            _foodNameText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Remove o alimento deste drop (ao arrastá-lo para fora de qualquer drop).
    /// </summary>
    public void Remover()
    {
        if (AlimentoAtual == null)
            return;

        LimparDrop();
        ContentChanged?.Invoke();
    }

    private void ReceberAlimento(FoodDragHandler drag)
    {
        SetConteudo(drag.Dados, drag.Sprite, drag.NomeExibido);

        SoundManager.Instance?.PlayDropSfx();

        ContentChanged?.Invoke();
    }

    public void CopyFrom(FoodDropZone other)
    {
        if (other == null)
            return;

        Inicializar();

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
    /// Impede arrastar o alimento deste drop e receber novos alimentos.
    /// </summary>
    public void Bloquear()
    {
        Inicializar();
        Bloqueado = true;

        // Sem raycast na imagem, o arraste vai para o Scroll View do cardápio (a rolagem continua funcionando).
        if (_foodImage != null)
            _foodImage.raycastTarget = false;
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
