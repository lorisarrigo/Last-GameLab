using System;
using UnityEngine;

public class BTN_Manager : MonoBehaviour
{
    [SerializeField] GameObject Maingame;
    [SerializeField] GameObject MainGameCanva;
    [SerializeField] GameObject PauseMenu;

    public static event Action OnPause;
    public static event Action OnResume;
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
