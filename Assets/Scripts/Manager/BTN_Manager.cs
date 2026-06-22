using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BTN_Manager : MonoBehaviour
{
    [SerializeField] GameObject Maingame;
    [SerializeField] GameObject MainGameCanva;
    [SerializeField] GameObject PauseMenu;

    [SerializeField] Button LoadBtn;
    [SerializeField] string mainMenuScene = "";

    public static event Action OnPause;
    public static event Action OnResume;

    private void Start()
    {
        CheckLoadButton();
    }
    public void NewGameBtn()
    {
        if(Save_Manager.instance != null) Save_Manager.instance.NewGame();
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
    public void LoadGame()
    {
        if (Save_Manager.instance != null) Save_Manager.instance.LoadGame();
    }
    void CheckLoadButton()
    {
        if(LoadBtn != null)
        {
            bool hasSave = PlayerPrefs.GetInt("HasSavedData", 0) == 1;
            LoadBtn.interactable = hasSave;
        }
    }
    public void Retry() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void MainMenu() { SceneManager.LoadScene(mainMenuScene); }
    public void QuitGame() { Application.Quit(); }
}
