using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI label;
    public float riseDistance = 60f;
    public float life = 0.8f;
    public Vector2 randomJitter = new Vector2(10f, 4f);

    RectTransform rt;
    Color baseColor;

    void Awake()
    {
        rt = transform as RectTransform;
        if (!label) label = GetComponent<TextMeshProUGUI>();
        if (label) baseColor = label.color;
    }

    // ✅ Método original (para no romper ClickFXManager)
    public void Play(string text)
    {
        if (label)
        {
            label.text = text;
            // no cambiamos color, usamos el que ya tenga el TMP
            baseColor = label.color;
        }
        StopAllCoroutines();
        StartCoroutine(CoPlay());
    }

    // ✅ Nuevo método con color (para críticos)
    public void Play(string text, Color color)
    {
        if (label)
        {
            label.text = text;
            label.color = color;
            baseColor = color; // importante para el fade
        }
        StopAllCoroutines();
        StartCoroutine(CoPlay());
    }

    IEnumerator CoPlay()
    {
        Vector2 start = rt.anchoredPosition +
                        new Vector2(Random.Range(-randomJitter.x, randomJitter.x),
                                    Random.Range(-randomJitter.y, randomJitter.y));
        Vector2 end = start + new Vector2(0, riseDistance);
        float t = 0f;

        while (t < life)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / life);

            rt.anchoredPosition = Vector2.LerpUnclamped(start, end, 1f - Mathf.Pow(1f - k, 2f));

            if (label)
            {
                var c = baseColor;
                c.a = 1f - k;
                label.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
