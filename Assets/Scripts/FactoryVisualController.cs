using TMPro;
using UnityEngine;

public class FactoryVisualController : MonoBehaviour
{
    [Header("Referencias en escena")]
    [SerializeField] private UpgradeButton factoryUpgrade;
    [SerializeField] private RectTransform effectsRoot;          // Canvas/EffectsRoot
    [SerializeField] private TextMeshProUGUI floatingTextPrefab; // el mismo que ClickFXManager

    [Header("Mouse Factory")]
    [SerializeField] private FactoryMouseAgent mousePrefab; // prefab del mouse (UI)
    [SerializeField] private Transform mouseParent;         // Canvas/FactoryMouseRoot

    private FactoryMouseAgent mouseInstance;

    public void OnFactoryPurchased()
    {
        if (!factoryUpgrade || !mousePrefab || !mouseParent)
        {
            Debug.LogError("FactoryVisualController: faltan referencias (factoryUpgrade/mousePrefab/mouseParent).");
            return;
        }

        if (mouseInstance == null)
            mouseInstance = Instantiate(mousePrefab, mouseParent);

        mouseInstance.effectsRoot = effectsRoot;
        mouseInstance.floatingTextPrefab = floatingTextPrefab;

        double perTick = factoryUpgrade.level * factoryUpgrade.amountPerLevel;
        mouseInstance.SetAmountPerTick(perTick);
    }
}
