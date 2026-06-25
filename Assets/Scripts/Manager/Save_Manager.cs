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
    const string KEY_QUALITY = "", KEY_VSYNC = "", KEY_FULLSCREEN = "", KEY_VOLUME = "";


    bool shouldLoadSavedData = false;
    public static Save_Manager instance;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        bestDay = PlayerPrefs.GetInt(KEY_DAY,0);
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
        if(PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1)
        {
            shouldLoadSavedData = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    public void SaveSettings(int qualityIndex, bool isVSyinc, bool isFull)
    {
        PlayerPrefs.SetInt(KEY_QUALITY, qualityIndex);
        PlayerPrefs.SetInt(KEY_VSYNC, isVSyinc ? 1:0);
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFull ? 1:0);

        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        if(PlayerPrefs.HasKey(KEY_QUALITY))
        {
            int quality = PlayerPrefs.GetInt(KEY_QUALITY);
            bool vsync = PlayerPrefs.GetInt(KEY_VSYNC) == 1;
            bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN) == 1;

            QualitySettings.SetQualityLevel(quality);
            QualitySettings.vSyncCount = vsync ? 1 : 0;
            Screen.fullScreen = fullscreen;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == sceneToLoad)
        {
            if(shouldLoadSavedData)
            {
                Game_Manager.instance.currentDay = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_DAY));
                Jew_Manager.instance.currentMoney = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_MONEY));
                
                Debug.Log($"[SaveSystem]Dati caricati con successo!"); 
            }
            else
            {
                Game_Manager.instance.currentDay = 0;
                Jew_Manager.instance.currentMoney = startingMoney;
                Debug.Log("[SaveSystem] Nuova partita avviata con dati di default.");
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
            if(_currentDay >= 0)
            {
                if(_currentDay > bestDay)
                {
                    bestDay = _currentDay;
                    PlayerPrefs.SetInt(KEY_BEST_DAY, bestDay);
                }
                PlayerPrefs.SetFloat(KEY_DAY, (float)_currentDay);
                PlayerPrefs.SetFloat(KEY_MONEY, (float)Jew_Manager.instance.overallTotal);
                PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
                PlayerPrefs.Save();

                Debug.Log($"[SaveSystem] Gioco salvato su disco. Giorno{_currentDay}, Soldi: {Jew_Manager.instance.currentMoney}"); //da sostituire con un tmp_text

            }
        }
    }
}
