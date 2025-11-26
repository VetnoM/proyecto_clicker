using System.Collections;
using UnityEngine;

public class ClickPulse : MonoBehaviour
{
    [Header("Objetivo (si se deja vacío usa este mismo)")]
    public RectTransform target;

    [Header("Ajustes")]
    public float pressScale = 0.9f;     // escala al presionar
    public float reboundScale = 1.1f;   // rebote
    public float downTime = 0.10f;      // tiempo al bajar
    public float upTime = 0.18f;        // tiempo al volver

    Vector3 baseScale;
    Coroutine anim;
    float progress = 0f; // 0..1

    void Awake()
    {
        if (!target) target = transform as RectTransform;
        baseScale = target.localScale;
    }

    public void Play()
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(CoPulse());
    }

    IEnumerator CoPulse()
    {
        // bajar hasta pressScale
        Vector3 from = target.localScale;
        Vector3 to = baseScale * pressScale;
        progress = 0f;

        while (progress < 1f)
        {
            progress += Time.unscaledDeltaTime / downTime;
            float k = 1f - (1f - Mathf.Clamp01(progress)) * (1f - Mathf.Clamp01(progress)); // easeOutQuad
            target.localScale = Vector3.LerpUnclamped(from, to, k);
            yield return null;
        }

        // subir con rebote y volver a base
        Vector3 peak = baseScale * reboundScale;

        // subir al pico
        from = to;
        float t = 0f;
        while (t < upTime * 0.55f)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - (1f - Mathf.Clamp01(t / (upTime * 0.55f))) * (1f - Mathf.Clamp01(t / (upTime * 0.55f)));
            target.localScale = Vector3.LerpUnclamped(from, peak, k);
            yield return null;
        }

        // bajar a base
        from = target.localScale;
        t = 0f;
        while (t < upTime * 0.45f)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / (upTime * 0.45f));
            k = k * k; // easeIn
            target.localScale = Vector3.LerpUnclamped(from, baseScale, k);
            yield return null;
        }

        target.localScale = baseScale;
        anim = null;
        progress = 0f;
    }
}
