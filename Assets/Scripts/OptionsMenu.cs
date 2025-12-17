using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    const string VOLUME_KEY = "volume";

    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI txtVolume; // opcional

    [Header("Default")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultVolume = 0.6f;
    

    void OnEnable()
    {
        ApplySavedVolumeToUI();
        HookSlider();
        
    }

void ApplySavedVolumeToUI()
{
    float v = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
    v = Mathf.Clamp01(v);

    // NO tocar AudioListener ni MusicManager aquí

    if (volumeSlider != null)
        volumeSlider.SetValueWithoutNotify(v);

    if (txtVolume != null)
        txtVolume.text = Mathf.RoundToInt(v * 100f).ToString();
}



    void HookSlider()
    {
        if (volumeSlider == null) return;

        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float v)
    {
        v = Mathf.Clamp01(v);

        AudioListener.volume = v;

        PlayerPrefs.SetFloat(VOLUME_KEY, v);
        PlayerPrefs.Save();

        if (txtVolume != null)
            txtVolume.text = Mathf.RoundToInt(v * 100f).ToString();

        if (MusicManager.I != null)
            MusicManager.I.SetMasterVolume(v);
    }
}
