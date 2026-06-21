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
        if (LoadBtn != null)
        {
            bool haSave = PlayerPrefs.GetInt("HasSavedData", 0) == 1;
            LoadBtn.interactable = haSave;
        }
    }
    public void LoadMainGame()
    {
        //SceneManager.LoadScene("Level1"); considerando il fatto che verrà usato anche nel mai questa funzione, per il momento usiamo la linea sotto 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Pause()
    {
        OnPause?.Invoke();
        MainGameCanva.SetActive(false);
        PauseMenu.SetActive(true);
    }
    public void Resume()
    {
        OnResume?.Invoke();
        MainGameCanva.SetActive(true);
        PauseMenu.SetActive(false);
    }
    public void LoadGame()
    {
        if (Save_Manager.instance != null)
        {
            Save_Manager.instance.LoadGame();
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
