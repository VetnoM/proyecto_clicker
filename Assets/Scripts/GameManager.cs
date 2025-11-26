using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Nuevo Input System
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Economía")]
    public double coins = 0;
    public double coinsPerClick = 1;
    public double coinsPerSecond = 0;

    [Header("UI")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI cpsText; // CPS manual (media móvil)

    // Refresco UI
    float uiTimer = 0f;
    const float UI_REFRESH = 0.25f;

    // CPS manual (ventana móvil)
    readonly Queue<float> clickTimes = new Queue<float>();
    [SerializeField] float cpsWindowSeconds = 3f;

    public double PlayerCPS
    {
        get
        {
            float now = Time.unscaledTime;
            while (clickTimes.Count > 0 && now - clickTimes.Peek() > cpsWindowSeconds)
                clickTimes.Dequeue();
            if (cpsWindowSeconds <= 0f) return 0;
            return clickTimes.Count / (double)cpsWindowSeconds;
        }
    }

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }
        UpdateUI();
    }

    void Update()
    {
        if (coinsPerSecond > 0)
            coins += coinsPerSecond * Time.deltaTime;

        uiTimer += Time.deltaTime;
        if (uiTimer >= UI_REFRESH)
        {
            uiTimer = 0f;
            UpdateUI();
        }
    }

    /// Click del jugador (desde el botón).
    public void OnClick()
    {
        coins += coinsPerClick;
        clickTimes.Enqueue(Time.unscaledTime);

        // FX: SOLO una llamada, centrada en el botón (más potente)
        ClickFXManager.I?.PlayClickFX($"+{coinsPerClick:0}", 36);
        // Nota: no llamamos UpdateUI aquí; ya refresca en Update() cada 0.25s.
    }

    /// Alternativa si quisieras separar explícitamente el click manual.
    public void OnManualClick()
    {
        coins += coinsPerClick;
        clickTimes.Enqueue(Time.unscaledTime);
        ClickFXManager.I?.PlayClickFX($"+{coinsPerClick:0}", 36);
    }

    // Mejoras usadas por los botones
    public void AddClickValue(double amount)
    {
        coinsPerClick += amount;
        if (coinsPerClick < 0) coinsPerClick = 0;
        UpdateUI();
    }

    public void AddCPS(double amount)
    {
        coinsPerSecond += amount;
        if (coinsPerSecond < 0) coinsPerSecond = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinsText) coinsText.text = $"Monedas: {coins:0}";
        if (cpsText) cpsText.text = $"CPS: {PlayerCPS:0.##}";
    }
}
