using System.Collections;
using TMPro;
using UnityEngine;

public class UnlockPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private float fadeIn = 0.15f;
    [SerializeField] private float hold = 1.0f;
    [SerializeField] private float fadeOut = 0.35f;
 [Header("Solo una vez")]
[SerializeField] private string onceKey = "popup_advanced_unlocked";


    Coroutine routine;

    void Awake()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        if (txt == null) txt = GetComponent<TextMeshProUGUI>();

        if (cg != null) cg.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (txt != null) txt.text = message;

        gameObject.SetActive(true);

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        if (cg == null) yield break;

        cg.alpha = 0f;

        // Fade in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.001f, fadeIn);
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        cg.alpha = 1f;

        // Hold
        float h = 0f;
        while (h < hold)
        {
            h += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.001f, fadeOut);
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        cg.alpha = 0f;

        gameObject.SetActive(false);
        routine = null;
    }

public void ShowOnce(string message)
{
    if (PlayerPrefs.GetInt(onceKey, 0) == 1) return;
    PlayerPrefs.SetInt(onceKey, 1);
    PlayerPrefs.Save();
    Show(message);
}
}
