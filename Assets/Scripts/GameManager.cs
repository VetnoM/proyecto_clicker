using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Economía (run actual)")]
    public double coins = 0;        // tus "clicks" actuales (aunque se llamen coins)
    public double clickValue = 1;   // cuánto suma cada click
    public double cps = 0;    
    public int totalClicks = 0;
      // clicks por segundo

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textClicks; // "Clicks: X"
    [SerializeField] private TextMeshProUGUI textCPS;    // "CPS: X"

    [Header("Renacer (valores base)")]
    [SerializeField] private double baseClickValue = 1;
    [SerializeField] private double baseCPS = 0;
    private bool baseSaved = false;

    [Header("Crítico (Mejoras Avanzadas)")]
public float critChance = 0f;      // 0.10 = 10%
public float critMultiplier = 2f;  // x2


    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    void Start()
    {
        SaveBaseIfNeeded();
        RefreshUI();
        ResetRun();

    }

    void Update()
    {
        // Sumar CPS continuamente
        if (cps > 0)
        {
            coins += cps * Time.deltaTime;
            RefreshUI();
        }
    }

    // ---------- CLICK (lo que te faltaba para ClickArea.cs) ----------
public void OnClick()
{
    totalClicks++;

    bool isCrit = Random.value < critChance;
    double gain = clickValue * (isCrit ? critMultiplier : 1f);

    coins += gain;
    RefreshUI();

    if (ClickFXManager.I != null)
    {
        if (isCrit)
            ClickFXManager.I.PlayClickFXColored($"<b>CRIT</b> +{gain:0}", new Color(1f, 0.2f, 0.2f, 1f));
        else
            ClickFXManager.I.PlayClickFX($"+{gain:0}");
    }
}


    // ---------- UPGRADES ----------
    public void AddClickValue(double amount)
    {
        clickValue += amount;
        RefreshUI();
    }

    public void AddCPS(double amount)
    {
        cps += amount;
        RefreshUI();
    }

    public bool TrySpendClicks(double amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        RefreshUI();
        return true;
    }

    // Alias opcional para usar "Clicks" sin renombrar coins
    public double Clicks
    {
        get => coins;
        set { coins = value; RefreshUI(); }
    }

    // ---------- RENACER / PRESTIGIO ----------
    public void SaveBaseIfNeeded()
    {
        if (baseSaved) return;
        baseClickValue = clickValue;
        baseCPS = cps;
        baseSaved = true;
    }

    public void ResetRun()
    {
        coins = 0;
        clickValue = baseClickValue;
        cps = baseCPS;
        RefreshUI();
    }

    // ---------- UI ----------
    public void RefreshUI()
    {
        if (textClicks != null) textClicks.text = $"Clicks: {coins:0}";
        if (textCPS != null) textCPS.text = $"CPS: {cps:0}";
    }

    public double coinsPerClick
    {
        get => clickValue;
        set => clickValue = value;
    }

    public double coinsPerSecond
    {
        get => cps;
        set => cps = value;
    }
}
