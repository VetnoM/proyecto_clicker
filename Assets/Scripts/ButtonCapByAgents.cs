using UnityEngine;
using UnityEngine.UI;

public class ButtonCapByAgents : MonoBehaviour
{
    public AutoclickerManager manager;
    public Button button;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (!manager) manager = FindFirstObjectByType<AutoclickerManager>();
    }

    void Update()
    {
        // Solo forzamos a false cuando se llega al límite.
        if (manager.AgentsCount >= manager.maxAgents)
        {
            button.interactable = false;
        }
    }
}
