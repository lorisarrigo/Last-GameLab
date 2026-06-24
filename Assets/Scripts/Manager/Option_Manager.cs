using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option_Manager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Toggle vSyingToggle;
    [SerializeField] Toggle fullscreenToggle;
    public TMP_Text bestScore;
    
    public static Option_Manager instance;
    private void Awake()
    {
        if(instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
    private void Start()
    {
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
        if (vSyingToggle != null) vSyingToggle.isOn = QualitySettings.vSyncCount > 0;
        if (qualityDropdown != null) qualityDropdown.value = QualitySettings.GetQualityLevel();
        bestScore.text = (Save_Manager.instance.bestDay < 10 ? "0" : "") + Save_Manager.instance.bestDay;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscrenn impostato a: {isFullscreen}");
    }
    public void SetVysinc(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        Debug.Log($"VSyinc impostato a: {isVSync}");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log($"Qualit? grafica impostata al livello {qualityIndex}");
    }
}