using UnityEngine;
using TMPro;

public class MouseCountTitle : MonoBehaviour
{
    public AutoclickerManager manager;
    public TextMeshProUGUI title;

    static T FindOne<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }

    void Awake()
    {
        if (!manager) manager = FindOne<AutoclickerManager>();
        if (!title) title = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!manager || !title) return;
        title.text = $"Añadir ratón ({manager.AgentsCount}/{manager.maxAgents})";
    }
}
