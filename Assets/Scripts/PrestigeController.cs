using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeController : MonoBehaviour
{
    [Header("UI (Panel Prestigio)")]
    [SerializeField] private TextMeshProUGUI txtPrestigeLevel;

    [Header("Renacer")]
    [SerializeField] private Button btnRenacer;
    [SerializeField] private TextMeshProUGUI txtRenacerInfo;

    [Header("Progreso Renacer")]
    [SerializeField] private Slider renacerProgress;               // 0..1
    [SerializeField] private TextMeshProUGUI txtRenacerProgress;   // "X / Y"

    [Header("Requisito Renacer (lineal)")]
    [SerializeField] private double baseRequiredClicks = 100;      // Prestigio 0 -> 100
    [SerializeField] private double clicksPerPrestige = 100;       // +100 por nivel

    [Header("Mejoras avanzadas")]
    [SerializeField] private Button btnMejorasAvanzadas;
    [SerializeField] private int requiredPrestigeForAdvanced = 2;
    [SerializeField] private TextMeshProUGUI txtAdvancedLockedInfo; // opcional

    [Header("Opcional: limpiar instancias al renacer")]
    [SerializeField] private Transform agentsRoot;
    [SerializeField] private Transform factoryMouseRoot;

    [Header("Guardar/Cargar")]
    [SerializeField] private string prestigeKey = "prestigeLevel";

    [Header("Botón HUD (aparece al desbloquear)")]
[SerializeField] private Button btnAdvancedHUD;
[SerializeField] private string advancedHudKey = "advancedHudEnabled";

[SerializeField] private UnlockPopup unlockPopup;



    public int PrestigeLevel { get; private set; }

    private GameManager gm;

    void Awake()
    {
        PrestigeLevel = PlayerPrefs.GetInt(prestigeKey, 0);
    }

    void Start()
    {
        gm = GameManager.I != null ? GameManager.I : FindFirstObjectByType<GameManager>();
        // HUD: por defecto oculto, pero si ya lo activaste antes lo recordamos
if (btnAdvancedHUD != null)
{
    bool hudEnabled = PlayerPrefs.GetInt(advancedHudKey, 0) == 1;
    bool unlocked = PrestigeLevel >= requiredPrestigeForAdvanced;
    btnAdvancedHUD.gameObject.SetActive(hudEnabled && unlocked);
}
        RefreshUI();
    }

    void Update()
    {
        RefreshUI();
    }

    double RequiredClicksForNextPrestige()
    {
        // Prestigio 0 -> 100, 1 -> 200, 2 -> 300 ...
        return baseRequiredClicks + (PrestigeLevel * clicksPerPrestige);
    }

  void RefreshUI()
{
    // 1) Esto NO depende de GameManager, así que se actualiza siempre
    if (txtPrestigeLevel != null)
        txtPrestigeLevel.text = $"Prestigio: {PrestigeLevel}";

    bool advancedUnlocked = PrestigeLevel >= requiredPrestigeForAdvanced;
    bool hudEnabled = PlayerPrefs.GetInt(advancedHudKey, 0) == 1;

    // Botón del panel: solo sirve para “desbloquear” una vez
    if (btnMejorasAvanzadas != null)
        btnMejorasAvanzadas.interactable = advancedUnlocked && !hudEnabled;

    if (txtAdvancedLockedInfo != null)
    {
        if (!advancedUnlocked) txtAdvancedLockedInfo.text = $"Requiere Prestigio {requiredPrestigeForAdvanced}";
        else if (!hudEnabled) txtAdvancedLockedInfo.text = "Pulsa para desbloquear";
        else txtAdvancedLockedInfo.text = "Ya desbloqueado ✅";
    }

    // 2) Ahora sí, lo que depende de GameManager
    if (gm == null)
        gm = GameManager.I != null ? GameManager.I : FindFirstObjectByType<GameManager>();

    if (gm == null) return;

    double current = gm.coins;
    double req = RequiredClicksForNextPrestige();

    bool canRenacer = current >= req;
    if (btnRenacer != null) btnRenacer.interactable = canRenacer;

    if (txtRenacerInfo != null)
    {
        double falta = req - current;
        txtRenacerInfo.text = (falta <= 0) ? "Renacer listo (+1 Prestigio)" : $"Te faltan {falta:0} clicks";
    }

    float p = (req <= 0) ? 1f : (float)(current / req);
    p = Mathf.Clamp01(p);

    if (renacerProgress != null)
    {
        renacerProgress.minValue = 0f;
        renacerProgress.maxValue = 1f;
        renacerProgress.value = p;
        renacerProgress.interactable = false;
    }

    if (txtRenacerProgress != null)
    {
        double shown = current > req ? req : current;
        txtRenacerProgress.text = $"{shown:0} / {req:0}";
    }
}


    public void Renacer()
    {
        if (gm == null) return;

        double req = RequiredClicksForNextPrestige();
        if (gm.coins < req) return;

        // 1) subir prestigio y guardar
        PrestigeLevel++;
        PlayerPrefs.SetInt(prestigeKey, PrestigeLevel);
        PlayerPrefs.Save();

        // 2) resetear niveles de upgrades + refrescar textos
        var upgrades = FindObjectsByType<UpgradeButton>(FindObjectsSortMode.None);
        foreach (var u in upgrades)
        {
            u.level = 0;
            u.RefreshUI();
        }

        // 3) limpiar instancias opcional
        ClearChildren(agentsRoot);
        ClearChildren(factoryMouseRoot);

        // 4) reset run (coins/cps/clickValue)
        gm.ResetRun();

        RefreshUI();
    }

    void ClearChildren(Transform root)
    {
        if (root == null) return;
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

public void EnableAdvancedHudButton()
{
    if (PrestigeLevel < requiredPrestigeForAdvanced) return;

    bool hudAlreadyEnabled = PlayerPrefs.GetInt(advancedHudKey, 0) == 1;

    // Si no estaba habilitado, lo habilitamos
    if (!hudAlreadyEnabled)
    {
        PlayerPrefs.SetInt(advancedHudKey, 1);
        PlayerPrefs.Save();

        if (btnAdvancedHUD != null)
            btnAdvancedHUD.gameObject.SetActive(true);
    }

    // ✅ POPUP (aunque ya estuviera habilitado)
    if (unlockPopup != null)
        unlockPopup.ShowOnce("¡NUEVAS MEJORAS DESBLOQUEADAS!");
        
RefreshUI();
}


}
