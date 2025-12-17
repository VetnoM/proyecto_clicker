using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader I;

    [Header("Fade")]
    public float fadeDuration = 0.75f;
    public CanvasGroup canvasGroup;

    bool isFading = false;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        // Mantener todo el Canvas entre escenas
        DontDestroyOnLoad(transform.root.gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        // Empezamos en negro y hacemos fade-in de la primera escena
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        isFading = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = 1f - k;   // 1 → 0
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        // 1) Fade-out escena actual
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = k;       // 0 → 1
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // 2) Cargar escena
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return null; // un frame

        // 3) Fade-in de la nueva escena
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = 1f - k;  // 1 → 0
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
    }
}
