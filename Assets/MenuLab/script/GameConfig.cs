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
        public GameObject _menuLab;

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
        [HideInInspector] public string GameToLoad => nameof(StartMenuLabGame);

        private MenuManager _gameMenu;

        private GameState _gameState = GameState.OutGame;

        private void Awake()
        {
            SetupAll();
        }

        private void SetupAll()
        {
            SetupMenuEvents();
        }

        public void StartMenuLabGame()
        {
            _menuLab.SetActive(true);

            _gameState = GameState.InGame;
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
            if (_mapMenu == null || _mapMenu.ButtonsList == null)
                return;
            if (_mapMenu.ButtonsList.Count > 0)
                _mapMenu.ButtonsList[0].onClick.AddListener(() => _mainMenu.Open());
            if (_mapMenu.ButtonsList.Count > 1)
                _mapMenu.ButtonsList[1].onClick.AddListener(() =>
            {
                _gameMenu.CloseMenus();
                Invoke(GameToLoad, 0f);
            });
            if (_mapMenu.ButtonsList.Count > 2)
                _mapMenu.ButtonsList[2].onClick.AddListener(() =>
            {
                _gameMenu.CloseMenus();
                Invoke(GameToLoad, 0f);
            });
            if (_mapMenu.ButtonsList.Count > 3)
                _mapMenu.ButtonsList[3].onClick.AddListener(() =>
            {
                _gameMenu.CloseMenus();
                Invoke(GameToLoad, 0f);
            });
            if (_mapMenu.ButtonsList.Count > 4)
                _mapMenu.ButtonsList[4].onClick.AddListener(() =>
            {
                _gameMenu.CloseMenus();
                Invoke(GameToLoad, 0f);
            });
            if (_mapMenu.ButtonsList.Count > 5)
                _mapMenu.ButtonsList[5].onClick.AddListener(() =>
            {
                _gameMenu.CloseMenus();
                Invoke(GameToLoad, 0f);
            });

            //FeedbackMenu
            if (_creditsMenu == null || _creditsMenu.ButtonsList == null)
                return;
            if (_creditsMenu.ButtonsList.Count > 0)
                _creditsMenu.ButtonsList[0].onClick.AddListener(() => _mainMenu.Open());
            if (_creditsMenu.ButtonsList.Count > 1)
                _creditsMenu.ButtonsList[1].onClick.AddListener(() => _mainMenu.Open());
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
