using TMPro;
using UnityEngine;

public class MouseCountTitle : MonoBehaviour
{
    public AutoclickerManager manager;
    public TextMeshProUGUI title;

    void Awake()
    {
        if (!title) title = GetComponent<TextMeshProUGUI>();
        if (!manager) manager = FindFirstObjectByType<AutoclickerManager>();
    }

    void Update()
    {
        if (!manager || !title) return;
        title.text = $"Añadir ratón ({manager.AgentsCount}/{manager.maxAgents})";
    }
}
