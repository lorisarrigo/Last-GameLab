using System;
using UnityEngine;

public enum GameStates { Flapping, MinigameGO, Running, Paused }
public class Game_Manager : MonoBehaviour
{
    [Header("Things to activate/deactivate")]
    [SerializeField] GameObject Translate_BTN;
    [SerializeField] GameObject Pause_BTN;
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject miniGame;
    [SerializeField] GameObject gameOverScreen;

    [Header("Day variables")]
    public int currentDay;
    public int baseClients;
    public int clientToAdd;

    [Header("Balance")]
    [SerializeField] GameObject balance_Pannel;
    GameStates maingame = GameStates.Running;

    int Difficulty;

    //eventi
    public static event Action OnPoint;
    public static event Action OnDay;

    public static Game_Manager instance;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
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
            case GameStates.Running: Time.timeScale = 1; break;
            case GameStates.Paused: Time.timeScale = 0; break;
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
    }
    public void StartDayBTN()
    {
        if (Jew_Manager.instance.overallTotal > 0)
        {
            Jew_Manager.instance.currentMoney = Jew_Manager.instance.overallTotal;
            UI_Manager.instance.moneyCounter.text = Jew_Manager.instance.currentMoney + " Æ";

            if (Save_Manager.instance != null)
            {
                Save_Manager.instance.SaveGame();
            }

            StartFlow();
            balance_Pannel.SetActive(false);
            Jew_Manager.instance.ResetDailyGains();
        }
        else
        {
            balance_Pannel.SetActive(false);
            gameOverScreen.SetActive(true);
        }
    }
    void StartFlow()
    {
        NPC_Manager.instance.clientToday = baseClients + (currentDay * clientToAdd);
        NPC_Manager.instance.StartDay(NPC_Manager.instance.clientToday);
    }
    void EndDay()
    {
        balance_Pannel.SetActive(true);
        currentDay++;
        Jew_Manager.instance.CalculateEndDayExpanses(currentDay);
        OnDay?.Invoke();
    }
    #region minigame
    public void Translate()
    {
        maingame = GameStates.Flapping;
        Difficulty = UnityEngine.Random.Range(0, 3);
        switch (Difficulty)
        {
            case 0: FB_Manager.instance.Goal = FB_Manager.instance.goal1; break;
            case 1: FB_Manager.instance.Goal = FB_Manager.instance.goal2; break;
            case 2: FB_Manager.instance.Goal = FB_Manager.instance.goal3; break;
        }
        OnPoint?.Invoke();
    }
    void UpdateScore()
    {
        FB_Manager.instance.currentScore++;
        OnPoint?.Invoke();
        if (FB_Manager.instance.currentScore == FB_Manager.instance.Goal) WinMinigame();
    }
    void GameOverMinigame()
    {
        ResetMG();
    }
    void WinMinigame()
    {
        ResetMG();
    }
    void ResetMG()
    {
        maingame = GameStates.MinigameGO;
        FB_Manager.instance.currentScore = 0;
        FB_Manager.instance.Goal = 0;
    }
    #endregion
    void PauseGame() { maingame = GameStates.Paused; }
    void ResumeGame() { maingame = GameStates.Running; }
}
