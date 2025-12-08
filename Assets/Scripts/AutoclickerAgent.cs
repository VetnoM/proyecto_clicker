using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AutoclickerAgent : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform canvasRoot;      // Debe ser el mismo Canvas que el botón
    public RectTransform targetButton;    // RectTransform del botón central
    public ClickPulse clickPulse;         // Opcional: pulso visual del botón
    public Image icon;                    // Image del ratón (si no, se encuentra en Awake)

    [Header("Órbita")]
    public float radius = 180f;           // radio de la órbita (px)
    public float angularSpeed = 90f;      // grados/segundo
    public float angularOffsetDeg = 0f;   // desfase angular inicial

    [Header("Ritmo fijo por agente")]
    public float baseCPS = 0.2f;          // clicks por segundo base del agente
    public int speedLevel = 0;          // nivel individual de velocidad (lo sube el Manager)
    public float perLevelBonus = 0.15f;   // +15% de CPS por nivel
    public float minInterval = 0.12f;     // límite inferior del intervalo (seg)
    public float CurrentMultiplier => 1f + speedLevel * perLevelBonus;

    [Header("Dash")]
    public float dashTime = 0.25f;        // tiempo de ida
    public float holdTime = 0.05f;        // pausa encima del botón
    public float returnTime = 0.25f;      // tiempo de vuelta
    public float overshoot = 1.03f;       // sobrepaso sutil para que se sienta dinámico

    [Header("Colores / Feedback")]
    public Color baseColor = Color.white;              // color del nivel 0
    public Color flashColor = new Color(1f, 0.9f, 0.2f); // dorado suave al mejorar
    public float flashTime = 0.2f;                     // duración de cada tramo del flash
    public int maxVisualLevel = 10;                    // para mapear niveles a color
    public Gradient levelGradient;                     // opcional: arrástralo en el Inspector

    [Header("FX AutoClicker")]
    public bool showFx = true;
    public float fxCooldown = 0.15f;   // mínimo tiempo entre FX de ESTE agente (≈ 6-7 por segundo)
    float lastFxTime;

    // Estado interno
    RectTransform rt;
    float angle;
    bool isDashing;
    Coroutine flashCo;

    void Awake()
    {
        rt = transform as RectTransform;
        if (!icon) icon = GetComponent<Image>();
        if (!icon) icon = GetComponentInChildren<Image>(true); // ← por si el Image está en un hijo
        angle = angularOffsetDeg;
        if (icon) icon.color = GetLevelColor();
    }


    void Start()
    {

        SetPositionOnOrbit();
        StartCoroutine(ClickLoop());
    }

    void Update()
    {
        // Mientras hace dash, no actualizamos la órbita
        if (isDashing) return;

        angle += angularSpeed * Time.unscaledDeltaTime;
        if (angle > 360f) angle -= 360f;

        SetPositionOnOrbit();

        // (Opcional) orientar el icono mirando al centro
        Vector2 dir = (targetButton.anchoredPosition - rt.anchoredPosition).normalized;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rt.rotation = Quaternion.Euler(0, 0, z);
    }

    void SetPositionOnOrbit()
    {
        Vector2 center = targetButton.anchoredPosition;
        float rad = angle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        rt.anchoredPosition = center + offset;      // mover SIEMPRE en espacio UI
    }

    IEnumerator ClickLoop()
    {
        while (true)
        {
            // CPS del agente (fijo por nivel). Convertimos a intervalo (s por click)
            float agentCPS = Mathf.Max(0.0001f, baseCPS * CurrentMultiplier);
            float wait = Mathf.Max(minInterval, 1f / agentCPS) + Random.Range(-0.08f, 0.08f);

            yield return new WaitForSecondsRealtime(wait);
            yield return DashAndClickOnce();
        }
    }

    IEnumerator DashAndClickOnce()
    {
        isDashing = true;

        Vector2 from = rt.anchoredPosition;
        Vector2 to = targetButton.anchoredPosition;

        // Ida (ease out)
        float t = 0f;
        while (t < dashTime)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / dashTime), 3f);
            rt.anchoredPosition = Vector2.LerpUnclamped(from, to, k * overshoot);
            yield return null;
        }
        rt.anchoredPosition = to;

        // “Click” real + pulso visual
        if (clickPulse) clickPulse.Play();
        if (GameManager.I != null) GameManager.I.OnClick();

        // --- FX de "+X" para el agente (justo después del click) ---
        if (showFx && ClickFXManager.I != null && GameManager.I != null)
        {
            float now = Time.unscaledTime;
            if (now - lastFxTime >= fxCooldown)
            {
                lastFxTime = now;

                Vector2 uiPos;
                if (targetButton != null)
                {
                    uiPos = targetButton.anchoredPosition;
                }
                else
                {
                    var rt = transform as RectTransform;
                    uiPos = rt != null ? rt.anchoredPosition : Vector2.zero;
                }

                ClickFXManager.I.PlayClickFXAt(
                    uiPos,
                    $"+{GameManager.I.coinsPerClick:0}",
                    10 // menos partículas que el click manual
                );
            }
        }

        // Pausa breve encima del botón
        yield return new WaitForSecondsRealtime(holdTime);

        // Vuelta (ease in)
        t = 0f;
        while (t < returnTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Pow(Mathf.Clamp01(t / returnTime), 3f);
            rt.anchoredPosition = Vector2.LerpUnclamped(to, from, k);
            yield return null;
        }
        rt.anchoredPosition = from;

        isDashing = false;
    }


    // === Colores / Feedback ===
    public void ApplyLevelColor()
    {
        if (icon) icon.color = GetLevelColor();
    }

    public void PlayUpgradeFX()
    {
        if (!icon) return;
        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Color from = icon.color;
        Color toFlash = Color.Lerp(from, flashColor, 0.85f);

        float t = 0f;
        while (t < flashTime)
        {
            t += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(from, toFlash, t / flashTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.05f);

        Color back = GetLevelColor();
        t = 0f;
        while (t < flashTime)
        {
            t += Time.unscaledDeltaTime;
            icon.color = Color.Lerp(toFlash, back, t / flashTime);
            yield return null;
        }
        icon.color = back;
        flashCo = null;
    }

    Color GetLevelColor()
    {
        // Si hay Gradient, úsalo
        if (levelGradient != null && levelGradient.colorKeys != null && levelGradient.colorKeys.Length > 0)
        {
            float k = Mathf.Clamp01((float)speedLevel / Mathf.Max(1, maxVisualLevel));
            return levelGradient.Evaluate(k);
        }
        // Fallback si baseColor es blanco/gris (saturación baja)
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        if (s < 0.05f) { h = 0.58f; s = 0.85f; v = 1f; } // parte azul saturado
        float shift = 0.08f * speedLevel;
        return Color.HSVToRGB(Mathf.Repeat(h + shift, 1f), s, v);
    }


}
