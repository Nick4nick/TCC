using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerencia música de fundo (playlist que avança sozinha) e efeitos sonoros (clique de botão,
/// alimento solto no drop, feedback com nota boa/ruim), cada um com seu próprio volume.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sfx
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
    }

    [Header("Música de fundo")]
    [Tooltip("Playlist de músicas de fundo. Quando uma termina, a próxima da lista começa sozinha (ao chegar no fim, volta para a primeira).")]
    [SerializeField] private List<AudioClip> _musicPlaylist;
    [Tooltip("Deixe vazio para criar um AudioSource automaticamente")]
    [SerializeField] private AudioSource _musicSource;
    [Range(0f, 1f)]
    [SerializeField] private float _musicVolume = 1f;

    [Header("SFX")]
    [Tooltip("Deixe vazio para criar um AudioSource automaticamente")]
    [SerializeField] private AudioSource _sfxSource;
    [Tooltip("Som ao clicar em qualquer botão do jogo")]
    [SerializeField] private Sfx _buttonClickSfx;
    [Tooltip("Som ao soltar um alimento em um drop")]
    [SerializeField] private Sfx _dropSfx;
    [Tooltip("Som quando a nota final do feedback é >= 7,5")]
    [SerializeField] private Sfx _feedbackNotaAltaSfx;
    [Tooltip("Som quando a nota final do feedback é < 7,5")]
    [SerializeField] private Sfx _feedbackNotaBaixaSfx;

    private const float NotaMinimaAltaSfx = 7f;

    private int _musicIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_musicSource == null)
            _musicSource = gameObject.AddComponent<AudioSource>();
        if (_sfxSource == null)
            _sfxSource = gameObject.AddComponent<AudioSource>();

        _musicSource.loop = false;
        _musicSource.playOnAwake = false;
        _musicSource.volume = _musicVolume;

        _sfxSource.playOnAwake = false;

        HookAllButtons();
    }

    private void Start()
    {
        PlayNextMusic();
    }

    private void Update()
    {
        if (_musicPlaylist == null || _musicPlaylist.Count == 0)
            return;

        if (!_musicSource.isPlaying)
            PlayNextMusic();
    }

    private void PlayNextMusic()
    {
        if (_musicPlaylist == null || _musicPlaylist.Count == 0)
            return;

        _musicIndex++;
        if (_musicIndex >= _musicPlaylist.Count)
            _musicIndex = 0;

        AudioClip clip = _musicPlaylist[_musicIndex];
        if (clip == null)
            return;

        _musicSource.clip = clip;
        _musicSource.Play();
    }

    /// <summary>
    /// Liga o som de clique em todo Button já existente na cena no momento do Awake.
    /// Botões criados depois (ex: clones instanciados em runtime) precisam chamar
    /// PlayButtonClickSfx() manualmente ou ser re-registrados.
    /// </summary>
    private void HookAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
            button.onClick.AddListener(PlayButtonClickSfx);
    }

    public void PlayButtonClickSfx() => PlaySfx(_buttonClickSfx);

    public void PlayDropSfx() => PlaySfx(_dropSfx);

    public void PlayFeedbackSfx(float notaFinal)
    {
        PlaySfx(notaFinal >= NotaMinimaAltaSfx ? _feedbackNotaAltaSfx : _feedbackNotaBaixaSfx);
    }

    private void PlaySfx(Sfx sfx)
    {
        if (sfx == null || sfx.Clip == null || _sfxSource == null)
            return;

        _sfxSource.PlayOneShot(sfx.Clip, sfx.Volume);
    }
}
