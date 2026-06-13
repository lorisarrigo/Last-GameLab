using System;
using UnityEngine;

public enum GameStates
{
    Flapping,
    MinigameGO,
    Running,
    Paused
}
public class Game_Manager : MonoBehaviour
{
    [Header("Thing to activate/deactivate")]
    [SerializeField] GameObject Translate_BTN;
    [SerializeField] GameObject Pause_BTN;
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject miniGame;


    GameStates maingame = GameStates.Running;

    int Difficulty;

    //eventi
    public static event Action OnPoint;
    void OnEnable()
    {
        Bird_Controller.OnPoint += UpdateScore;
        Bird_Controller.OnColl += GameOverMinigame;
        BTN_Manager.OnPause += PauseGame;
        BTN_Manager.OnResume += ResumeGame;
    }
    void OnDisable()
    {
        Bird_Controller.OnPoint -= UpdateScore;
        Bird_Controller.OnColl -= GameOverMinigame;
        BTN_Manager.OnPause -= PauseGame;
        BTN_Manager.OnResume -= ResumeGame;
    }

    void Update()
    {
        #region Game States
        switch (maingame)
        {
            case GameStates.Running:
                Time.timeScale = 1;
                break;
            case GameStates.Paused:
                Time.timeScale = 0;
                break;
            case GameStates.Flapping:
                mainGame.SetActive(false);
                miniGame.SetActive(true);
                break;
            case GameStates.MinigameGO:
                FB_Manager.instance.bird.transform.position = FB_Manager.instance.startingBPos;
                FB_Manager.instance.TS.ResetSpawner();
                miniGame.SetActive(false);
                mainGame.SetActive(true);
                break;
        }
        #endregion
        #region Minigame Difficulty
        switch (Difficulty)
        {
            case 0:
                FB_Manager.instance.Goal = FB_Manager.instance.goal1;
                OnPoint?.Invoke();
                break;
            case 1:
                FB_Manager.instance.Goal = FB_Manager.instance.goal2;
                OnPoint?.Invoke();
                break;
            case 2:
                FB_Manager.instance.Goal = FB_Manager.instance.goal3;
                OnPoint?.Invoke();
                break;
        }
        #endregion
    }
    public void Translate()
    {
        maingame = GameStates.Flapping;
        Difficulty = UnityEngine.Random.Range(0, 3);
    }
    void UpdateScore()
    {
        FB_Manager.instance.currentScore++;
        OnPoint?.Invoke();
        if (FB_Manager.instance.currentScore == FB_Manager.instance.Goal)
        {
            WinMinigame();
        }
    }
    void GameOverMinigame()
    {
        maingame = GameStates.MinigameGO;
        FB_Manager.instance.currentScore = 0;
        FB_Manager.instance.Goal = 0;
    }
    void WinMinigame()
    {
        maingame = GameStates.MinigameGO;
        FB_Manager.instance.currentScore = 0;
        FB_Manager.instance.Goal = 0;
    }
    void PauseGame()
    {
        maingame = GameStates.Paused;
    }
    void ResumeGame()
    {
        maingame = GameStates.Running;
    }
}
