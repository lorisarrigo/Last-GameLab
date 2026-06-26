using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Save_Manager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] string sceneToLoad = "";

    [Header("Starting Settings")]
    [SerializeField] int startingMoney = 100;
    public int bestDay;

    const string KEY_DAY = "SavedDay", KEY_MONEY = "SavedMoney", KEY_HAS_SAVE = "HasSavedData", KEY_BEST_DAY = "SaveBestDay";
    const string KEY_QUALITY = "Opt_Quality", KEY_VSYNC = "Opt_VSync", KEY_FULLSCREEN = "Opt_Fullscreen";
    const string KEY_MASTER_VOL = "Opt_Master", KEY_MUSIC_VOL = "Opt_Music", KEY_SFX_VOL = "Opt_Sfx";

    bool shouldLoadSavedData = false;
    public static Save_Manager instance;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        bestDay = PlayerPrefs.GetInt(KEY_DAY, 0);
        LoadSettings();
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    public void NewGame()
    {
        shouldLoadSavedData = false;
        SceneManager.LoadScene(sceneToLoad);
    }
    public void LoadGame()
    {
        if (PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1)
        {
            shouldLoadSavedData = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    public void SaveSettings(int qualityIndex, bool isVSyinc, bool isFull, float master, float music, float sfx)
    {
        PlayerPrefs.SetInt(KEY_QUALITY, qualityIndex);
        PlayerPrefs.SetInt(KEY_VSYNC, isVSyinc ? 1 : 0);
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFull ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_MASTER_VOL, master);
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL, music);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sfx);

        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        if (!PlayerPrefs.HasKey(KEY_QUALITY)) return;
        int quality = PlayerPrefs.GetInt(KEY_QUALITY);
        bool vsync = PlayerPrefs.GetInt(KEY_VSYNC) == 1;
        bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN) == 1;
        float master = PlayerPrefs.GetFloat(KEY_MASTER_VOL, 1f);
        float music = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, 1f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);

        QualitySettings.SetQualityLevel(quality);
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        Screen.fullScreen = fullscreen;
        StartCoroutine(ApplySettingsDelayed(master, music, sfx));

    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSettings();
        if (scene.name == sceneToLoad)
        {
            if (shouldLoadSavedData)
            {
                Game_Manager.instance.currentDay = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_DAY));
                Jew_Manager.instance.currentMoney = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_MONEY));
            }
            else
            {
                Game_Manager.instance.currentDay = 0;
                Jew_Manager.instance.currentMoney = startingMoney;
            }
            Jew_Manager.instance.todayGains = 0;
            Jew_Manager.instance.todayTotal = 0;
            Jew_Manager.instance.overallTotal = Jew_Manager.instance.currentMoney;

            UI_Manager.instance.moneyCounter.text = Jew_Manager.instance.currentMoney + " Æ";
        }
    }
    public void SaveGame()
    {
        if (Game_Manager.instance != null && Jew_Manager.instance != null)
        {
            int _currentDay = Game_Manager.instance.currentDay;
            if (_currentDay >= 0)
            {
                if (_currentDay > bestDay)
                {
                    bestDay = _currentDay;
                    PlayerPrefs.SetInt(KEY_BEST_DAY, bestDay);
                }
                PlayerPrefs.SetFloat(KEY_DAY, (float)_currentDay);
                PlayerPrefs.SetFloat(KEY_MONEY, (float)Jew_Manager.instance.overallTotal);
                PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
                PlayerPrefs.Save();
            }
        }
    }
    IEnumerator ApplySettingsDelayed(float master, float music, float sfx)
    {
        while (SoundMixer_Manager.instance == null) yield return null;


        SoundMixer_Manager.instance.SetMasterVolume(master);
        SoundMixer_Manager.instance.SetMusicVolume(music);
        SoundMixer_Manager.instance.SetSFXVolume(sfx);
    }
}
