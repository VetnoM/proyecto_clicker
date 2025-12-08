using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType { ClickValue, Passive, Custom }

    [Header("Tipo de mejora")]
    public UpgradeType type = UpgradeType.ClickValue;

    [Header("Economía")]
    public double baseCost = 10;
    public double costMultiplier = 1.15;
    public double amountPerLevel = 1;

    [Header("Estado")]
    public int level = 0;

    [Header("UI")]
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtLevel;
    public Button button;

    [Header("Evento al comprar (solo si type = Custom)")]
    public UnityEvent OnPurchased;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        RefreshUI();

        if (GameManager.I)
            button.interactable = GameManager.I.coins >= CurrentCost();
    }

    void Update()
    {
        if (!button || !GameManager.I) return;

        double cost = CurrentCost();
        // IMPORTANTE: sin "&& button.interactable"
        button.interactable = GameManager.I.coins >= cost;
    }

    public void Buy()
    {
        if (!GameManager.I) return;

        double cost = CurrentCost();
        if (GameManager.I.coins < cost) return;

        // pagar
        GameManager.I.coins -= cost;

        // aplicar según tipo
        switch (type)
        {
            case UpgradeType.ClickValue:
                GameManager.I.AddClickValue(amountPerLevel);
                break;

            case UpgradeType.Passive:
                GameManager.I.AddCPS(amountPerLevel);
                break;

            case UpgradeType.Custom:
                // lo ejecuta OnPurchased
                break;
        }

        level++;

        // evento custom (para autoclickers, etc.)
        OnPurchased?.Invoke();

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
        if (txtLevel) txtLevel.text = $"Nivel: {level}";
    }
}
