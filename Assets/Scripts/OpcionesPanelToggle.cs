using UnityEngine;

public class OpcionesPanelToggle : MonoBehaviour
{
    public RectTransform panel;      // OptionsPanelGame
    public float slideSpeed = 8f;
    public bool startShown = false;

    float shownX;
    float hiddenX;
    float targetX;
    bool isShown;

    CanvasGroup canvasGroup;

    void Awake()
    {
        if (!panel)
            panel = GetComponent<RectTransform>();

        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // posición visible actual
        shownX = panel.anchoredPosition.x;
        // posición oculta: lo mandamos una anchura hacia la derecha
        hiddenX = shownX + panel.rect.width;

        isShown = startShown;
        targetX = isShown ? shownX : hiddenX;

        // colocar en posición inicial
        panel.anchoredPosition = new Vector2(targetX, panel.anchoredPosition.y);

        canvasGroup.interactable = isShown;
        canvasGroup.blocksRaycasts = isShown;
    }

    public void Toggle()
    {
        isShown = !isShown;
        targetX = isShown ? shownX : hiddenX;

        canvasGroup.interactable = isShown;
        canvasGroup.blocksRaycasts = isShown;
    }

    void Update()
    {
        var pos = panel.anchoredPosition;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * slideSpeed);
        panel.anchoredPosition = pos;
    }
}
