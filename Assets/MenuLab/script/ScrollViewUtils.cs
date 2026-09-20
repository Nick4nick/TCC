using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollViewUtils
{
    public static void ResetContentPositionY(Transform root)
    {
        if (root == null)
            return;

        ScrollRect[] scrollRects = root.GetComponentsInChildren<ScrollRect>(true);
        foreach (ScrollRect scrollRect in scrollRects)
        {
            Reset(scrollRect);
        }
    }

    /// <summary>
    /// Reseta o ScrollRect mais próximo entre os ancestrais de target.
    /// Útil quando target é uma "página" dentro do Content de um Scroll View
    /// já existente (ex: painéis que se alternam via SetActive), caso em que
    /// o Scroll View não está dentro de target, e sim acima dele na hierarquia.
    /// </summary>
    public static void ResetAncestorScrollView(Transform target)
    {
        if (target == null)
            return;

        Reset(target.GetComponentInParent<ScrollRect>());
    }

    private static void Reset(ScrollRect scrollRect)
    {
        if (scrollRect == null || scrollRect.content == null)
            return;

        // StopMovement zera a velocity/inércia; sem isso o ScrollRect pode
        // sobrescrever a posição manual no próximo frame (LateUpdate interno).
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = 1f;

        Vector2 pos = scrollRect.content.anchoredPosition;
        pos.y = 0f;
        scrollRect.content.anchoredPosition = pos;
    }

    /// <summary>
    /// Redimensiona o Content de um Scroll View para acompanhar a altura de UM texto específico dentro dele
    /// (ex: o Content do Notepad acompanhando o FeedbackTxt), sem Layout Group — então nenhum outro
    /// componente dentro do Content é reposicionado, só a altura do próprio texto e a do Content mudam.
    ///
    /// Assume que o texto está ancorado/pivotado no topo do Content (padrão para texto dentro de um
    /// Scroll View), de forma que ele só "cresce para baixo" e não empurra o que está acima dele.
    /// </summary>
    public static void ResizeContentToText(ScrollRect scrollRect, TMP_Text text, float bottomPadding = 0f)
    {
        if (scrollRect == null || scrollRect.content == null || text == null)
            return;

        RectTransform content = scrollRect.content;
        RectTransform textRect = text.rectTransform;

        ContentSizeFitter fitter = textRect.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = textRect.gameObject.AddComponent<ContentSizeFitter>();

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);

        float distanceFromContentTop = -textRect.anchoredPosition.y;
        float neededHeight = distanceFromContentTop + textRect.rect.height + bottomPadding;

        float minHeight = scrollRect.viewport != null ? scrollRect.viewport.rect.height : 0f;

        Vector2 size = content.sizeDelta;
        size.y = Mathf.Max(minHeight, neededHeight);
        content.sizeDelta = size;

        Reset(scrollRect);
    }
}
