using UnityEngine;

public class HintArrowOrbit : MonoBehaviour
{
    [Header("Centro (ButtonClick)")]
    [SerializeField] private RectTransform center; // arrastra ButtonClick aquí

    [Header("Movimiento tipo “va y viene”")]
    [SerializeField] private float baseAngleDeg = 20f;
    [SerializeField] private float arcDeg = 40f;
    [SerializeField] private float swingSpeed = 4f;

    [Header("Acercarse / Alejarse")]
    [SerializeField] private float radius = 300f;
    [SerializeField] private float radiusAmp = 20f;
    [SerializeField] private float radiusSpeed = 20f;

    [Header("Toque “random” suave")]
    [SerializeField] private float noiseDeg = 50f;
    [SerializeField] private float noiseRadius = 20f;
    [SerializeField] private float noiseSpeed = 0.1f;

    [Header("Flecha apunta al mouse")]
    [SerializeField] private bool rotateToCenter = true;

    private RectTransform rt;
    private Canvas canvas;

    void Awake()
    {
        rt = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (center == null) return;

        float t = Time.unscaledTime;

        // va y viene (en arco)
        float swing = Mathf.Sin(t * swingSpeed); // -1..1
        float angle = baseAngleDeg + swing * (arcDeg * 0.5f);

        // acercarse / alejarse
        float r = radius + Mathf.Sin(t * radiusSpeed) * radiusAmp;

        // “ruido” suave
        float nA = (Mathf.PerlinNoise(t * noiseSpeed, 1.23f) - 0.5f) * 2f * noiseDeg;
        float nR = (Mathf.PerlinNoise(2.34f, t * noiseSpeed) - 0.5f) * 2f * noiseRadius;

        angle += nA;
        r += nR;

        float rad = angle * Mathf.Deg2Rad;

        // posición en mundo alrededor del centro
        Vector3 centerWorld = center.position;
        Vector3 arrowWorld = centerWorld + new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f);

        // convertir a anchoredPosition del parent del arrow
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, arrowWorld);

        RectTransform parentRT = rt.parent as RectTransform;
        if (parentRT == null) return;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screen, cam, out local);
        rt.anchoredPosition = local;

        if (rotateToCenter)
        {
            Vector2 toCenter = (Vector2)(centerWorld - rt.position);
            float rot = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
            rt.rotation = Quaternion.Euler(0, 0, rot);
        }
    }
}
