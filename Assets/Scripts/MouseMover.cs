using System.Collections;
using UnityEngine;

public class MouseMover : MonoBehaviour
{
    public RectTransform target;       // El botón central
    public float travelTime = 0.45f;    // Duración del vuelo
    public float overshoot = 1.03f;    // Pequeño “sobrepaso” para efecto
    public float fadeTime = 0.15f;     // Desvanecer al llegar

    RectTransform rt;
    CanvasGroup cg;

    void Awake()
    {
        rt = transform as RectTransform;
        cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
    }

    public void LaunchFrom(RectTransform start, RectTransform to)
    {
        if (start) rt.position = start.position; // start en pantalla
        target = to;
        StopAllCoroutines();
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        if (!target) yield break;

        Vector3 from = rt.position;
        Vector3 to = target.position;
        float t = 0f;

        // vuelo con suavizado
        while (t < travelTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / travelTime;
            // easeOutCubic
            k = 1f - Mathf.Pow(1f - k, 3f);
            rt.position = Vector3.LerpUnclamped(from, to, k * overshoot);
            yield return null;
        }

        rt.position = to;

        // pequeño fade out
        float f = 0f;
        while (f < fadeTime)
        {
            f += Time.unscaledDeltaTime;
            cg.alpha = 1f - (f / fadeTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
