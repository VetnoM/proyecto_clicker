using System.Collections.Generic;
using UnityEngine;

public class AutoclickerManager : MonoBehaviour
{
    public static AutoclickerManager I;

    [Header("Referencias")]
    public RectTransform canvasRoot;
    public RectTransform agentsRoot;   // padre para los agentes (hijo del Canvas)
    public RectTransform clickButton;  // botón central
    public ClickPulse clickPulse;      // pulso del botón central
    public GameObject agentPrefab;     // prefab del ratón

    [Header("Config")]
    public int maxAgents = 10;

    readonly List<AutoclickerAgent> agents = new List<AutoclickerAgent>();

    public int AgentsCount
    {
        get
        {
            if (!agentsRoot) return 0;
            return agentsRoot.GetComponentsInChildren<AutoclickerAgent>(false).Length;
        }
    }

void Awake()
{
    if (I == null) I = this;
    else { Destroy(gameObject); return; }

    if (!canvasRoot)
    {
        var c = FindFirstObjectByType<Canvas>();
        if (c) canvasRoot = c.transform as RectTransform;
    }
}


    public void BuyMouse()
    {
        if (!agentsRoot || !agentPrefab || !clickButton) return;

        int countInScene = AgentsCount;
        if (countInScene >= maxAgents) return;

        SpawnAgent();
    }

    void SpawnAgent()
    {
        var go = Instantiate(agentPrefab, agentsRoot);
        var rt = go.GetComponent<RectTransform>();
        if (!rt) rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        var agent = go.GetComponent<AutoclickerAgent>();
        if (!agent) agent = go.AddComponent<AutoclickerAgent>();

        agent.canvasRoot = canvasRoot;
        agent.targetButton = clickButton;
        agent.clickPulse = clickPulse;

        // Repartir ángulos para que no se monten
        int index = agents.Count;
        float angleStep = 360f / Mathf.Max(1, maxAgents);
        agent.angularOffsetDeg = index * angleStep + Random.Range(-10f, 10f);

        agents.Add(agent);
    }

    public void UpgradeSpeed()
    {
        agents.RemoveAll(a => a == null);
        if (agents.Count == 0) return;

        // Escoge el agente con menos nivel de velocidad
        AutoclickerAgent target = agents[0];
        foreach (var a in agents)
        {
            if (a && a.speedLevel < target.speedLevel)
                target = a;
        }

        target.speedLevel++;
        target.ApplyLevelColor();
        target.PlayUpgradeFX();
    }
}
