using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BTN_Manager : MonoBehaviour
{
    [SerializeField] GameObject Maingame;
    [SerializeField] GameObject MainGameCanva;
    [SerializeField] GameObject PauseMenu;

    [SerializeField] TMP_Text jokingFrase;
    [SerializeField] string saveFrase;
    [SerializeField] Button LoadBtn;
    public Button TranslateBtn;

    [SerializeField] string mainMenuScene = "";

    public static event Action OnPause;
    public static event Action OnResume;
    public static BTN_Manager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
            instance = this;
    }

    private void Start()
    {
        CheckLoadButton();
    }
    private void OnEnable()
    {
        NPC_Manager.OnRequest += Translatable;
    }
    private void OnDisable()
    {
        NPC_Manager.OnRequest -= Translatable;
    }
    public void NewGameBtn()
    {
        if (Save_Manager.instance != null) Save_Manager.instance.NewGame();
    }
    void Translatable()
    {
        int currentNPCIndex = NPC_Manager.instance.randomNPC;

        bool cantranslate = currentNPCIndex >= 7;

        TranslateBtn.interactable = cantranslate;
    }
    public void Pause()
    {
        OnPause?.Invoke();
        MainGameCanva.SetActive(false);
        PauseMenu.SetActive(true);
        CheckLoadButton();
    }
    public void Resume()
    {
        OnResume?.Invoke();
        MainGameCanva.SetActive(true);
        PauseMenu.SetActive(false);
    }
    public void FalseSave()
    {
        jokingFrase.gameObject.SetActive(true);
        StartCoroutine(LerpTransparency());
    }
    public void LoadGame()
    {
        if (Save_Manager.instance != null) Save_Manager.instance.LoadGame();
    }
    void CheckLoadButton()
    {
        if (LoadBtn != null)
        {
            bool hasSave = PlayerPrefs.GetInt("HasSavedData", 0) == 1;
            LoadBtn.interactable = hasSave;
        }
    }
    public void Retry() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void MainMenu() { SceneManager.LoadScene(mainMenuScene); }
    public void QuitGame() { Application.Quit(); }

    IEnumerator LerpTransparency()
    {
        jokingFrase.text = saveFrase;
        Color plusMoneyCol = jokingFrase.color;

        jokingFrase.gameObject.SetActive(true);
        plusMoneyCol.a = 1;
        jokingFrase.color = plusMoneyCol;

        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime / fadeDuration;

            plusMoneyCol.a = Mathf.Lerp(1, 0, timer);

            jokingFrase.color = plusMoneyCol;
            yield return null;
        }
        jokingFrase.gameObject.SetActive(false);
    }
}
