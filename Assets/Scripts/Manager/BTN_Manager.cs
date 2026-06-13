using System;
using UnityEngine;

public class BTN_Manager : MonoBehaviour
{
    [SerializeField] GameObject Maingame;
    [SerializeField] GameObject MainGameCanva;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject PauseMenu;

    public static event Action OnPause;
    public static event Action OnResume;

    public void NewGame()
    {
        MainMenu.SetActive(false);
        Maingame.SetActive(true);
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
    public void Menu()
    {
        PauseMenu.SetActive(false);
        MainGameCanva.SetActive(true);
        MainMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
