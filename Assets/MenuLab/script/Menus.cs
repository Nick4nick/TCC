using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Menus : MonoBehaviour
{
    [Header("Base Menu Manager Comunication")]
    [SerializeField] private MenuManager _menuManager;
    [Tooltip("Menu name for Menu Manager identification. Leave empty to use GameObject name")]
    [SerializeField] private string _menuName;

    [Header("References")]
    public List<Button> ButtonsList;
    public List<TMP_Text> TextsList;
    public List<Image> ImagesList;

    private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            _canvasGroup ??= GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(_menuName))
            _menuName = gameObject.name;
    }

    public virtual void Open()
    {
        if (_menuManager != null)
            _menuManager.OpenMenu(_menuName);
    }
}
