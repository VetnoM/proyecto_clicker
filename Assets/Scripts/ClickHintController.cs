using System.Collections;
using UnityEngine;

public class ClickHintController : MonoBehaviour
{
    [SerializeField] private GameObject clickHint;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.35f;
    [SerializeField] private string prefKey = "hint_click_done";

    [Header("Dev")]
    [SerializeField] private bool alwaysShowInEditor = true; // ✅ para no borrar PlayerPrefs probando

    bool fading = false;

    void Start()
    {
        if (clickHint == null) return;
        if (canvasGroup == null) canvasGroup = clickHint.GetComponent<CanvasGroup>();

        bool done = PlayerPrefs.GetInt(prefKey, 0) == 1;

#if UNITY_EDITOR
        if (alwaysShowInEditor) done = false;
#endif

        clickHint.SetActive(!done);
        if (!done && canvasGroup != null) canvasGroup.alpha = 1f;
    }

    void Update()
    {
        if (fading) return;
        if (clickHint == null || !clickHint.activeSelf) return;
        if (GameManager.I == null) return;

        if (GameManager.I.totalClicks >= 1)
        {
            fading = true;
            StartCoroutine(FadeOutAndHide());
        }
    }

    IEnumerator FadeOutAndHide()
    {
        if (canvasGroup == null) canvasGroup = clickHint.GetComponent<CanvasGroup>();

        float t = 0f;
        float start = (canvasGroup != null) ? canvasGroup.alpha : 1f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.001f, fadeTime);
            float a = Mathf.Lerp(start, 0f, t);
            if (canvasGroup != null) canvasGroup.alpha = a;
            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 0f;
        clickHint.SetActive(false);

#if UNITY_EDITOR
        if (alwaysShowInEditor) yield break; // ✅ en editor no guardamos "done"
#endif

        PlayerPrefs.SetInt(prefKey, 1);
        PlayerPrefs.Save();
    }
}
