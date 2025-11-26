using UnityEngine;

public class AutoClickerVFXManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;   // arrástralo en el Inspector
    [SerializeField] AutoclickerManager autoMgr;

    void Awake()
    {
        // Si olvidaste asignarlo, como fallback usamos Find* compatible (ver Opción B)
        if (!gameManager || !autoMgr)
        {
            gameManager = gameManager ? gameManager : FindOne<GameManager>();
            autoMgr = autoMgr ? autoMgr : FindOne<AutoclickerManager>();
        }
    }

    // helper compatible 2023+
    static T FindOne<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }
}
