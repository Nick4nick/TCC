using System;
using System.Collections;
using UnityEngine;

public enum Turn
{
    on,
    off
}

[Serializable]
public class MenuPanel
{
    public string Name;
    public CanvasGroup Group;
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuPanel[] _menus;
    [SerializeField] private CanvasGroup _menuBackground;
    [SerializeField] private float _transitionDuration = 0.25f;
    [SerializeField] private string _initialMenu;

    private string _currentCanvasGroup;

    private void Start()
    {
        if (!string.IsNullOrEmpty(_initialMenu))
        {
            OpenMenu(_initialMenu);
        }
    }

    public void OpenMenu(string menuName, Turn turnBackground = Turn.on)
    {
        MenuPanel m = Array.Find(_menus, menu => menu.Name.ToUpper() == menuName.ToUpper());
        if (m == null)
        {
            Debug.LogError("There is no one Menu with the name " + menuName + "!\n" + "Please, use a valid menu name");
            return;
        }

        _currentCanvasGroup = m.Name;

        StartCoroutine(SwitchCanvasGroupAnimated(_menuBackground, turnBackground));

        foreach (MenuPanel menu in _menus)
        {
            StartCoroutine(SwitchCanvasGroupAnimated(menu.Group, menu == m ? Turn.on : Turn.off));
        }
    }

    public void CloseMenus()
    {
        StartCoroutine(SwitchCanvasGroupAnimated(_menuBackground, Turn.off));
        foreach (MenuPanel menu in _menus)
        {
            StartCoroutine(SwitchCanvasGroupAnimated(menu.Group, Turn.off));
        }
    }

    private IEnumerator SwitchCanvasGroupAnimated(CanvasGroup group, Turn turn)
    {
        if (group == null)
            yield break;

        float startAlpha = group.alpha;
        float targetAlpha = turn == Turn.on ? 1f : 0f;
        float elapsed = 0f;

        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _transitionDuration);
            yield return null;
        }

        group.alpha = targetAlpha;
        group.interactable = turn == Turn.on;
        group.blocksRaycasts = turn == Turn.on;
    }
}
