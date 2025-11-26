using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[DefaultExecutionOrder(1000)] // corre después de la mayoría de scripts de UI
public class ButtonCapByAgents : MonoBehaviour
{
    public AutoclickerManager manager;

    Button button;
    CanvasGroup cg;

    static T FindOne<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }

    void Awake()
    {
        button = GetComponent<Button>();
        cg = GetComponent<CanvasGroup>(); // opcional
        if (!manager) manager = FindOne<AutoclickerManager>();
        // estado inicial conservador: que empiece apagado y ya otros lo enciendan si toca
        if (button) button.interactable = false;
        if (cg) { cg.interactable = false; cg.blocksRaycasts = false; }
    }

    IEnumerator Start()
    {
        // espera a que todos los Start/OnEnable de otros scripts corran
        yield return null;                  // siguiente frame
        yield return new WaitForEndOfFrame(); // final del frame
        Apply(); // aplica una vez al final del primer frame
    }

    void LateUpdate()
    {
        Apply(); // y luego cada frame, al final
    }

    void Apply()
    {
        if (!button || !manager) return;

        bool atCap = manager.AtCap;

        if (atCap)
        {
            // Fuerza OFF cuando llegaste al tope
            if (button.interactable) button.interactable = false;
            if (cg)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
                // cg.alpha = 0.6f; // (opcional) aspecto apagado
            }
        }
        else
        {
            // No tocamos el interactable si no hay tope: lo decide UpgradeButton (dinero)
            if (cg)
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                // cg.alpha = 1f;
            }
        }
    }
}
