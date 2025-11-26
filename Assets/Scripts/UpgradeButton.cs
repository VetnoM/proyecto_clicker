using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    GameManager game;

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
        game = GameManager.I ?? FindObjectOfType<GameManager>();

        RefreshUI();
        EvaluateInteractable();
    }

    void OnEnable()
    {
        if (!button) button = GetComponent<Button>();
        if (!game) game = GameManager.I ?? FindObjectOfType<GameManager>();

        if (button)
            button.onClick.AddListener(Buy);

        EvaluateInteractable();
    }

    void OnDisable()
    {
        if (button)
            button.onClick.RemoveListener(Buy);
    }

    void Update()
    {
        if (!button)
            return;

        if (!game)
            game = GameManager.I ?? FindObjectOfType<GameManager>();

        EvaluateInteractable();
    }

    public void Buy()
    {
        if (!game) return;

        double cost = CurrentCost();
        if (game.coins < cost) return;

        // Pagar
        game.coins -= cost;

        // Aplicar
        switch (type)
        {
            case UpgradeType.ClickValue: game.AddClickValue(amountPerLevel); break;
            case UpgradeType.Passive: game.AddCPS(amountPerLevel); break;
            case UpgradeType.Custom:     /* vía OnPurchased */                        break;
        }

        level++;

        // Evento custom (autoclickers, etc.)
        OnPurchased?.Invoke();

        // Refrescar textos
        game.SendMessage("UpdateUI", SendMessageOptions.DontRequireReceiver);
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

    void EvaluateInteractable()
    {
        if (!button) return;

        if (!game)
            game = GameManager.I ?? FindObjectOfType<GameManager>();

        if (!game)
        {
            button.interactable = false;
            return;
        }

        double cost = CurrentCost();
        button.interactable = game.coins >= cost;
    }
}
