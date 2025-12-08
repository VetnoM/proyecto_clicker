using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // por si lo necesitas más tarde
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Economía")]
    public double coins = 0;              // tu “moneda”: la llamamos Clicks en UI
    public double coinsPerClick = 1;
    public double coinsPerSecond = 0;     // pasivo (si lo usas)

    [Header("UI")]
    public TextMeshProUGUI coinsText;     // mostrará: Clicks: X
    public TextMeshProUGUI cpsText;       // mostrará: CPS: Y

 

    // CPS manual (media móvil)
    readonly Queue<float> clickTimes = new Queue<float>();
    [SerializeField] float cpsWindowSeconds = 3f;

    public double PlayerCPS
    {
        get
        {
            float now = Time.unscaledTime;
            while (clickTimes.Count > 0 && now - clickTimes.Peek() > cpsWindowSeconds)
                clickTimes.Dequeue();
            if (cpsWindowSeconds <= 0f) return 0;
            return clickTimes.Count / (double)cpsWindowSeconds;
        }
    }

    void Awake()
    {
        if (I == null) I = this; else { Destroy(gameObject); return; }
        UpdateUI();
    }

    void Update()
    {
        if (coinsPerSecond > 0)
            coins += coinsPerSecond * Time.deltaTime;

        //uiTimer += Time.deltaTime;
        //if (uiTimer >= UI_REFRESH)
        //{
        //    uiTimer = 0f;
        //    UpdateUI();
        //}
        UpdateUI();
    }

    // Click manual del botón central
    public void OnClick()
    {
        coins += coinsPerClick;

        // registrar el click para el CPS del jugador
        clickTimes.Enqueue(Time.unscaledTime);

        //// 🔹 REFRESCAR UI AL INSTANTE
        //UpdateUI();

        ////// FX de texto+partículas
        ////ClickFXManager.I?.PlayClickFX($"+{coinsPerClick:0}", 36);

//        Vector2 screenPos;
//#if ENABLE_INPUT_SYSTEM
//        if (Mouse.current != null)
//            screenPos = Mouse.current.position.ReadValue();
//        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
//            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
//        else
//            screenPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
//#else
//    screenPos = Input.mousePosition;
//#endif

//        if (ClickFXManager.I && ClickFXManager.I.canvasRoot)
//        {
//            Vector2 uiPos;
//            RectTransformUtility.ScreenPointToLocalPointInRectangle(
//                ClickFXManager.I.canvasRoot, screenPos, null, out uiPos
//            );

//            ClickFXManager.I.PlayClickFXAt(uiPos, $"+{coinsPerClick:0}", 28);
//        }
    }


    // APIs que usan los botones de mejora
    public void AddClickValue(double amount)
    {
        coinsPerClick += amount;
        if (coinsPerClick < 0) coinsPerClick = 0;
        UpdateUI();
    }

    public void AddCPS(double amount)
    {
        coinsPerSecond += amount;
        if (coinsPerSecond < 0) coinsPerSecond = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinsText) coinsText.text = $"Clicks: {coins:0}";

        if (cpsText)
        {
            double totalCPS = PlayerCPS + coinsPerSecond;
            cpsText.text = $"CPS: {totalCPS:0.##}";
          
        }
    }

}
