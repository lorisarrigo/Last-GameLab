using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option_Manager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Toggle vSyingToggle;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    public TMP_Text bestScore;

    public static Option_Manager instance;
    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
    private void Start()
    {
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        if (vSyingToggle != null) vSyingToggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);
        if (qualityDropdown != null) qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Opt_Master", 1f));
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Opt_Music", 1f));
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Opt_Sfx", 1f));


        bestScore.text = (Save_Manager.instance.bestDay < 10 ? "0" : "") + Save_Manager.instance.bestDay;
    }
    public void SetMasterVolume(float volume)
    {
        if (SoundMixer_Manager.instance == null)
        { Debug.LogWarning("SoundMixer_Manager non ancora pronto"); return; }
        SoundMixer_Manager.instance.SetMasterVolume(volume);
        PassDataToSaveManager();
    }
    public void SetMusicVolume(float volume)
    {
        if (SoundMixer_Manager.instance == null)
        { Debug.LogWarning("SoundMixer_Manager non ancora pronto"); return; }
        SoundMixer_Manager.instance.SetMusicVolume(volume);
        PassDataToSaveManager();
    }
    public void SetSFXVolume(float volume)
    {
        if (SoundMixer_Manager.instance == null)
        { Debug.LogWarning("SoundMixer_Manager non ancora pronto"); return; }
        SoundMixer_Manager.instance.SetSFXVolume(volume);
        PassDataToSaveManager();
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscrenn impostato a: {isFullscreen}");
        PassDataToSaveManager();
    }
    public void SetVysinc(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        Debug.Log($"VSyinc impostato a: {isVSync}");
        PassDataToSaveManager();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log($"Qualit? grafica impostata al livello {qualityIndex}");
        PassDataToSaveManager();
    }
    void PassDataToSaveManager()
    {
        if (Save_Manager.instance != null)
        {
            Save_Manager.instance.SaveSettings(qualityDropdown.value, vSyingToggle.isOn, fullscreenToggle.isOn, masterSlider.value, musicSlider.value, sfxSlider.value);
        }
    }
}