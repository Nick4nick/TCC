using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private MenuManager _gameMenu;

        [Space(5)]
        [Header("Menus")]
        [SerializeField] private Menus _mainMenu;
        [SerializeField] private Menus _creditsMenu;
        [SerializeField] private Menus _mapMenu;
        [SerializeField] private Menus _feedbackMenu;

        [Space(5)]
        [Header("Level Selection (MapMenu)")]
        [SerializeField] private LevelSelectedConfig _levelSelectedConfig;

        [Space(5)]
        [Header("MenuLab (Game)")]
        [SerializeField] private Button _GameExitButton;
        [Tooltip("Empty de cada fase, na mesma ordem do Buttons List do MapMenu (elemento 1 a 5)")]
        [SerializeField] private List<GameObject> _phaseEmpties;

        [HideInInspector] public const string MainMenuName = "MainMenu";
        [HideInInspector] public const string CreditsMenuName = "CreditsMenu";
        [HideInInspector] public const string MapMenuName = "MapMenu";
        [HideInInspector] public const string FeedbackMenuName = "FeedbackMenu";
        [HideInInspector] public string GameToLoad => nameof(StartMenuLabGame);


        private GameState _gameState = GameState.OutGame;

        private int _selectedLevelIndex = -1;

        private static readonly string[][] _levelTexts =
        {
            new[] { "fase 1", "café da manhã na creche", "Monte um café da manhã adequado para crianças de 4 a 5 anos" },
            new[] { "fase 2", "almoço hospitalar", "Montar um almoço para pacientes adultos internados, evitando qualquer preparação que contenha leite ou derivados." },
            new[] { "fase 3", "almoço no refeitório da empresa", "Montar um cardápio de almoço variado, combinando bem cores, sabores, texturas e métodos de preparo entre os pratos." },
            new[] { "fase 4", "jantar na instituição", "Montar um jantar adequado ao público atendido, também com boa variedade e combinação entre as preparações." },
            new[] { "fase 5", "desafio final", "Sem o livro de apoio, montar sozinho o cardápio de um dia inteiro: café da manhã, almoço e jantar, aplicando tudo o que foi aprendido nas fases anteriores." },
        };

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

            ActivatePhaseEmpty(_selectedLevelIndex);

            _gameState = GameState.InGame;
        }

        private void ExitMenuLabGame()
        {
            _menuLab.SetActive(false);

            DeactivateAllPhaseEmpties();

            _gameState = GameState.OutGame;

            _mapMenu.Open();
        }

        private void ActivatePhaseEmpty(int levelIndex)
        {
            if (_phaseEmpties == null)
                return;

            DeactivateAllPhaseEmpties();

            if (levelIndex < 0 || levelIndex >= _phaseEmpties.Count || _phaseEmpties[levelIndex] == null)
                return;

            _phaseEmpties[levelIndex].SetActive(true);
        }

        private void DeactivateAllPhaseEmpties()
        {
            if (_phaseEmpties == null)
                return;

            foreach (GameObject phaseEmpty in _phaseEmpties)
            {
                if (phaseEmpty != null)
                    phaseEmpty.SetActive(false);
            }
        }

        private void ConfirmQuitGame()
        {
            PopUpManager.Instance.Abrir(
                "Certeza que deseja sair do jogo",
                "Sair",
                "Voltar",
                (msg, esq, dir) => PopUpManager.Instance.Fechar(),
                (msg, esq, dir) =>
                {
                    PopUpManager.Instance.Fechar();
                    QuitGame();
                });
        }

        private void ConfirmBackToMainMenu()
        {
            PopUpManager.Instance.Abrir(
                "Certeza que deseja voltar para a tela inicial?",
                "Voltar",
                "Continuar",
                (msg, esq, dir) => PopUpManager.Instance.Fechar(),
                (msg, esq, dir) =>
                {
                    PopUpManager.Instance.Fechar();
                    _mainMenu.Open();
                });
        }

        private void ConfirmExitLevel()
        {
            PopUpManager.Instance.Abrir(
                "Certeza que deseja sair da fase?\nO progresso feito não será salvo",
                "Sair",
                "Voltar",
                (msg, esq, dir) => PopUpManager.Instance.Fechar(),
                (msg, esq, dir) =>
                {
                    PopUpManager.Instance.Fechar();
                    ExitMenuLabGame();
                });
        }

        private void OpenLevelSelected(int levelIndex)
        {
            if (_levelSelectedConfig == null || _levelSelectedConfig.Empty == null)
                return;

            _selectedLevelIndex = levelIndex;

            SetLevelTexts(levelIndex);

            _levelSelectedConfig.Empty.SetActive(true);
        }

        private void SetLevelTexts(int levelIndex)
        {
            if (_levelSelectedConfig.TextsList == null)
                return;
            if (levelIndex < 0 || levelIndex >= _levelTexts.Length)
                return;

            string[] texts = _levelTexts[levelIndex];
            for (int i = 0; i < texts.Length && i < _levelSelectedConfig.TextsList.Count; i++)
                _levelSelectedConfig.TextsList[i].text = texts[i];
        }

        private void CloseLevelSelected()
        {
            if (_levelSelectedConfig == null || _levelSelectedConfig.Empty == null)
                return;

            _levelSelectedConfig.Empty.SetActive(false);
        }

        private void SetupMenuEvents()
        {
            //MainMenu
            if (_mainMenu == null || _mainMenu.ButtonsList == null)
                return;
            if (_mainMenu.ButtonsList.Count > 0)
                _mainMenu.ButtonsList[0].onClick.AddListener(() => _mapMenu.Open());
            if (_mainMenu.ButtonsList.Count > 1)
                _mainMenu.ButtonsList[1].onClick.AddListener(ConfirmQuitGame);
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
                _mapMenu.ButtonsList[0].onClick.AddListener(ConfirmBackToMainMenu);
            for (int i = 1; i < _mapMenu.ButtonsList.Count; i++)
            {
                int levelIndex = i - 1;
                _mapMenu.ButtonsList[i].onClick.AddListener(() => OpenLevelSelected(levelIndex));
            }

            //LevelSelectedConfig (popup aberto ao clicar em uma fase do mapa)
            if (_levelSelectedConfig != null && _levelSelectedConfig.ButtonsList != null)
            {
                if (_levelSelectedConfig.ButtonsList.Count > 0)
                    _levelSelectedConfig.ButtonsList[0].onClick.AddListener(CloseLevelSelected);
                if (_levelSelectedConfig.ButtonsList.Count > 1)
                    _levelSelectedConfig.ButtonsList[1].onClick.AddListener(CloseLevelSelected);
                if (_levelSelectedConfig.ButtonsList.Count > 2)
                    _levelSelectedConfig.ButtonsList[2].onClick.AddListener(() =>
                {
                    CloseLevelSelected();
                    _gameMenu.CloseMenus();
                    Invoke(GameToLoad, 0f);
                });
            }

            //MenuLab (game)
            if (_GameExitButton != null)
                _GameExitButton.onClick.AddListener(ConfirmExitLevel);

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
