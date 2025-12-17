using UnityEngine;

public class ClickPulse : MonoBehaviour
{
    public RectTransform target;

    [Header("Ajustes")]
    public float pressScale = 0.9f;    // escala al hacer click
    public float relaxSpeed = 20f;     // rapidez para volver a la normal

    Vector3 baseScale;

    void Awake()
    {
        if (!target) target = transform as RectTransform;
        baseScale = target.localScale;
    }

    public void Play()
    {
        // Al hacer click, simplemente dejamos el botón un poco "hundido"
        target.localScale = baseScale * pressScale;
    }

    void Update()
    {
        // Cada frame, volvemos hacia la escala original
        target.localScale = Vector3.Lerp(
            target.localScale,
            baseScale,
            relaxSpeed * Time.unscaledDeltaTime
        );
    }
}
