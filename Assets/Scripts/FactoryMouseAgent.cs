using TMPro;
using UnityEngine;

public class FactoryMouseAgent : MonoBehaviour
{
    public RectTransform effectsRoot;          // Canvas/EffectsRoot (de escena)
    public TextMeshProUGUI floatingTextPrefab; // EL MISMO prefab TMP que usa ClickFXManager
    public float intervalSeconds = 1f;

    double amountPerTick = 1;
    float timer;
    RectTransform mouseRT;
    Canvas canvas;

    void Awake()
    {
        mouseRT = transform as RectTransform;
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetAmountPerTick(double v) => amountPerTick = v;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < intervalSeconds) return;
        timer = 0f;

        if (!effectsRoot || !floatingTextPrefab || mouseRT == null) return;

        // Convertir posición del mouse a anchoredPosition dentro de effectsRoot
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, mouseRT.position);

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(effectsRoot, screen, cam, out local);

        var ft = Instantiate(floatingTextPrefab, effectsRoot);
        ((RectTransform)ft.transform).anchoredPosition = local;

        var fts = ft.GetComponent<FloatingText>() ?? ft.gameObject.AddComponent<FloatingText>();
        fts.label = ft;
        fts.Play($"+{amountPerTick:0}");
    }
}
