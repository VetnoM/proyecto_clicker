using UnityEngine;
using TMPro;

public class ClickFXManager : MonoBehaviour
{
    public static ClickFXManager I;

    [Header("Refs UI")]
    public RectTransform canvasRoot;           // Canvas (RectTransform)
    public RectTransform clickButton;          // RectTransform del botón central

    [Header("Prefabs")]
    public TextMeshProUGUI floatingTextPrefab; // TMP para el "+X"

    [Header("Burst UI (pooled)")]
    public UIBurstPooled uiBurst;              // ← arrastra el componente UIBurstPooled

    [Header("Audio (opcional)")]
    public AudioSource audioSource;
    public AudioClip clickSfx;

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!canvasRoot)
        {
#if UNITY_2023_1_OR_NEWER
            var c = Object.FindFirstObjectByType<Canvas>();
#else
            var c = Object.FindObjectOfType<Canvas>();
#endif
            if (c) canvasRoot = c.transform as RectTransform;
        }
    }

    /// <summary>
    /// Efecto centrado en el botón (si no pasas posición).
    /// </summary>
    public void PlayClickFX(string text = null, int countOverride = -1)
    {
        if (floatingTextPrefab && canvasRoot && clickButton)
        {
            var ft = Instantiate(floatingTextPrefab, canvasRoot);
            var rt = (RectTransform)ft.transform;
            rt.anchoredPosition = clickButton.anchoredPosition;
            var fts = ft.GetComponent<FloatingText>() ?? ft.gameObject.AddComponent<FloatingText>();
            fts.label = ft;
string finalText = text;
if (string.IsNullOrEmpty(finalText))
    finalText = (GameManager.I != null) ? $"+{GameManager.I.coinsPerClick:0}" : "+1";

fts.Play(finalText, Color.white);
       }

        uiBurst?.PlayAt(clickButton.anchoredPosition, countOverride);

        if (audioSource && clickSfx) audioSource.PlayOneShot(clickSfx, 0.85f);
        Debug.Log("[FX] PlayClickFX (centro botón)");

    }

    /// <summary>
    /// Efecto en una POSICIÓN UI concreta (anchoredPosition en el Canvas).
    /// Úsalo para el clic del mouse o la posición del agente.
    /// </summary>
    public void PlayClickFXAt(Vector2 uiPos, string text = null, int countOverride = -1)
    {
        if (floatingTextPrefab && canvasRoot)
        {
            var ft = Instantiate(floatingTextPrefab, canvasRoot);
            var rt = (RectTransform)ft.transform;
            rt.anchoredPosition = uiPos;
            var fts = ft.GetComponent<FloatingText>() ?? ft.gameObject.AddComponent<FloatingText>();
            fts.label = ft;
           string finalText = text;
if (string.IsNullOrEmpty(finalText))
    finalText = (GameManager.I != null) ? $"+{GameManager.I.coinsPerClick:0}" : "+1";

fts.Play(finalText);

        }

        uiBurst?.PlayAt(uiPos, countOverride);

        if (audioSource && clickSfx) audioSource.PlayOneShot(clickSfx, 0.85f);
        Debug.Log("[FX] PlayClickFXAt (pos UI)");

    }

    public void PlayClickFXColored(string text, Color color, int countOverride = -1)
{
    if (floatingTextPrefab && canvasRoot && clickButton)
    {
        var ft = Instantiate(floatingTextPrefab, canvasRoot);
        var rt = (RectTransform)ft.transform;
        rt.anchoredPosition = clickButton.anchoredPosition;

        var fts = ft.GetComponent<FloatingText>() ?? ft.gameObject.AddComponent<FloatingText>();
        fts.label = ft;
        fts.Play(text, color);
    }

    uiBurst?.PlayAt(clickButton.anchoredPosition, countOverride);

    if (audioSource && clickSfx) audioSource.PlayOneShot(clickSfx, 0.85f);
}

}
