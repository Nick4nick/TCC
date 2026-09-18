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

    [System.Serializable]
    public class PageJumpButton
    {
        public Button Button;
        public int PageIndex;
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
        [Tooltip("Botão que fecha o jogo e abre a tela de FeedbackMenu")]
        [SerializeField] private Button _GameCloseButton;
        [Tooltip("Empty de cada fase, na mesma ordem do Buttons List do MapMenu (elemento 1 a 5)")]
        [SerializeField] private List<GameObject> _phaseEmpties;
        [Tooltip("Scroll View do Pantry (lista de alimentos), presente durante todas as fases")]
        [SerializeField] private RectTransform _pantry;

        [Space(5)]
        [Header("Briefing")]
        [Tooltip("Botão que abre o Briefing")]
        [SerializeField] private Button _briefingButton;
        [Tooltip("Empty do Briefing (deve estar ativo ao iniciar o jogo)")]
        [SerializeField] private GameObject _briefingEmpty;
        [SerializeField] private Button _briefingCloseButton1;
        [SerializeField] private Button _briefingCloseButton2;
        [SerializeField] private TMP_Text _briefingFaseText;
        [SerializeField] private TMP_Text _briefingTemaText;
        [SerializeField] private TMP_Text _briefingLocalText;
        [SerializeField] private TMP_Text _briefingPublicoText;
        [SerializeField] private TMP_Text _briefingRefeicaoText;
        [SerializeField] private TMP_Text _briefingTextoText;

        [Space(5)]
        [Header("HelpBook")]
        [Tooltip("Botão que abre o HelpBook")]
        [SerializeField] private Button _helpBookButton;
        [Tooltip("Empty do HelpBook (livro)")]
        [SerializeField] private GameObject _helpBookEmpty;
        [SerializeField] private Button _helpBookCloseButton1;
        [SerializeField] private Button _helpBookCloseButton2;
        [Tooltip("Páginas do HelpBook, na ordem de exibição")]
        [SerializeField] private List<GameObject> _helpBookPages;
        [SerializeField] private Button _helpBookNextPageButton;
        [SerializeField] private Button _helpBookPreviousPageButton;
        [Tooltip("Botões de pulo para capítulo, cada um aponta para o índice da página em HelpBook Pages")]
        [SerializeField] private List<PageJumpButton> _helpBookChapterJumpButtons;
        [Tooltip("Botões de pulo para o alfabeto, cada um aponta para o índice da página em HelpBook Pages")]
        [SerializeField] private List<PageJumpButton> _helpBookAlphabetJumpButtons;

        [Space(5)]
        [Header("Fase 5 - Troca de Empty")]
        [SerializeField] private Button _fase5PreviousEmptyButton;
        [SerializeField] private Button _fase5NextEmptyButton;
        [Tooltip("Empties que alternam entre si na Fase 5 (navegação circular)")]
        [SerializeField] private List<GameObject> _fase5Empties;

        [HideInInspector] public const string MainMenuName = "MainMenu";
        [HideInInspector] public const string CreditsMenuName = "CreditsMenu";
        [HideInInspector] public const string MapMenuName = "MapMenu";
        [HideInInspector] public const string FeedbackMenuName = "FeedbackMenu";
        [HideInInspector] public string GameToLoad => nameof(StartMenuLabGame);


        private GameState _gameState = GameState.OutGame;

        private int _selectedLevelIndex = -1;

        private int _helpBookPageIndex = 0;

        private int _fase5EmptyIndex = 0;

        private static readonly string[][] _levelTexts =
        {
            new[] { "fase 1", "café da manhã na creche", "Monte um café da manhã adequado para crianças de 4 a 5 anos" },
            new[] { "fase 2", "almoço hospitalar", "Montar um almoço para pacientes adultos internados, evitando qualquer preparação que contenha leite ou derivados." },
            new[] { "fase 3", "almoço no refeitório da empresa", "Montar um cardápio de almoço variado, combinando bem cores, sabores, texturas e métodos de preparo entre os pratos." },
            new[] { "fase 4", "jantar na instituição", "Montar um jantar adequado ao público atendido, também com boa variedade e combinação entre as preparações." },
            new[] { "fase 5", "desafio final", "Sem o livro de apoio, montar sozinho o cardápio de um dia inteiro: café da manhã, almoço e jantar, aplicando tudo o que foi aprendido nas fases anteriores." },
        };

        private static readonly string[][] _briefingTexts =
        {
            new[]
            {
                "Fase 1",
                "Café da manhã na creche",
                "Creche municipal",
                "Crianças de 4 a 5 anos",
                "Café da manhã",
                @"Você é o nutricionista responsável pelo planejamento do café da manhã da creche.

A refeição será oferecida para crianças de 4 a 5 anos antes do início das atividades.

Monte uma refeição adequada considerando as características do público atendido."
            },
            new[]
            {
                "Fase 2",
                "Almoço hospitalar",
                "Hospital",
                "Adultos com intolerância à lactose",
                "Almoço",
                @"Você é responsável pelo planejamento do almoço de pacientes adultos internados.

Para esta fase, considere que os pacientes apresentam intolerância à lactose.

Monte um almoço adequado, evitando preparações que contenham ingredientes incompatíveis com a restrição apresentada."
            },
            new[]
            {
                "Fase 3",
                "Almoço no refeitório da empresa",
                "Refeitório de uma empresa",
                "Trabalhadores adultos",
                "Almoço",
                @"Você é responsável pelo planejamento do almoço servido no refeitório.

O cardápio deve apresentar variedade entre as preparações, buscando uma boa combinação de cores, sabores, texturas e métodos de preparo.

Monte o cardápio completo."
            },
            new[]
            {
                "Fase 4",
                "Jantar na instituição",
                "Casa de repouso",
                "Pessoas com 65 anos ou mais",
                "Jantar",
                @"Você é responsável pelo planejamento do jantar da instituição.

A refeição deverá apresentar preparações adequadas ao público atendido, considerando variedade, combinação entre os alimentos e características das preparações.

Monte o jantar."
            },
            new[]
            {
                "Fase 5",
                "Desafio final",
                "Refeitório de uma empresa",
                "Trabalhadores adultos",
                "Café da manhã, almoço e jantar",
                @"Você chegou ao desafio final.
Durante as fases anteriores, você aprendeu a considerar diferentes públicos, tipos de refeição, composição dos cardápios, restrições alimentares, variedade e combinação entre preparações.
Agora você deverá planejar todas as principais refeições de um dia.

O cardápio deverá ser composto por: <b>Café da manhã, Almoço e Jantar.</b>

<b>O livro de apoio não estará disponível nesta fase.</b>
Utilize os conhecimentos adquiridos durante as fases anteriores."
            },
        };

        private void Awake()
        {
            SetupAll();
        }

        private void OnEnable()
        {
            FoodDropZone.ContentChanged += UpdateGameCloseButtonInteractable;
        }

        private void OnDisable()
        {
            FoodDropZone.ContentChanged -= UpdateGameCloseButtonInteractable;
        }

        private void SetupAll()
        {
            SetupMenuEvents();
        }

        public void StartMenuLabGame()
        {
            _menuLab.SetActive(true);

            ActivatePhaseEmpty(_selectedLevelIndex);

            OpenBriefing();

            _gameState = GameState.InGame;
        }

        private void ExitMenuLabGame()
        {
            _menuLab.SetActive(false);

            DeactivateAllPhaseEmpties();

            _gameState = GameState.OutGame;

            _mapMenu.Open();
        }

        private void CloseMenuLabGame()
        {
            _menuLab.SetActive(false);

            _gameState = GameState.OutGame;

            _feedbackMenu.Open();
        }

        private void ReturnToGameFromFeedback()
        {
            _gameMenu.CloseMenus();
            Invoke(GameToLoad, 0f);
        }

        private void OpenBriefing()
        {
            if (_briefingEmpty == null)
                return;

            SetBriefingTexts(_selectedLevelIndex);

            _briefingEmpty.SetActive(true);

            ScrollViewUtils.ResetContentPositionY(_briefingEmpty.transform);
        }

        private void CloseBriefing()
        {
            if (_briefingEmpty == null)
                return;

            _briefingEmpty.SetActive(false);
        }

        private void SetBriefingTexts(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= _briefingTexts.Length)
                return;

            string[] texts = _briefingTexts[levelIndex];

            if (_briefingFaseText != null)
                _briefingFaseText.text = texts[0];
            if (_briefingTemaText != null)
                _briefingTemaText.text = texts[1];
            if (_briefingLocalText != null)
                _briefingLocalText.text = texts[2];
            if (_briefingPublicoText != null)
                _briefingPublicoText.text = texts[3];
            if (_briefingRefeicaoText != null)
                _briefingRefeicaoText.text = texts[4];
            if (_briefingTextoText != null)
                _briefingTextoText.text = texts[5];
        }

        private void OpenHelpBook()
        {
            if (_helpBookEmpty == null)
                return;

            _helpBookEmpty.SetActive(true);

            ShowHelpBookPage(0);
        }

        private void CloseHelpBook()
        {
            if (_helpBookEmpty == null)
                return;

            _helpBookEmpty.SetActive(false);
        }

        private void ShowHelpBookPage(int pageIndex)
        {
            if (_helpBookPages == null || _helpBookPages.Count == 0)
                return;

            pageIndex = Mathf.Clamp(pageIndex, 0, _helpBookPages.Count - 1);

            for (int i = 0; i < _helpBookPages.Count; i++)
            {
                if (_helpBookPages[i] != null)
                    _helpBookPages[i].SetActive(i == pageIndex);
            }

            if (_helpBookPages[pageIndex] != null)
                ScrollViewUtils.ResetContentPositionY(_helpBookPages[pageIndex].transform);

            _helpBookPageIndex = pageIndex;

            UpdateHelpBookNavigationButtons();
        }

        private void UpdateHelpBookNavigationButtons()
        {
            if (_helpBookPages == null)
                return;

            if (_helpBookNextPageButton != null)
                _helpBookNextPageButton.interactable = _helpBookPageIndex < _helpBookPages.Count - 1;

            if (_helpBookPreviousPageButton != null)
                _helpBookPreviousPageButton.interactable = _helpBookPageIndex > 0;
        }

        private void NextHelpBookPage()
        {
            ShowHelpBookPage(_helpBookPageIndex + 1);
        }

        private void PreviousHelpBookPage()
        {
            ShowHelpBookPage(_helpBookPageIndex - 1);
        }

        private void ShowFase5Empty(int index)
        {
            if (_fase5Empties == null || _fase5Empties.Count == 0)
                return;

            for (int i = 0; i < _fase5Empties.Count; i++)
            {
                if (_fase5Empties[i] != null)
                    _fase5Empties[i].SetActive(i == index);
            }

            if (_fase5Empties[index] != null)
            {
                ScrollViewUtils.ResetContentPositionY(_fase5Empties[index].transform);
                ScrollViewUtils.ResetAncestorScrollView(_fase5Empties[index].transform);
            }

            _fase5EmptyIndex = index;
        }

        private void NextFase5Empty()
        {
            if (_fase5Empties == null || _fase5Empties.Count == 0)
                return;

            int nextIndex = _fase5EmptyIndex + 1;
            if (nextIndex >= _fase5Empties.Count)
                nextIndex = 0;

            ShowFase5Empty(nextIndex);
        }

        private void PreviousFase5Empty()
        {
            if (_fase5Empties == null || _fase5Empties.Count == 0)
                return;

            int previousIndex = _fase5EmptyIndex - 1;
            if (previousIndex < 0)
                previousIndex = _fase5Empties.Count - 1;

            ShowFase5Empty(previousIndex);
        }

        private void ActivatePhaseEmpty(int levelIndex)
        {
            if (_phaseEmpties == null)
                return;

            DeactivateAllPhaseEmpties();

            if (levelIndex < 0 || levelIndex >= _phaseEmpties.Count || _phaseEmpties[levelIndex] == null)
                return;

            _phaseEmpties[levelIndex].SetActive(true);

            ScrollViewUtils.ResetContentPositionY(_phaseEmpties[levelIndex].transform);
            ScrollViewUtils.ResetContentPositionY(_pantry);

            ShowFase5Empty(0);

            UpdateGameCloseButtonInteractable();
        }

        private void UpdateGameCloseButtonInteractable()
        {
            if (_GameCloseButton == null)
                return;

            _GameCloseButton.interactable = AllDropZonesFilled();
        }

        private bool AllDropZonesFilled()
        {
            if (_phaseEmpties == null || _selectedLevelIndex < 0 || _selectedLevelIndex >= _phaseEmpties.Count)
                return false;

            GameObject currentPhaseEmpty = _phaseEmpties[_selectedLevelIndex];
            if (currentPhaseEmpty == null)
                return false;

            FoodDropZone[] dropZones = currentPhaseEmpty.GetComponentsInChildren<FoodDropZone>(true);
            if (dropZones.Length == 0)
                return false;

            foreach (FoodDropZone dropZone in dropZones)
            {
                if (dropZone.AlimentoAtual == null)
                    return false;
            }

            return true;
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

        private void ConfirmFinalizarJogo()
        {
            PopUpManager.Instance.Abrir(
                "Deseja finalizar o jogo?",
                "Finalizar",
                "Voltar",
                (msg, esq, dir) => PopUpManager.Instance.Fechar(),
                (msg, esq, dir) =>
                {
                    PopUpManager.Instance.Fechar();
                    CloseMenuLabGame();
                });
        }

        private void OpenLevelSelected(int levelIndex)
        {
            if (_levelSelectedConfig == null || _levelSelectedConfig.Empty == null)
                return;

            _selectedLevelIndex = levelIndex;

            SetLevelTexts(levelIndex);

            _levelSelectedConfig.Empty.SetActive(true);

            ScrollViewUtils.ResetContentPositionY(_levelSelectedConfig.Empty.transform);
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
            if (_GameCloseButton != null)
                _GameCloseButton.onClick.AddListener(ConfirmFinalizarJogo);

            //Briefing
            if (_briefingButton != null)
                _briefingButton.onClick.AddListener(OpenBriefing);
            if (_briefingCloseButton1 != null)
                _briefingCloseButton1.onClick.AddListener(CloseBriefing);
            if (_briefingCloseButton2 != null)
                _briefingCloseButton2.onClick.AddListener(CloseBriefing);

            //HelpBook
            if (_helpBookButton != null)
                _helpBookButton.onClick.AddListener(OpenHelpBook);
            if (_helpBookCloseButton1 != null)
                _helpBookCloseButton1.onClick.AddListener(CloseHelpBook);
            if (_helpBookCloseButton2 != null)
                _helpBookCloseButton2.onClick.AddListener(CloseHelpBook);
            if (_helpBookNextPageButton != null)
                _helpBookNextPageButton.onClick.AddListener(NextHelpBookPage);
            if (_helpBookPreviousPageButton != null)
                _helpBookPreviousPageButton.onClick.AddListener(PreviousHelpBookPage);

            if (_helpBookChapterJumpButtons != null)
            {
                foreach (PageJumpButton jump in _helpBookChapterJumpButtons)
                {
                    if (jump == null || jump.Button == null)
                        continue;

                    int pageIndex = jump.PageIndex;
                    jump.Button.onClick.AddListener(() => ShowHelpBookPage(pageIndex));
                }
            }

            if (_helpBookAlphabetJumpButtons != null)
            {
                foreach (PageJumpButton jump in _helpBookAlphabetJumpButtons)
                {
                    if (jump == null || jump.Button == null)
                        continue;

                    int pageIndex = jump.PageIndex;
                    jump.Button.onClick.AddListener(() => ShowHelpBookPage(pageIndex));
                }
            }

            //Fase 5 - Troca de Empty
            if (_fase5PreviousEmptyButton != null)
                _fase5PreviousEmptyButton.onClick.AddListener(PreviousFase5Empty);
            if (_fase5NextEmptyButton != null)
                _fase5NextEmptyButton.onClick.AddListener(NextFase5Empty);

            //FeedbackMenu
            if (_feedbackMenu == null || _feedbackMenu.ButtonsList == null)
                return;
            if (_feedbackMenu.ButtonsList.Count > 0)
                _feedbackMenu.ButtonsList[0].onClick.AddListener(ReturnToGameFromFeedback);
            if (_feedbackMenu.ButtonsList.Count > 1)
                _feedbackMenu.ButtonsList[1].onClick.AddListener(ExitMenuLabGame);
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
