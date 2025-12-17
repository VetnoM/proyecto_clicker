using UnityEngine;

public class MejorasPanelToggle : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform panel;          // Panel Upgrades
    public RectTransform toggleButton;   // Botón "Mejoras"

    [Header("Config")]
    public float slideSpeed = 8f;
    public bool startShown = false;

    [Tooltip("Cuánto se mueve el botón hacia la izquierda al abrir el panel")]
    public float buttonShift = 250f;     // ajústalo en el Inspector

    float panelShownX;
    float panelHiddenX;
    float panelTargetX;
    bool isShown;

    float buttonClosedX;
    float buttonOpenedX;
    float buttonTargetX;

    CanvasGroup panelCanvasGroup;

    void Awake()
    {
        if (!panel) panel = GetComponent<RectTransform>();

        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (!panelCanvasGroup)
            panelCanvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // Panel: posición visible actual
        panelShownX = panel.anchoredPosition.x;
        // Oculto: una anchura hacia la derecha
        panelHiddenX = panelShownX + panel.rect.width;

        // Botón: posición inicial = cerrado
        if (toggleButton)
        {
            buttonClosedX = toggleButton.anchoredPosition.x;
            // Abierto: se mueve a la izquierda buttonShift unidades
            buttonOpenedX = buttonClosedX - buttonShift;
        }

        isShown = startShown;

        panelTargetX = isShown ? panelShownX : panelHiddenX;
        buttonTargetX = isShown ? buttonOpenedX : buttonClosedX;

        // Colocar en la posición inicial
        panel.anchoredPosition =
            new Vector2(panelTargetX, panel.anchoredPosition.y);

        if (toggleButton)
        {
            toggleButton.anchoredPosition =
                new Vector2(buttonTargetX, toggleButton.anchoredPosition.y);
        }

        panelCanvasGroup.interactable = isShown;
        panelCanvasGroup.blocksRaycasts = isShown;
    }

    public void Toggle()
    {
        isShown = !isShown;

        panelTargetX = isShown ? panelShownX : panelHiddenX;
        buttonTargetX = isShown ? buttonOpenedX : buttonClosedX;

        panelCanvasGroup.interactable = isShown;
        panelCanvasGroup.blocksRaycasts = isShown;
    }

    void Update()
    {
        // Panel
        var p = panel.anchoredPosition;
        p.x = Mathf.Lerp(p.x, panelTargetX, Time.deltaTime * slideSpeed);
        panel.anchoredPosition = p;

        // Botón
        if (toggleButton)
        {
            var b = toggleButton.anchoredPosition;
            b.x = Mathf.Lerp(b.x, buttonTargetX, Time.deltaTime * slideSpeed);
            toggleButton.anchoredPosition = b;
        }
    }
}
