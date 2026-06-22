using UnityEngine;
using UnityEngine.SceneManagement;


public class Save_Manager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] string sceneToLoad = "";

    [Header("Starting Settings")]
    [SerializeField] int startingMoney = 100;

    const string KEY_DAY = "SavedDay", KEY_MONEY = "SavedMoney", KEY_HAS_SAVE = "HasSavedData";

    bool shouldLoadSavedData = false;
    public static Save_Manager instance;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
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
                PlayerPrefs.SetFloat(KEY_DAY, (float)_currentDay);
                PlayerPrefs.SetFloat(KEY_MONEY, (float)Jew_Manager.instance.overallTotal);
                PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
                PlayerPrefs.Save();

                Debug.Log($"[SaveSystem] Gioco salvato su disco. Giorno{_currentDay}, Soldi: {Jew_Manager.instance.currentMoney}"); //da sostituire con un tmp_text

            }
        }
    }
}
