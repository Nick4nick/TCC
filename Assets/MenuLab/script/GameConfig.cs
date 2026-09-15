using UnityEngine;

namespace GameTemplate
{
    public enum GameState
    {
        OutGame,
        InGame,
        Preaparing
    }

    public class GameConfig : MonoBehaviour
    {
        [Space(5)]
        [Header("Menus")]
        [SerializeField] private Menus _mainMenu;
        [SerializeField] private Menus _creditsMenu;
        [SerializeField] private Menus _mapMenu;
        [SerializeField] private Menus _feedbackMenu;
        [HideInInspector] public const string MainMenuName = "MainMenu";
        [HideInInspector] public const string CreditsMenuName = "CreditsMenu";
        [HideInInspector] public const string MapMenuName = "MapMenu";
        [HideInInspector] public const string FeedbackMenuName = "FeedbackMenu";

        private GameState _state = GameState.OutGame;

        private void Awake()
        {
            SetupAll();
        }

        private void SetupAll()
        {
            SetupMenuEvents();
        }

        private void SetupMenuEvents()
        {
            //MainMenu
            if (_mainMenu == null || _mainMenu.ButtonsList == null)
                return;
            if (_mainMenu.ButtonsList.Count > 0)
                _mainMenu.ButtonsList[0].onClick.AddListener(() => _mapMenu.Open());
            if (_mainMenu.ButtonsList.Count > 1)
                _mainMenu.ButtonsList[1].onClick.AddListener(QuitGame);
            if (_mainMenu.ButtonsList.Count > 2)
                _mainMenu.ButtonsList[2].onClick.AddListener(() => _creditsMenu.Open());

            //CreditsMenu
            if (_creditsMenu == null || _creditsMenu.ButtonsList == null)
                return;
            if (_creditsMenu.ButtonsList.Count > 0)
                _creditsMenu.ButtonsList[0].onClick.AddListener(() => _mainMenu.Open());

            //MapMenu


            //FeedbackMenu
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }
    }
}
