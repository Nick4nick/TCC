using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referências (preenchidas automaticamente se vazias)")]
    [SerializeField] private CategorizacaoAlimento _dados;
    [SerializeField] private Image _image;
    [Tooltip("Texto com o nome do alimento, copiado para o Food Name Txt do drop ao soltar")]
    [SerializeField] private TMP_Text _nameText;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;

    private Transform _startParent;
    private int _startSiblingIndex;
    private Vector2 _startAnchoredPosition;

    public CategorizacaoAlimento Dados => _dados;
    public Sprite Sprite => _image != null ? _image.sprite : null;
    public string NomeExibido => _nameText != null ? _nameText.text : string.Empty;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (_dados == null)
            _dados = GetComponentInChildren<CategorizacaoAlimento>();
        if (_image == null)
            _image = GetComponentInChildren<Image>();
        if (_nameText == null)
            _nameText = GetComponentInChildren<TMP_Text>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startParent = transform.parent;
        _startSiblingIndex = transform.GetSiblingIndex();
        _startAnchoredPosition = _rectTransform.anchoredPosition;

        _canvasGroup.blocksRaycasts = false;

        if (_canvas != null)
        {
            transform.SetParent(_canvas.transform, true);
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null)
            return;

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        transform.SetParent(_startParent, true);
        transform.SetSiblingIndex(_startSiblingIndex);
        _rectTransform.anchoredPosition = _startAnchoredPosition;
    }
}
