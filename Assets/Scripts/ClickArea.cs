using UnityEngine;
using UnityEngine.EventSystems;

public class ClickArea : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ClickPulse clickPulse;
    [SerializeField] private ClickFXManager clickFXManager; // arrástralo en el inspector si lo tienes
    [SerializeField] private float fxCooldown = 0.08f;

    private float lastFxTime = -999f;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Animación (opcional)
        if (clickPulse != null) clickPulse.Play();

        // FX / texto flotante (solo 1 vez por pulsación)
        if (Time.unscaledTime - lastFxTime < fxCooldown) return;
        lastFxTime = Time.unscaledTime;

        if (clickFXManager != null)
        {
           // clickFXManager.PlayClickFX(); // <-- si en tu ClickFXManager el método se llama distinto, cámbialo aquí
        }
    }
}
