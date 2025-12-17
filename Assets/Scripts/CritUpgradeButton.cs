using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CritUpgradeButton : MonoBehaviour
{
    [Header("Config")]
    public double baseCost = 50;
    public float costMultiplier = 1.25f;

    public float critChancePerLevel = 0.02f;   // +2% por nivel
    public float maxCritChance = 0.5f;         // 50% máximo

    public float critMultiplierBase = 2f;      // x2 fijo (por ahora)

    [Header("UI")]
    public Button button;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtInfo;

    int level = 0;

    GameManager gm;

    void Start()
    {
        gm = GameManager.I;
        RefreshUI();
    }

    double CurrentCost()
    {
        return Mathf.RoundToInt((float)(baseCost * Mathf.Pow(costMultiplier, level)));
    }

    void Update()
    {
        if (gm == null) gm = GameManager.I;
        if (gm == null || button == null) return;

        button.interactable = gm.coins >= CurrentCost();
    }

    public void Buy()
    {
        if (gm == null) gm = GameManager.I;
        if (gm == null) return;

        double cost = CurrentCost();
        if (!gm.TrySpendClicks(cost)) return;

        level++;

        gm.critMultiplier = critMultiplierBase;
        gm.critChance = Mathf.Min(maxCritChance, gm.critChance + critChancePerLevel);

        RefreshUI();
    }

    void RefreshUI()
    {
        if (txtCost != null) txtCost.text = $"Coste: {CurrentCost():0}";
        if (txtLevel != null) txtLevel.text = $"Nivel: {level}";
        if (txtInfo != null) txtInfo.text = $"Prob: {gm.critChance * 100f:0}%  Mult: x{gm.critMultiplier:0.0}";
    }
}
