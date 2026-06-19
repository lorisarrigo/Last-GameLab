using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BTN_Manager : MonoBehaviour
{
    [SerializeField] GameObject Maingame;
    [SerializeField] GameObject MainGameCanva;
    [SerializeField] GameObject PauseMenu;

    public static event Action OnPause;
    public static event Action OnResume;

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
    public void QuitGame()
    {
        Application.Quit();
    }
}
