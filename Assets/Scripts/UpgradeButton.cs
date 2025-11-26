using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType { ClickValue, Passive, Custom }

    [Header("Tipo de mejora")]
    public UpgradeType type = UpgradeType.Custom;

    [Header("Economía")]
    public double baseCost = 50;
    public double costMultiplier = 1.15;   // coste escala por nivel
    public double amountPerLevel = 1;      // ClickValue = +X por click; Passive = +X cps

    [Header("Estado")]
    public int level = 0;

    [Header("UI")]
    public TextMeshProUGUI txtName;   // opcional
    public TextMeshProUGUI txtTitle;  // opcional
    public TextMeshProUGUI txtCost;   // coste
    public Button button;

    [Header("Evento al comprar (para Custom)")]
    public UnityEvent OnPurchased;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        RefreshUI();
        if (GameManager.I) button.interactable = GameManager.I.coins >= CurrentCost();
    }

    void OnEnable()
    {
        if (!button) button = GetComponent<Button>();
        if (button)
            button.onClick.AddListener(Buy);
    }

    void OnDisable()
    {
        if (button)
            button.onClick.RemoveListener(Buy);
    }

    void Update()
    {
        if (!button || !GameManager.I) return;
        double cost = CurrentCost();
        button.interactable = GameManager.I.coins >= cost; // ← FIX
    }

    public void Buy()
    {
        if (!GameManager.I) return;

        double cost = CurrentCost();
        if (GameManager.I.coins < cost) return;

        // Pagar
        GameManager.I.coins -= cost;

        // Aplicar
        switch (type)
        {
            case UpgradeType.ClickValue: GameManager.I.AddClickValue(amountPerLevel); break;
            case UpgradeType.Passive: GameManager.I.AddCPS(amountPerLevel); break;
            case UpgradeType.Custom:     /* vía OnPurchased */                        break;
        }

        level++;

        // Evento custom (autoclickers, etc.)
        OnPurchased?.Invoke();

        // Refrescar textos
        GameManager.I.SendMessage("UpdateUI", SendMessageOptions.DontRequireReceiver);
        RefreshUI();
    }

    public double CurrentCost()
    {
        return baseCost * System.Math.Pow(costMultiplier, level);
    }

    public void RefreshUI()
    {
        if (txtCost) txtCost.text = $"Coste: {CurrentCost():0}";
    }
}
