using System.Collections.Generic;
using UnityEngine;

public class AutoclickerManager : MonoBehaviour
{
    public static AutoclickerManager I;

    [Header("Refs")]
    public RectTransform canvasRoot;
    public RectTransform agentsRoot;   // Contenedor de agentes (hijo del Canvas)
    public RectTransform clickButton;
    public ClickPulse clickPulse;
    public GameObject agentPrefab;     // Prefab del ratón (UI Image)

    [Header("Parámetros")]
    public int maxAgents = 10;       // tope de ratones visibles
    public float perLevelBonus = 0.15f; // +15% de CPS por nivel (de cada agente)



    // Seguimiento interno (también reconstruimos desde la escena)
    readonly List<AutoclickerAgent> agents = new List<AutoclickerAgent>();

    public int AgentsCount => agentsRoot ? agentsRoot.childCount : 0;
    public bool AtCap => AgentsCount >= maxAgents;

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }

#if UNITY_2023_1_OR_NEWER
        var c = Object.FindFirstObjectByType<Canvas>();
#else
    var c = Object.FindObjectOfType<Canvas>();
#endif
        if (!canvasRoot && c) canvasRoot = c.transform as RectTransform;

        // Crea contenedor si no existe
        if (!agentsRoot)
        {
            var go = new GameObject("Agents", typeof(RectTransform));
            agentsRoot = go.transform as RectTransform;
            agentsRoot.SetParent(canvasRoot, false);
            agentsRoot.anchorMin = agentsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            agentsRoot.anchoredPosition = Vector2.zero;
            agentsRoot.sizeDelta = Vector2.zero;
        }

        // Sincroniza al entrar
        RebuildAgentsListFromScene();
        SanitizeAgentsList();
        ReflowAgents();
    }

    // ====== API para los dos botones ======

    // Botón A: Añadir ratón (hasta maxAgents)
    public void BuyMouse()
    {
        Debug.Log("[AutoClickers] BuyMouse llamado");

        if (!agentsRoot || !agentPrefab || !clickButton)
        {
            Debug.LogWarning("[AutoClickers] Falta asignar agentsRoot / agentPrefab / clickButton en el Inspector");
            return;
        }

        // Contamos agentes reales en escena bajo agentsRoot
        int countInScene = agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false).Length;
        Debug.Log($"[AutoClickers] Agentes en escena antes de comprar: {countInScene}");

        if (countInScene >= maxAgents)
        {
            Debug.Log("[AutoClickers] Ya está en el máximo de agentes, no creo más.");
            return;
        }

        SpawnAgent();
    }

    // Botón B: Mejorar velocidad de 1 ratón (el menos mejorado)
    public void UpgradeSpeed()
    {
        RebuildAgentsListFromScene(); SanitizeAgentsList();
        if (agents.Count == 0) return;

        // Encuentra el agente con menor nivel
        AutoclickerAgent target = agents[0];
        int minLvl = target.speedLevel;

        foreach (var a in agents)
        {
            if (!a) continue;
            if (a.speedLevel < minLvl)
            {
                minLvl = a.speedLevel;
                target = a;
            }
        }

        target.speedLevel += 1;
        target.perLevelBonus = perLevelBonus;
        target.ApplyLevelColor();   // ← pinta el color del nivel
        target.PlayUpgradeFX();     // ← flash
        Debug.Log($"[AutoClickers] UpgradeSpeed → {target.name} ahora nivel {target.speedLevel}");
    }

    // ====== Interna ======

    public void SpawnAgent()
    {
        RebuildAgentsListFromScene(); SanitizeAgentsList();
        if (AgentsCount >= maxAgents) return;

        var go = Instantiate(agentPrefab, agentsRoot);
        var rt = go.transform as RectTransform;
        rt.anchoredPosition = Vector2.zero;

        // Nombre provisional único basado en la lista (antes de agregar)
        int idx = agents.Count;
        string baseName = $"Agent_{idx:00}";
        string name = baseName;
        int tries = 1;
        while (agentsRoot.Find(name) != null)
        {
            name = $"{baseName}_{tries++}";
        }
        go.name = name;

        var agent = go.GetComponent<AutoclickerAgent>() ?? go.AddComponent<AutoclickerAgent>();
        agent.canvasRoot = canvasRoot;
        agent.targetButton = clickButton;
        agent.clickPulse = clickPulse;
        agent.perLevelBonus = perLevelBonus;

        // distribución
        float baseAngle = agents.Count * (360f / Mathf.Max(1, agents.Count + 1));
        agent.angularOffsetDeg = baseAngle + Random.Range(-15f, 15f);
        agent.radius += Random.Range(-12f, 12f);
        agent.angularSpeed += Random.Range(-10f, 10f);

        agents.Add(agent);
        RebuildAgentsListFromScene(); SanitizeAgentsList();
        ReflowAgents();
    }

    void RebuildAgentsListFromScene()
    {
        agents.Clear();
        if (agentsRoot)
            agents.AddRange(agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false));
    }

    void SanitizeAgentsList()
    {
        agents.RemoveAll(a => a == null);
        // (opcional) eliminar duplicados si alguna vez ocurre
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            if (agents.FindAll(x => x == agents[i]).Count > 1)
                agents.RemoveAt(i);
        }
    }

    // Para el HUD
    public double EstimatedCPS(double clickValue)
    {
        double sum = 0;
        foreach (var a in agents)
            if (a) sum += a.baseCPS * (1f + a.speedLevel * perLevelBonus);
        return sum * clickValue;
    }

    // Utilidad (debug) para limpiar sobrantes si alguna vez se colaron
    [ContextMenu("DEBUG/Destroy Extra Agents > maxAgents")]
    void DebugDestroyExtras()
    {
        if (!agentsRoot) return;
        var all = agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false);
        for (int i = all.Length - 1; i >= 0 && all.Length > maxAgents; i--)
            Destroy(all[i].gameObject);

        RebuildAgentsListFromScene();
        SanitizeAgentsList();
        Debug.Log("[AutoClickers] Limpieza hecha.");
    }

    void ReflowAgents()
    {
        if (!agentsRoot) return;
        var list = agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false);
        int n = list.Length;
        if (n == 0) return;
        float step = 360f / n;
        for (int i = 0; i < n; i++)
        {
            var a = list[i];
            if (!a) continue;
            a.angularOffsetDeg = i * step;
            a.radius = 180f + Random.Range(-8f, 8f);
            a.angularSpeed = 90f + Random.Range(-8f, 8f);
        }
    }

    [ContextMenu("DEBUG/Reparent & Enforce Cap")]
    void DebugReparentAndCap()
    {
        if (!canvasRoot || !agentsRoot) return;

        // Reparenta todos los AutoclickerAgent del Canvas a agentsRoot
        var all = canvasRoot.GetComponentsInChildren<AutoclickerAgent>(true);
        foreach (var a in all)
        {
            var rt = a.transform as RectTransform;
            if (rt.parent != agentsRoot) rt.SetParent(agentsRoot, false);
        }

        // Reconstruye lista y aplica tope
        RebuildAgentsListFromScene();
        SanitizeAgentsList();

        // Si hay más de maxAgents, destruye extras
        var list = agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false);
        for (int i = list.Length - 1; i >= maxAgents; i--)
            DestroyImmediate(list[i].gameObject);

        RebuildAgentsListFromScene();
        SanitizeAgentsList();
        ReflowAgents();

        Debug.Log($"[AutoClickers] Reparent hecho. Agentes: {agentsRoot.childCount}/{maxAgents}");
    }

    public int CurrentAgents
    {
        get
        {
            if (!agentsRoot) return 0;
            return agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false).Length;
        }
    }

    public int MaxAgents => maxAgents;



}
