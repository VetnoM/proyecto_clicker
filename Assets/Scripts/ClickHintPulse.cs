using UnityEngine;

public class ClickHintPulse : MonoBehaviour
{
    [Header("Elementos a animar")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private RectTransform clickText;

    [Header("Palpitación")]
    [SerializeField] private float speed = 2.2f;          // velocidad
    [SerializeField] private float arrowScaleAmp = 0.08f;  // 8%
    [SerializeField] private float textScaleAmp = 0.12f;   // 12%

    [Header("Extra (opcional)")]
    [SerializeField] private float arrowRotateAmp = 6f;    // grados
    [SerializeField] private float bobAmp = 10f;           // px arriba/abajo

    private Vector3 arrowBaseScale;
    private Vector3 textBaseScale;
    private Vector2 arrowBasePos;
    private Vector2 textBasePos;

    void Awake()
    {
        if (arrow != null)
        {
            arrowBaseScale = arrow.localScale;
            arrowBasePos = arrow.anchoredPosition;
        }
        if (clickText != null)
        {
            textBaseScale = clickText.localScale;
            textBasePos = clickText.anchoredPosition;
        }
    }

    void OnDisable()
    {
        // reset por si se desactiva con el fade
        if (arrow != null)
        {
            arrow.localScale = arrowBaseScale;
            arrow.anchoredPosition = arrowBasePos;
            arrow.localRotation = Quaternion.identity;
        }
        if (clickText != null)
        {
            clickText.localScale = textBaseScale;
            clickText.anchoredPosition = textBasePos;
        }
    }

    void Update()
    {
        float t = Time.unscaledTime * speed;
        float s = (Mathf.Sin(t) + 1f) * 0.5f; // 0..1

        if (arrow != null)
        {
            float k = 1f + Mathf.Lerp(-arrowScaleAmp, arrowScaleAmp, s);
            arrow.localScale = arrowBaseScale * k;

            float bob = Mathf.Sin(t) * bobAmp;
            arrow.anchoredPosition = arrowBasePos + new Vector2(0f, bob);

            float rot = Mathf.Sin(t) * arrowRotateAmp;
            arrow.localRotation = Quaternion.Euler(0, 0, rot);
        }

        if (clickText != null)
        {
            float k = 1f + Mathf.Lerp(-textScaleAmp, textScaleAmp, s);
            clickText.localScale = textBaseScale * k;

            float bob = Mathf.Sin(t + 1.2f) * (bobAmp * 0.6f);
            clickText.anchoredPosition = textBasePos + new Vector2(0f, bob);
        }
    }
}
