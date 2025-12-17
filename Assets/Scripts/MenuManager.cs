using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject menuPanel;     // panel con BtnPlay / BtnOptions / BtnQuit
    public GameObject optionsPanel;  // panel de opciones

    [Header("Escenas")]
    [SerializeField] string gameSceneName = "Game";
    

    public void PlayGame()
    {
        if (SceneFader.I != null)
        {
            // Usamos el fader: fade a negro → load → fade desde negro
            SceneFader.I.FadeToScene(gameSceneName);
        }
        else
        {
            // Backup por si algo falla
            Debug.LogWarning("[MenuManager] SceneFader no encontrado, cargando escena directamente.");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowOptions()
    {
        if (menuPanel) menuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (menuPanel) menuPanel.SetActive(true);
    }


    public void OpenOptions()
    {
        if (menuPanel) menuPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (menuPanel) menuPanel.SetActive(true);
    }
}
