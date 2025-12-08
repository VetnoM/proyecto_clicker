using UnityEngine;
using UnityEngine.EventSystems;

public class ClickArea : MonoBehaviour, IPointerDownHandler
{
    public ClickPulse clickPulse;

    [Header("FX")]
    public float fxCooldown = 0.08f;   // mínimo tiempo entre FX (~12 por segundo)
    float lastFxTime;

    void Awake()
    {
        if (!clickPulse) clickPulse = GetComponent<ClickPulse>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1) Lógica del click (barata)
        if (GameManager.I != null)
            GameManager.I.OnClick();

        // 2) Animación del botón (barata)
        if (clickPulse)
            clickPulse.Play();

        // 3) FX de "+X" con frecuencia limitada
        if (ClickFXManager.I != null && GameManager.I != null)
        {
            float now = Time.unscaledTime;
            if (now - lastFxTime >= fxCooldown)
            {
                lastFxTime = now;
                ClickFXManager.I.PlayClickFX(
                    $"+{GameManager.I.coinsPerClick:0}",
                    20 // menos partículas que antes
                );
            }
        }
    }
}
