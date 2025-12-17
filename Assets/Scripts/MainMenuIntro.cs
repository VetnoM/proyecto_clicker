using System.Collections;
using UnityEngine;

public class MainMenuIntro : MonoBehaviour
{
    [Header("Refs")]
    public CanvasGroup backgroundGroup; // Background (imagen "Clicker Empire")
    public CanvasGroup menuGroup;       // MenuPanel (botones)

    [Header("Background")]
    public float backgroundDelay = 2f;        // Espera antes de mostrar el fondo
    public float backgroundFadeDuration = 1.5f; // Duración fade del fondo

    [Header("Menu")]
    public float menuDelayAfterBackground = 1f; // Espera DESPUÉS del fondo
    public float menuFadeDuration = 1.8f;         // Duración fade de los botones

    IEnumerator Start()
    {
        // Estado inicial: todo invisible
        if (backgroundGroup != null)
            backgroundGroup.alpha = 0f;

        if (menuGroup != null)
        {
            menuGroup.alpha = 0f;
            menuGroup.interactable = false;
            menuGroup.blocksRaycasts = false;
        }

        // 1) Espera antes de mostrar el fondo
        yield return new WaitForSecondsRealtime(backgroundDelay);

        // 2) Fade-in del fondo
        if (backgroundGroup != null)
        {
            float t = 0f;
            while (t < backgroundFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / backgroundFadeDuration);
                backgroundGroup.alpha = k; // 0 → 1
                yield return null;
            }

            backgroundGroup.alpha = 1f;
        }

        // 3) Espera extra antes de los botones
        yield return new WaitForSecondsRealtime(menuDelayAfterBackground);

        // 4) Fade-in de los botones
        if (menuGroup != null)
        {
            float t = 0f;
            while (t < menuFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / menuFadeDuration);
                menuGroup.alpha = k; // 0 → 1
                yield return null;
            }

            menuGroup.alpha = 1f;
            menuGroup.interactable = true;
            menuGroup.blocksRaycasts = true;
        }
    }
}
