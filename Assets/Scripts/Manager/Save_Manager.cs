using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Save_Manager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] string mainGameSceneName = "";

    [Header("Main Menu UI Buttons")]
    //[SerializeField] Button mainMenuContinueBtn;
    [SerializeField] Button loadBtn;

    [Header("Starting Settings")]
    [SerializeField] int startingMoney = 100;

    const string KEY_DAY = "SavedDay", KEY_MONEY = "SavedMoney", KEY_HAS_SAVE = "HasSavedData";
    //const string KEY_MONEY = "SavedMoney";
    //const string KEY_HAS_SAVE = "HasSavedData";

    public static Save_Manager instance;

    bool shouldLoadSavedData = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() { UpdateButtonInteractability(); }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    public void NewGame()
    {
        shouldLoadSavedData = false;
        SceneManager.LoadScene(mainGameSceneName);
    }
    public void LoadGame()
    {
        if(PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1)
        {
            shouldLoadSavedData = true;
            SceneManager.LoadScene(mainGameSceneName);
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == mainGameSceneName)
        {
            if(shouldLoadSavedData)
            {
                UI_Manager.instance.currentDay = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_DAY));
                UI_Manager.instance.currentMoney = Mathf.RoundToInt(PlayerPrefs.GetFloat(KEY_MONEY));
                Debug.Log($"[SaveSystem]Dati caricati con successo!");
            }
            else
            {
                UI_Manager.instance.currentDay = 0;
                UI_Manager.instance.currentMoney = startingMoney;
                Debug.Log("[SaveSystem] Nuova partita avviata con dati di default.");
            }
            UI_Manager.instance.todayGains = 0;
            UI_Manager.instance.todayTotal = 0;
            UI_Manager.instance.overallTotal = UI_Manager.instance.currentMoney;

            UI_Manager.instance.moneyCounter.text = UI_Manager.instance.currentMoney + " Æ";
        }
        else
        {
            UpdateButtonInteractability();
        }
    }
    public void SaveGame()
    {
        if (UI_Manager.instance != null)
        { 
            int _currentDay = UI_Manager.instance.currentDay;
            if(_currentDay > 0)
            {
                PlayerPrefs.SetFloat(KEY_DAY, (float)_currentDay);
                PlayerPrefs.SetFloat(KEY_MONEY, (float)UI_Manager.instance.currentMoney);
                PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
                PlayerPrefs.Save();

                Debug.Log($"[SaveSystem] Giocosalvato su disco. Giorno{_currentDay}, Soldi: {UI_Manager.instance.currentMoney}");
                UpdateButtonInteractability();
            }
        }
    }
    public void UpdateButtonInteractability()
    {
        bool hasSave = PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;

        if(loadBtn != null) loadBtn.interactable = hasSave;
    }
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
