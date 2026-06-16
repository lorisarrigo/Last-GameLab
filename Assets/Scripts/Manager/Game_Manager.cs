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
    [Header("Things to activate/deactivate")]
    [SerializeField] GameObject Translate_BTN;
    [SerializeField] GameObject Pause_BTN;
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject miniGame;

    [Header("Day variables")]
    //public int currentDay;
    public int baseClients;
    public int clientToAdd;

    GameStates maingame = GameStates.Running;

    int Difficulty;

    //eventi
    public static event Action OnPoint;
    public static event Action OnDay;
    private void Start()
    {
        StartFlow();
    }
    void OnEnable()
    {
        NPC_Manager.OnEndDay += EndDay;
        Bird_Controller.OnPoint += UpdateScore;
        Bird_Controller.OnColl += GameOverMinigame;
        BTN_Manager.OnPause += PauseGame;
        BTN_Manager.OnResume += ResumeGame;
    }
    void OnDisable()
    {
        NPC_Manager.OnEndDay -= EndDay;
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
    void StartFlow()
    {
        NPC_Manager.instance.clientToday = baseClients + (UI_Manager.instance.currentDay * clientToAdd);
        Debug.Log("giorno: " + UI_Manager.instance.currentDay + " con " + NPC_Manager.instance.clientToday + " clienti");
        NPC_Manager.instance.StartDay(NPC_Manager.instance.clientToday);
    }
    void EndDay()
    {
        Debug.Log("Giornata Finita");
        UI_Manager.instance.currentDay++;
        OnDay?.Invoke();
        StartFlow();
    }
    #region minigame
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
#endregion
    void PauseGame()
    {
        maingame = GameStates.Paused;
    }
    void ResumeGame()
    {
        maingame = GameStates.Running;
    }
}
