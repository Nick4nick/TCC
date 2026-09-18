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
}
