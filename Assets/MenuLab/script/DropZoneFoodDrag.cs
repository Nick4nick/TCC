using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Permite arrastar o alimento que já está em um FoodDropZone (pela FoodImage dele).
/// Soltando em outro drop, o alimento é movido (ou trocado, se o outro já tiver um);
/// soltando fora de qualquer drop, o alimento é removido.
/// Adicionado automaticamente pelo FoodDropZone na sua FoodImage.
/// </summary>
public class DropZoneFoodDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private FoodDropZone _zone;
    private Image _image;
    private Canvas _canvas;

    private RectTransform _ghost;
    private bool _dragging;

    public FoodDropZone Zone => _zone;

    /// <summary>Marcado pelo drop que recebeu o alimento, para não limpar a origem ao soltar.</summary>
    public bool DropAceito { get; set; }

    public void Init(FoodDropZone zone, Image image)
    {
        _zone = zone;
        _image = image;
        _image.raycastTarget = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = false;
        DropAceito = false;

        if (_zone == null || _zone.Bloqueado || _zone.AlimentoAtual == null || _image == null || !_image.enabled)
            return;

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas != null)
            _canvas = _canvas.rootCanvas;
        if (_canvas == null)
            return;

        _dragging = true;
        CriarGhost(eventData);

        Color c = _image.color;
        c.a = 0.4f;
        _image.color = c;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_dragging || _ghost == null)
            return;

        _ghost.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_dragging)
            return;

        _dragging = false;

        if (_ghost != null)
            Destroy(_ghost.gameObject);
        _ghost = null;

        if (_image != null)
        {
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;
        }

        // Soltou fora de qualquer drop: tira o alimento deste drop.
        if (!DropAceito && _zone != null)
            _zone.Remover();

        DropAceito = false;
    }

    private void CriarGhost(PointerEventData eventData)
    {
        RectTransform source = (RectTransform)_image.transform;

        GameObject go = new GameObject("FoodDragGhost", typeof(RectTransform), typeof(Image));
        _ghost = (RectTransform)go.transform;
        _ghost.SetParent(_canvas.transform, false);
        _ghost.SetAsLastSibling();

        _ghost.sizeDelta = new Vector2(source.rect.width, source.rect.height);
        _ghost.localScale = Vector3.one;

        Image ghostImage = go.GetComponent<Image>();
        ghostImage.sprite = _image.sprite;
        ghostImage.preserveAspect = _image.preserveAspect;
        ghostImage.color = _image.color;
        ghostImage.raycastTarget = false;

        RectTransform canvasRect = (RectTransform)_canvas.transform;
        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, cam, out Vector2 local))
        {
            _ghost.anchorMin = _ghost.anchorMax = new Vector2(0.5f, 0.5f);
            _ghost.anchoredPosition = local;
        }
    }
}
