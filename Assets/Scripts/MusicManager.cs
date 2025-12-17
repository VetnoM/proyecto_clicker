using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I { get; private set; }

    const string VOLUME_KEY = "volume";

    [Header("AudioSources (DISTINTOS)")]
    [SerializeField] private AudioSource a;
    [SerializeField] private AudioSource b;

    [Header("Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip officeIntro;
    [SerializeField] private AudioClip gameMusic;

    [Header("Ajustes")]
    [Range(0f, 1f)] public float masterVolume = 0.6f;
    public float fadeTime = 6f;
    public float officeSeconds = 2f;

    AudioSource current;
    AudioSource next;
    Coroutine co;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Volumen global SOLO aquí (no en OptionsMenu al abrir)
        float v = PlayerPrefs.GetFloat(VOLUME_KEY, masterVolume);
        v = Mathf.Clamp01(v);
        AudioListener.volume = v;
        masterVolume = v;

        if (a == null || b == null || a == b)
        {
            Debug.LogError("[MusicManager] Asigna AudioSource A y B (distintos).");
        }

        SetupSource(a);
        SetupSource(b);

        current = a;
        next = b;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        if (I == this)
            SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void Start()
    {
        // Arranque en la escena actual
        HandleScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        HandleScene(newScene.buildIndex);
    }

    void HandleScene(int buildIndex)
    {
        if (co != null) StopCoroutine(co);

        if (buildIndex == 0) // MainMenu
        {
            if (menuMusic == null) { Debug.LogError("[MusicManager] Falta menuMusic"); return; }
            co = StartCoroutine(CrossfadeTo(menuMusic, loop: true));
        }
        else if (buildIndex == 1) // Game
        {
            if (officeIntro == null || gameMusic == null)
            {
                Debug.LogError("[MusicManager] Falta officeIntro o gameMusic");
                return;
            }
            co = StartCoroutine(GameIntro());
        }
    }

    IEnumerator GameIntro()
    {
        yield return CrossfadeTo(officeIntro, loop: true);

        float t = 0f;
        while (t < officeSeconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return CrossfadeTo(gameMusic, loop: true);
    }

    void SetupSource(AudioSource s)
    {
        if (s == null) return;
        s.playOnAwake = false;
        s.loop = false;
        s.spatialBlend = 0f; // 2D
        s.mute = false;
        s.volume = 0f; // arrancamos en 0 para fade
    }

    IEnumerator CrossfadeTo(AudioClip clip, bool loop)
    {
        if (a == null || b == null || a == b) yield break;
        if (clip == null) yield break;

        next.clip = clip;
        next.loop = loop;
        next.volume = 0f;
        next.Play();

        float dur = Mathf.Max(0.05f, fadeTime);
        float t = 0f;
        float startCur = current.isPlaying ? current.volume : 0f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;

            next.volume = Mathf.Lerp(0f, masterVolume, k);
            current.volume = Mathf.Lerp(startCur, 0f, k);

            yield return null;
        }

        next.volume = masterVolume;
        current.volume = 0f;
        current.Stop();

        var tmp = current;
        current = next;
        next = tmp;
    }

    public void SetMasterVolume(float v)
    {
        v = Mathf.Clamp01(v);
        masterVolume = v;

        // NO tocar AudioListener aquí si no quieres saltos al cambiar escenas
        // pero cuando el usuario mueve el slider sí:
        AudioListener.volume = v;

        PlayerPrefs.SetFloat(VOLUME_KEY, v);
        PlayerPrefs.Save();

        if (current != null) current.volume = masterVolume;
    }
}
