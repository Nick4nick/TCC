using System.Collections;
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

        [System.NonSerialized] public TMP_Text Text;
        [System.NonSerialized] public Color DefaultColor;
    }

    public class GameConfig : MonoBehaviour
    {
        public GameObject _menuLab;
        [SerializeField] private MenuManager _gameMenu;

        [Space(5)]
        [Header("Fade")]
        [Tooltip("Duração do fade-in/fade-out do Briefing, HelpBook e Level Selected")]
        [SerializeField] private float _fadeDuration = 0.25f;

        [Space(5)]
        [Header("Menus")]
        [SerializeField] private Menus _mainMenu;
        [SerializeField] private Menus _creditsMenu;
        [SerializeField] private Menus _mapMenu;
        [SerializeField] private Menus _feedbackMenu;

        [Space(5)]
        [Header("MapMenu")]
        [SerializeField] private LevelSelectedConfig _levelSelectedConfig;
        [SerializeField] private List<Image> _paperLevelSelected;

        [Space(5)]
        [Header("FeedbackMenu")]
        [SerializeField] private GameObject _menuMade;

        [Space(5)]
        [Header("MenuLab (Game)")]
        [SerializeField] private Button _GameExitButton;
        [Tooltip("Botão que fecha o jogo e abre a tela de FeedbackMenu")]
        [SerializeField] private Button _GameCloseButton;
        [Tooltip("Empty de cada fase, na mesma ordem do Buttons List do MapMenu (elemento 1 a 5)")]
        [SerializeField] private List<GameObject> _phase;
        [Tooltip("Scroll View do Pantry (lista de alimentos), presente durante todas as fases")]
        [SerializeField] private RectTransform _pantry;

        [Space(5)]
        [Header("Briefing")]
        [Tooltip("Botão que abre o Briefing")]
        [SerializeField] private Button _briefingButton;
        [Tooltip("Empty do Briefing (deve estar ativo ao iniciar o jogo)")]
        [SerializeField] private GameObject _briefing;
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
        [SerializeField] private GameObject _helpBook;
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
        [Tooltip("Cor do texto do Chapter Jump Button enquanto a página exibida pertence ao seu capítulo")]
        [SerializeField] private Color _helpBookChapterJumpButtonHighlightColor = new Color32(0xBA, 0xA7, 0x97, 0xFF);

        [Space(5)]
        [Header("Fase 5 - Troca de cardápio")]
        [SerializeField] private Button _fase5PreviousButton;
        [SerializeField] private Button _fase5NextButton;
        [Tooltip("Empties que alternam entre si na Fase 5 (navegação circular)")]
        [SerializeField] private List<GameObject> _fase5Cardapios;

        [HideInInspector] public const string MainMenuName = "MainMenu";
        [HideInInspector] public const string CreditsMenuName = "CreditsMenu";
        [HideInInspector] public const string MapMenuName = "MapMenu";
        [HideInInspector] public const string FeedbackMenuName = "FeedbackMenu";
        [HideInInspector] public string GameToLoad => nameof(StartMenuLabGame);


        private const float NotaMinimaParaPassar = 7f;

        private const string SaveHasSaveKey = "MenuLab_HasSave";
        private const string SavePhaseCompletedKeyPrefix = "MenuLab_PhaseCompleted_";

        private GameState _gameState = GameState.OutGame;

        private readonly Dictionary<CanvasGroup, Coroutine> _activeFades = new Dictionary<CanvasGroup, Coroutine>();

        private bool[] _phaseCompleted;

        private int _selectedLevelIndex = -1;

        private int _helpBookPageIndex = 0;

        private bool _helpBookChapterJumpButton0Highlighted = false;

        private int _fase5EmptyIndex = 0;

        private GameObject _menuMadeCopy;
        private List<GameObject> _menuMadeFase5CardapiosCopy;
        private int _menuMadeFase5Index = 0;

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

            _phaseCompleted = new bool[_phase != null ? _phase.Count : 0];

            ResetMapCompletionImages();
            UpdateMapLevelButtonsInteractable();
            UpdateContinueGameButtonInteractable();
        }

        private bool IsPhaseCompleted(int levelIndex)
        {
            return _phaseCompleted != null && levelIndex >= 0 && levelIndex < _phaseCompleted.Length && _phaseCompleted[levelIndex];
        }

        private void MarkPhaseCompletedIfPassed(int levelIndex, float notaFinal)
        {
            if (_phaseCompleted == null || levelIndex < 0 || levelIndex >= _phaseCompleted.Length)
                return;

            if (notaFinal < NotaMinimaParaPassar)
                return;

            _phaseCompleted[levelIndex] = true;

            if (_mapMenu != null && _mapMenu.ImagesList != null && levelIndex < _mapMenu.ImagesList.Count && _mapMenu.ImagesList[levelIndex] != null)
                _mapMenu.ImagesList[levelIndex].gameObject.SetActive(true);

            UpdateMapLevelButtonsInteractable();

            SaveGame();
        }

        private void ResetMapCompletionImages()
        {
            if (_mapMenu == null || _mapMenu.ImagesList == null)
                return;

            foreach (Image image in _mapMenu.ImagesList)
            {
                if (image != null)
                    image.gameObject.SetActive(false);
            }
        }

        private void RefreshMapCompletionImages()
        {
            if (_mapMenu == null || _mapMenu.ImagesList == null)
                return;

            for (int i = 0; i < _mapMenu.ImagesList.Count; i++)
            {
                if (_mapMenu.ImagesList[i] != null)
                    _mapMenu.ImagesList[i].gameObject.SetActive(IsPhaseCompleted(i));
            }
        }

        /// <summary>
        /// Só permite acessar uma fase (elementos 1 a 5 do Buttons List do MapMenu) quando a
        /// fase anterior já foi concluída (nota final >= 7). A Fase 1 fica sempre liberada.
        /// </summary>
        private void UpdateMapLevelButtonsInteractable()
        {
            if (_mapMenu == null || _mapMenu.ButtonsList == null)
                return;

            for (int i = 1; i < _mapMenu.ButtonsList.Count; i++)
            {
                int levelIndex = i - 1;
                bool unlocked = levelIndex == 0 || IsPhaseCompleted(levelIndex - 1);

                if (_mapMenu.ButtonsList[i] != null)
                    _mapMenu.ButtonsList[i].interactable = unlocked;
            }
        }

        // ---------- Salvamento ----------

        /// <summary>
        /// Salva quais fases já foram concluídas (nota final >= 7). Não salva o que está
        /// dentro dos drops de uma fase em andamento — só o progresso de fases concluídas.
        /// </summary>
        private void SaveGame()
        {
            if (_phaseCompleted == null)
                return;

            for (int i = 0; i < _phaseCompleted.Length; i++)
                PlayerPrefs.SetInt(SavePhaseCompletedKeyPrefix + i, _phaseCompleted[i] ? 1 : 0);

            PlayerPrefs.SetInt(SaveHasSaveKey, 1);
            PlayerPrefs.Save();

            UpdateContinueGameButtonInteractable();
        }

        private bool HasSavedGame()
        {
            return PlayerPrefs.GetInt(SaveHasSaveKey, 0) == 1;
        }

        private void LoadGame()
        {
            if (_phaseCompleted == null)
                return;

            for (int i = 0; i < _phaseCompleted.Length; i++)
                _phaseCompleted[i] = PlayerPrefs.GetInt(SavePhaseCompletedKeyPrefix + i, 0) == 1;

            RefreshMapCompletionImages();
            UpdateMapLevelButtonsInteractable();
        }

        private void ClearSavedGame()
        {
            if (_phaseCompleted != null)
            {
                for (int i = 0; i < _phaseCompleted.Length; i++)
                    _phaseCompleted[i] = false;
            }

            if (_phase != null)
            {
                for (int i = 0; i < _phase.Count; i++)
                    PlayerPrefs.DeleteKey(SavePhaseCompletedKeyPrefix + i);
            }

            PlayerPrefs.DeleteKey(SaveHasSaveKey);
            PlayerPrefs.Save();

            ResetMapCompletionImages();
            UpdateMapLevelButtonsInteractable();
            UpdateContinueGameButtonInteractable();
        }

        private void UpdateContinueGameButtonInteractable()
        {
            if (_mainMenu == null || _mainMenu.ButtonsList == null || _mainMenu.ButtonsList.Count <= 3)
                return;

            if (_mainMenu.ButtonsList[3] != null)
                _mainMenu.ButtonsList[3].interactable = HasSavedGame();
        }

        private void OnClickPlayNewGame()
        {
            if (HasSavedGame())
            {
                PopUpManager.Instance.Abrir(
                    "Você possui um jogo salvo.\nDeseja criar um novo jogo?",
                    "Novo jogo",
                    "Voltar",
                    (msg, esq, dir) => PopUpManager.Instance.Fechar(),
                    (msg, esq, dir) =>
                    {
                        PopUpManager.Instance.Fechar();
                        ClearSavedGame();
                        _mapMenu.Open();
                    });

                return;
            }

            _mapMenu.Open();
        }

        private void OnClickContinueSavedGame()
        {
            LoadGame();
            _mapMenu.Open();
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

            ClearPhaseDropZones(_selectedLevelIndex);

            DeactivateAllPhaseEmpties();

            _gameState = GameState.OutGame;

            _mapMenu.Open();
        }

        private void CloseMenuLabGame()
        {
            _menuLab.SetActive(false);

            GenerateFeedback();
            CopyPhaseToMenuMade();

            _gameState = GameState.OutGame;

            _feedbackMenu.Open();
        }

        private void GenerateFeedback()
        {
            if (_feedbackMenu == null || _feedbackMenu.TextsList == null || _feedbackMenu.TextsList.Count < 3)
                return;
            if (_phase == null || _selectedLevelIndex < 0 || _selectedLevelIndex >= _phase.Count)
                return;

            GameObject phaseEmpty = _phase[_selectedLevelIndex];
            if (phaseEmpty == null)
                return;

            bool isFase5 = _selectedLevelIndex == _phase.Count - 1;

            if (isFase5)
            {
                GenerateFase5Feedback();
                return;
            }

            RefeicaoPrato refeicao = GetRefeicaoDaFase(_selectedLevelIndex);
            FoodDropZone[] dropZones = phaseEmpty.GetComponentsInChildren<FoodDropZone>(true);
            (string feedbackText, MenuScoreResult score) = MenuFeedbackEvaluator.Evaluate(dropZones, refeicao, _selectedLevelIndex);

            _feedbackMenu.TextsList[0].text = MenuFeedbackEvaluator.BuildCriteriaScoreText(score);
            _feedbackMenu.TextsList[1].text = MenuFeedbackEvaluator.BuildNotaTotalText(score);
            _feedbackMenu.TextsList[2].text = feedbackText;

            ResizeFeedbackTextScrollView();

            SoundManager.Instance?.PlayFeedbackSfx(score.NotaFinal);
            MarkPhaseCompletedIfPassed(_selectedLevelIndex, score.NotaFinal);
        }

        private void GenerateFase5Feedback()
        {
            if (_fase5Cardapios == null || _fase5Cardapios.Count < 3)
                return;

            RefeicaoPrato[] refeicoes = { RefeicaoPrato.CafeDaManha, RefeicaoPrato.Almoco, RefeicaoPrato.Jantar };
            float[] notasFinais = new float[3];

            System.Text.StringBuilder feedbackCompleto = new System.Text.StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                GameObject cardapio = _fase5Cardapios[i];
                if (cardapio == null)
                    continue;

                FoodDropZone[] dropZones = cardapio.GetComponentsInChildren<FoodDropZone>(true);
                (string feedbackText, MenuScoreResult score) = MenuFeedbackEvaluator.Evaluate(dropZones, refeicoes[i], _selectedLevelIndex);

                notasFinais[i] = score.NotaFinal;

                if (i > 0)
                    feedbackCompleto.AppendLine();
                feedbackCompleto.AppendLine(feedbackText);
            }

            float notaTotalFase5 = (notasFinais[0] + notasFinais[1] + notasFinais[2]) / 3f;

            _feedbackMenu.TextsList[0].text = MenuFeedbackEvaluator.BuildFase5ScoreText(notasFinais[0], notasFinais[1], notasFinais[2]);
            _feedbackMenu.TextsList[1].text = MenuFeedbackEvaluator.BuildNotaTotalText(notaTotalFase5);
            _feedbackMenu.TextsList[2].text = feedbackCompleto.ToString().TrimEnd();

            ResizeFeedbackTextScrollView();

            SoundManager.Instance?.PlayFeedbackSfx(notaTotalFase5);
            MarkPhaseCompletedIfPassed(_selectedLevelIndex, notaTotalFase5);
        }

        private void ResizeFeedbackTextScrollView()
        {
            TMP_Text feedbackTxt = _feedbackMenu.TextsList[2];
            if (feedbackTxt == null)
                return;

            ScrollRect scrollRect = feedbackTxt.GetComponentInParent<ScrollRect>();
            ScrollViewUtils.ResizeContentToText(scrollRect, feedbackTxt);
        }

        private static RefeicaoPrato GetRefeicaoDaFase(int faseIndex)
        {
            switch (faseIndex)
            {
                case 0: return RefeicaoPrato.CafeDaManha; // Fase 1
                case 1: return RefeicaoPrato.Almoco;       // Fase 2
                case 2: return RefeicaoPrato.Almoco;       // Fase 3
                case 3: return RefeicaoPrato.Jantar;       // Fase 4
                default: return RefeicaoPrato.Almoco;
            }
        }

        private void ReturnToGameFromFeedback()
        {
            _gameMenu.CloseMenus();
            Invoke(GameToLoad, 0f);
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject go)
        {
            if (go == null)
                return null;

            CanvasGroup group = go.GetComponent<CanvasGroup>();
            if (group == null)
                group = go.AddComponent<CanvasGroup>();

            return group;
        }

        private void FadeIn(GameObject go, float duration)
        {
            CanvasGroup group = GetOrAddCanvasGroup(go);
            if (group == null)
                return;

            StopFade(group);

            group.gameObject.SetActive(true);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = true;

            _activeFades[group] = StartCoroutine(FadeCanvasGroup(group, true, duration));
        }

        private void FadeOut(GameObject go, float duration)
        {
            CanvasGroup group = GetOrAddCanvasGroup(go);
            if (group == null)
                return;

            StopFade(group);

            group.interactable = false;
            group.blocksRaycasts = false;

            _activeFades[group] = StartCoroutine(FadeCanvasGroup(group, false, duration));
        }

        private void StopFade(CanvasGroup group)
        {
            if (_activeFades.TryGetValue(group, out Coroutine running) && running != null)
                StopCoroutine(running);
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup group, bool fadeIn, float duration)
        {
            float startAlpha = group.alpha;
            float targetAlpha = fadeIn ? 1f : 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, targetAlpha, duration > 0f ? elapsed / duration : 1f);
                yield return null;
            }

            group.alpha = targetAlpha;
            group.interactable = fadeIn;
            group.blocksRaycasts = fadeIn;

            if (!fadeIn)
                group.gameObject.SetActive(false);

            _activeFades.Remove(group);
        }

        private void OpenBriefing()
        {
            if (_briefing == null)
                return;

            SetBriefingTexts(_selectedLevelIndex);

            if (_briefingButton != null)
                FadeOut(_briefingButton.gameObject, _fadeDuration);

            FadeIn(_briefing, _fadeDuration);

            ScrollViewUtils.ResetContentPositionY(_briefing.transform);
        }

        private void CloseBriefing()
        {
            if (_briefing == null)
                return;

            FadeOut(_briefing, _fadeDuration);

            if (_briefingButton != null)
                FadeIn(_briefingButton.gameObject, _fadeDuration);
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
            if (_helpBook == null)
                return;

            if (_helpBookButton != null)
                FadeOut(_helpBookButton.gameObject, _fadeDuration);

            FadeIn(_helpBook, _fadeDuration);

            _helpBookChapterJumpButton0Highlighted = false;
            ShowHelpBookPage(0);
        }

        private void CloseHelpBook()
        {
            if (_helpBook == null)
                return;

            FadeOut(_helpBook, _fadeDuration);

            if (_helpBookButton != null)
                FadeIn(_helpBookButton.gameObject, _fadeDuration);
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
            UpdateHelpBookChapterJumpButtonsColor();
        }

        private void UpdateHelpBookChapterJumpButtonsColor()
        {
            if (_helpBookChapterJumpButtons == null)
                return;

            for (int i = 0; i < _helpBookChapterJumpButtons.Count; i++)
            {
                PageJumpButton jump = _helpBookChapterJumpButtons[i];
                if (jump == null || jump.Text == null)
                    continue;

                bool highlighted;

                if (i == 0)
                {
                    highlighted = _helpBookChapterJumpButton0Highlighted;
                }
                else
                {
                    int rangeStart = jump.PageIndex;
                    int rangeEnd = (i + 1 < _helpBookChapterJumpButtons.Count)
                        ? _helpBookChapterJumpButtons[i + 1].PageIndex
                        : (_helpBookPages != null ? _helpBookPages.Count : rangeStart + 1);

                    highlighted = _helpBookPageIndex >= rangeStart && _helpBookPageIndex < rangeEnd;
                }

                jump.Text.color = highlighted ? _helpBookChapterJumpButtonHighlightColor : jump.DefaultColor;
            }
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
            _helpBookChapterJumpButton0Highlighted = false;
            ShowHelpBookPage(_helpBookPageIndex + 1);
        }

        private void PreviousHelpBookPage()
        {
            _helpBookChapterJumpButton0Highlighted = false;
            ShowHelpBookPage(_helpBookPageIndex - 1);
        }

        private void ShowFase5Empty(int index)
        {
            if (_fase5Cardapios == null || _fase5Cardapios.Count == 0)
                return;

            for (int i = 0; i < _fase5Cardapios.Count; i++)
            {
                if (_fase5Cardapios[i] != null)
                    _fase5Cardapios[i].SetActive(i == index);
            }

            if (_fase5Cardapios[index] != null)
            {
                ScrollViewUtils.ResetContentPositionY(_fase5Cardapios[index].transform);
                ScrollViewUtils.ResetAncestorScrollView(_fase5Cardapios[index].transform);
            }

            _fase5EmptyIndex = index;
        }

        private void NextFase5Empty()
        {
            if (_fase5Cardapios == null || _fase5Cardapios.Count == 0)
                return;

            int nextIndex = _fase5EmptyIndex + 1;
            if (nextIndex >= _fase5Cardapios.Count)
                nextIndex = 0;

            ShowFase5Empty(nextIndex);
        }

        private void PreviousFase5Empty()
        {
            if (_fase5Cardapios == null || _fase5Cardapios.Count == 0)
                return;

            int previousIndex = _fase5EmptyIndex - 1;
            if (previousIndex < 0)
                previousIndex = _fase5Cardapios.Count - 1;

            ShowFase5Empty(previousIndex);
        }

        private void ActivatePhaseEmpty(int levelIndex)
        {
            if (_phase == null)
                return;

            DeactivateAllPhaseEmpties();

            if (levelIndex < 0 || levelIndex >= _phase.Count || _phase[levelIndex] == null)
                return;

            _phase[levelIndex].SetActive(true);

            ScrollViewUtils.ResetContentPositionY(_phase[levelIndex].transform);
            ScrollViewUtils.ResetContentPositionY(_pantry);

            ShowFase5Empty(0);

            UpdateGameCloseButtonInteractable();
            UpdateHelpBookButtonInteractable(levelIndex);
        }

        private void UpdateHelpBookButtonInteractable(int levelIndex)
        {
            if (_helpBookButton == null)
                return;

            bool isFase5 = levelIndex == _phase.Count - 1;
            _helpBookButton.interactable = !isFase5;
        }

        private void UpdateGameCloseButtonInteractable()
        {
            if (_GameCloseButton == null)
                return;

            _GameCloseButton.interactable = AllDropZonesFilled();
        }

        private bool AllDropZonesFilled()
        {
            if (_phase == null || _selectedLevelIndex < 0 || _selectedLevelIndex >= _phase.Count)
                return false;

            GameObject currentPhaseEmpty = _phase[_selectedLevelIndex];
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
            if (_phase == null)
                return;

            foreach (GameObject phaseEmpty in _phase)
            {
                if (phaseEmpty != null)
                    phaseEmpty.SetActive(false);
            }
        }

        /// <summary>
        /// Limpa todos os drops de uma fase (inclusive os 3 cardápios da Fase 5, que ficam
        /// aninhados dentro do mesmo Empty). Usado ao sair da fase sem finalizá-la — os
        /// FoodDropZone não se limpam sozinhos ao desativar, então sem isso uma nova tentativa
        /// dessa fase começaria com o que foi deixado da vez anterior.
        /// </summary>
        private void ClearPhaseDropZones(int levelIndex)
        {
            if (_phase == null || levelIndex < 0 || levelIndex >= _phase.Count || _phase[levelIndex] == null)
                return;

            FoodDropZone[] dropZones = _phase[levelIndex].GetComponentsInChildren<FoodDropZone>(true);
            foreach (FoodDropZone dropZone in dropZones)
                dropZone.Limpar();
        }

        private void CopyPhaseToMenuMade()
        {
            if (_menuMade == null || _phase == null)
                return;

            ClearMenuMade();

            if (_selectedLevelIndex < 0 || _selectedLevelIndex >= _phase.Count || _phase[_selectedLevelIndex] == null)
                return;

            GameObject originalPhase = _phase[_selectedLevelIndex];

            _menuMadeCopy = Instantiate(originalPhase, _menuMade.transform);
            _menuMadeCopy.SetActive(true);

            bool isFase5 = _selectedLevelIndex == _phase.Count - 1;

            if (isFase5)
                CopyFase5NavigationButtons(originalPhase);
            else
                CopyDropZonesData(originalPhase, _menuMadeCopy);
        }

        private void ClearMenuMade()
        {
            if (_menuMade == null)
                return;

            for (int i = _menuMade.transform.childCount - 1; i >= 0; i--)
                Destroy(_menuMade.transform.GetChild(i).gameObject);

            _menuMadeCopy = null;
            _menuMadeFase5CardapiosCopy = null;
        }

        private static void CopyDropZonesData(GameObject original, GameObject clone)
        {
            if (original == null || clone == null)
                return;

            FoodDropZone[] originalDropZones = original.GetComponentsInChildren<FoodDropZone>(true);
            FoodDropZone[] cloneDropZones = clone.GetComponentsInChildren<FoodDropZone>(true);

            for (int i = 0; i < originalDropZones.Length && i < cloneDropZones.Length; i++)
                cloneDropZones[i].CopyFrom(originalDropZones[i]);
        }

        private void CopyFase5NavigationButtons(GameObject originalPhase)
        {
            if (_menuMadeCopy == null || _fase5Cardapios == null)
                return;

            _menuMadeFase5CardapiosCopy = new List<GameObject>();
            foreach (GameObject cardapio in _fase5Cardapios)
            {
                GameObject cardapioCopy = cardapio != null
                    ? FindCloneEquivalent(originalPhase.transform, cardapio.transform, _menuMadeCopy.transform)
                    : null;

                // Copia os drops de cada cardápio (café/almoço/jantar) individualmente,
                // usando o índice local de cada um (em vez de uma lista "achatada" da fase inteira),
                // para garantir que os alimentos de TODOS os empties sejam levados para o cardápio feito.
                if (cardapio != null && cardapioCopy != null)
                    CopyDropZonesData(cardapio, cardapioCopy);

                _menuMadeFase5CardapiosCopy.Add(cardapioCopy);
            }

            Button previousCopy = CloneNavigationButton(_fase5PreviousButton, originalPhase.transform, _menuMadeCopy.transform);
            Button nextCopy = CloneNavigationButton(_fase5NextButton, originalPhase.transform, _menuMadeCopy.transform);

            if (previousCopy != null)
                previousCopy.onClick.AddListener(PreviousMenuMadeFase5Empty);
            if (nextCopy != null)
                nextCopy.onClick.AddListener(NextMenuMadeFase5Empty);

            ShowMenuMadeFase5Empty(_fase5EmptyIndex);
        }

        private Button CloneNavigationButton(Button original, Transform originalPhaseRoot, Transform cloneParent)
        {
            if (original == null)
                return null;

            GameObject clone = original.transform.IsChildOf(originalPhaseRoot)
                ? FindCloneEquivalent(originalPhaseRoot, original.transform, cloneParent)
                : Instantiate(original.gameObject, cloneParent);

            if (clone == null)
                return null;

            Button button = clone.GetComponent<Button>();
            if (button != null)
                button.onClick.RemoveAllListeners();

            return button;
        }

        private static GameObject FindCloneEquivalent(Transform originalRoot, Transform originalTarget, Transform cloneRoot)
        {
            if (originalTarget == originalRoot)
                return cloneRoot.gameObject;

            string relativePath = GetRelativePath(originalRoot, originalTarget);
            if (relativePath == null)
                return null;

            Transform found = cloneRoot.Find(relativePath);
            return found?.gameObject;
        }

        private static string GetRelativePath(Transform root, Transform target)
        {
            List<string> names = new List<string>();
            Transform current = target;

            while (current != null && current != root)
            {
                names.Add(current.name);
                current = current.parent;
            }

            if (current != root)
                return null;

            names.Reverse();
            return string.Join("/", names);
        }

        private void ShowMenuMadeFase5Empty(int index)
        {
            if (_menuMadeFase5CardapiosCopy == null || _menuMadeFase5CardapiosCopy.Count == 0)
                return;

            index = Mathf.Clamp(index, 0, _menuMadeFase5CardapiosCopy.Count - 1);

            for (int i = 0; i < _menuMadeFase5CardapiosCopy.Count; i++)
            {
                if (_menuMadeFase5CardapiosCopy[i] != null)
                    _menuMadeFase5CardapiosCopy[i].SetActive(i == index);
            }

            _menuMadeFase5Index = index;
        }

        private void NextMenuMadeFase5Empty()
        {
            if (_menuMadeFase5CardapiosCopy == null || _menuMadeFase5CardapiosCopy.Count == 0)
                return;

            int nextIndex = _menuMadeFase5Index + 1;
            if (nextIndex >= _menuMadeFase5CardapiosCopy.Count)
                nextIndex = 0;

            ShowMenuMadeFase5Empty(nextIndex);
        }

        private void PreviousMenuMadeFase5Empty()
        {
            if (_menuMadeFase5CardapiosCopy == null || _menuMadeFase5CardapiosCopy.Count == 0)
                return;

            int previousIndex = _menuMadeFase5Index - 1;
            if (previousIndex < 0)
                previousIndex = _menuMadeFase5CardapiosCopy.Count - 1;

            ShowMenuMadeFase5Empty(previousIndex);
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
            UpdateLevelSelectedCompletionImage(levelIndex);

            FadeOutPaperLevelSelected(levelIndex);
            FadeIn(_levelSelectedConfig.Empty, _fadeDuration);

            ScrollViewUtils.ResetContentPositionY(_levelSelectedConfig.Empty.transform);
        }

        private void UpdateLevelSelectedCompletionImage(int levelIndex)
        {
            if (_levelSelectedConfig == null || _levelSelectedConfig.ImagesList == null || _levelSelectedConfig.ImagesList.Count < 2)
                return;

            Image completionImage = _levelSelectedConfig.ImagesList[1];
            if (completionImage != null)
                completionImage.gameObject.SetActive(IsPhaseCompleted(levelIndex));
        }

        private void FadeOutPaperLevelSelected(int levelIndex)
        {
            if (_paperLevelSelected == null || levelIndex < 0 || levelIndex >= _paperLevelSelected.Count)
                return;

            Image paper = _paperLevelSelected[levelIndex];
            if (paper != null)
                FadeOut(paper.gameObject, _fadeDuration);
        }

        private void FadeInPaperLevelSelected(int levelIndex)
        {
            if (_paperLevelSelected == null || levelIndex < 0 || levelIndex >= _paperLevelSelected.Count)
                return;

            Image paper = _paperLevelSelected[levelIndex];
            if (paper != null)
                FadeIn(paper.gameObject, _fadeDuration);
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

            FadeOut(_levelSelectedConfig.Empty, _fadeDuration);
            FadeInPaperLevelSelected(_selectedLevelIndex);
        }

        private void SetupMenuEvents()
        {
            //MainMenu
            if (_mainMenu == null || _mainMenu.ButtonsList == null)
                return;
            if (_mainMenu.ButtonsList.Count > 0)
                _mainMenu.ButtonsList[0].onClick.AddListener(OnClickPlayNewGame);
            if (_mainMenu.ButtonsList.Count > 1)
                _mainMenu.ButtonsList[1].onClick.AddListener(ConfirmQuitGame);
            if (_mainMenu.ButtonsList.Count > 2)
                _mainMenu.ButtonsList[2].onClick.AddListener(() => _creditsMenu.Open());
            if (_mainMenu.ButtonsList.Count > 3)
                _mainMenu.ButtonsList[3].onClick.AddListener(OnClickContinueSavedGame);

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
                for (int i = 0; i < _helpBookChapterJumpButtons.Count; i++)
                {
                    PageJumpButton jump = _helpBookChapterJumpButtons[i];
                    if (jump == null || jump.Button == null)
                        continue;

                    jump.Text = jump.Button.GetComponentInChildren<TMP_Text>();
                    if (jump.Text != null)
                        jump.DefaultColor = jump.Text.color;

                    int pageIndex = jump.PageIndex;
                    bool isFirstButton = i == 0;
                    jump.Button.onClick.AddListener(() =>
                    {
                        _helpBookChapterJumpButton0Highlighted = isFirstButton;
                        ShowHelpBookPage(pageIndex);
                    });
                }
            }

            if (_helpBookAlphabetJumpButtons != null)
            {
                foreach (PageJumpButton jump in _helpBookAlphabetJumpButtons)
                {
                    if (jump == null || jump.Button == null)
                        continue;

                    int pageIndex = jump.PageIndex;
                    jump.Button.onClick.AddListener(() =>
                    {
                        _helpBookChapterJumpButton0Highlighted = false;
                        ShowHelpBookPage(pageIndex);
                    });
                }
            }

            //Fase 5 - Troca de Empty
            if (_fase5PreviousButton != null)
                _fase5PreviousButton.onClick.AddListener(PreviousFase5Empty);
            if (_fase5NextButton != null)
                _fase5NextButton.onClick.AddListener(NextFase5Empty);

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
