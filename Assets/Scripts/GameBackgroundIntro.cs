using System.Collections;
using UnityEngine;

public class GameBackgroundIntro : MonoBehaviour
{
    [Header("CanvasGroups de las portadas")]
    [SerializeField] private CanvasGroup presentacionStretch;
    [SerializeField] private CanvasGroup presentacionCenter;

    [Header("Tiempos")]
    [SerializeField] private float delayBeforeShow = 0.5f;
    [SerializeField] private float fadeDuration = 0.75f;

    private void Start()
    {
        // Aseguramos que empiezan invisibles
        if (presentacionStretch != null) presentacionStretch.alpha = 0f;
        if (presentacionCenter != null) presentacionCenter.alpha = 0f;

        StartCoroutine(FadeInCovers());
    }

    private IEnumerator FadeInCovers()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);

            if (presentacionStretch != null) presentacionStretch.alpha = a;
            if (presentacionCenter != null) presentacionCenter.alpha = a;

            yield return null;
        }
    }
}
