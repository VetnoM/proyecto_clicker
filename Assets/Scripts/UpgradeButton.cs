using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        ClickValue,   // Mejora valor por click
        PassiveCPS,   // Mejora CPS pasivo
        Custom        // Llama a un UnityEvent externo (autoclickers, etc.)
    }

    [Header("Config")]
    public UpgradeType type = UpgradeType.ClickValue;

    [Tooltip("Coste base de la mejora en monedas/clicks.")]
    public double baseCost = 10;

    [Tooltip("Multiplicador de coste por nivel (ej: 1.15).")]
    public double costMultiplier = 1.15;

    [Tooltip("Cantidad que se suma por nivel (click value, CPS, etc.).")]
    public double amountPerLevel = 1;

    [Tooltip("Nivel actual de la mejora.")]
    public int level = 0;

    [Header("Referencias UI")]
    public Button button;                 // Botón (se autoasigna si está vacío)
    public TextMeshProUGUI txtName;       // Opcional
    public TextMeshProUGUI txtTitle;      // Opcional
    public TextMeshProUGUI txtCost;       // "Coste: X"
    public TextMeshProUGUI txtLevel;      // "Nivel: N"

    [Header("Evento Custom")]
    public UnityEvent OnPurchased;        // Para mejoras especiales (autoclickers, etc.)

    private GameManager gm;

    // ------------------------------------------------------------
    void Reset() => AutoBindUI();
    void OnValidate() => AutoBindUI();

    void Awake()
    {
        AutoBindUI();
        if (!button) button = GetComponent<Button>();
    }

    void Start()
    {
        gm = GameManager.I != null ? GameManager.I : FindObjectOfType<GameManager>();
        RefreshUI();
    }

    void Update()
    {
        if (button == null) return;

        if (gm == null)
        {
            button.interactable = false;
            return;
        }

        double cost = GetCurrentCost();
        button.interactable = gm.coins >= cost;
    }

    // ------------------------------------------------------------
    public void Buy()
    {
        if (gm == null) return;

        double cost = GetCurrentCost();
        if (gm.coins < cost) return;

        // Pagar
        gm.coins -= cost;

        // Aplicar efecto
        switch (type)
        {
            case UpgradeType.ClickValue:
                gm.AddClickValue(amountPerLevel);
                break;

            case UpgradeType.PassiveCPS:
                gm.AddCPS(amountPerLevel);
                break;

            case UpgradeType.Custom:
                OnPurchased?.Invoke();
                break;
        }

        // Subir nivel y refrescar UI
        level++;
        RefreshUI();
    }

    public double GetCurrentCost()
    {
        // coste = baseCost * costMultiplier^level
        double cost = baseCost * System.Math.Pow(costMultiplier, level);
        return System.Math.Round(cost);
    }

    public void RefreshUI()
    {
        if (txtCost != null)
        {
            txtCost.text = $"Coste: {GetCurrentCost():0}";
            txtCost.ForceMeshUpdate();
        }

        if (txtLevel != null)
        {
            txtLevel.text = $"Nivel: {level}";
            txtLevel.ForceMeshUpdate();
        }

        // Por si el texto está dentro de un LayoutGroup/ContentSizeFitter
        var rt = transform as RectTransform;
        if (rt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    // ------------------------------------------------------------
    [ContextMenu("AutoBind UI")]
    void AutoBindUI()
    {
        if (!button) button = GetComponent<Button>();

        // Si tienes "TextContainer", prioriza esa ruta (tu jerarquía parece así)
        Transform container = transform.Find("TextContainer");
        if (container != null)
        {
            if (!txtName) txtName = container.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
            if (!txtTitle) txtTitle = container.Find("TxtTitle")?.GetComponent<TextMeshProUGUI>();
            if (!txtCost) txtCost = container.Find("TxtCost")?.GetComponent<TextMeshProUGUI>();
            if (!txtLevel) txtLevel = container.Find("TxtLevel")?.GetComponent<TextMeshProUGUI>();
        }

        // Fallback: si algo sigue null, busca por nombre en hijos
        if (!txtCost || !txtLevel || !txtName || !txtTitle)
        {
            var tmps = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in tmps)
            {
                string n = t.name.ToLower();

                if (!txtCost && (n.Contains("txtcost") || n.Contains("cost"))) txtCost = t;
                if (!txtLevel && (n.Contains("txtlevel") || n.Contains("nivel") || n.Contains("level"))) txtLevel = t;

                if (!txtName && n.Contains("txtname")) txtName = t;
                if (!txtTitle && n.Contains("txttitle")) txtTitle = t;
            }
        }
    }
}
