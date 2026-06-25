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
    [SerializeField] int baseClients;
    [SerializeField] int maxClient;
    public int currentDay;

    [Header("Balance")]
    [SerializeField] GameObject balance_Pannel;
    GameStates maingame = GameStates.Running;

    public int Difficulty;

    [Header("SFX")]
    public AudioClip flipPages;

    //eventi
    public static event Action OnDay;
    public static event Action OnPoint;
    public static event Action OnRefreshUI;
    public static event Action OnWinFB;

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
    public void PlayPages()
    {
        SFX_Manager.instance.PlaySfx(flipPages);
    }
    public void StartDayBTN()
    {
        if (Jew_Manager.instance.overallTotal > 0)
        {
            NPC_Manager.instance.clientLeft = 1;
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
        OnDay?.Invoke();
        NPC_Manager.instance.clientToday = baseClients + currentDay;
        if (NPC_Manager.instance.clientToday > maxClient) NPC_Manager.instance.clientToday = maxClient;
        NPC_Manager.instance.clientType = Mathf.RoundToInt(baseClients + (currentDay/2));
        if(currentDay >= 8)
        {
            FB_Manager.instance.speed += 0.25f * (currentDay - 8f);
            FB_Manager.instance.spawnRate /= 1 + (0.05f * (currentDay - 7f));
        }
        UI_Manager.instance.maxPatience /= 1 + (0.05f * currentDay);
        NPC_Manager.instance.StartDay(NPC_Manager.instance.clientToday);
    }
    void EndDay()
    {
        balance_Pannel.SetActive(true);
        currentDay++;
        Jew_Manager.instance.CalculateEndDayExpanses(currentDay);
        OnRefreshUI?.Invoke();
    }
    #region minigame
    public void Translate()
    {
        maingame = GameStates.Flapping;

        switch (NPC_Manager.instance.randomNPC)
        {
            case 7: FB_Manager.instance.Goal = FB_Manager.instance.goal1; break;
            case 8: FB_Manager.instance.Goal = FB_Manager.instance.goal2; break;
            case 9: FB_Manager.instance.Goal = FB_Manager.instance.goal3; break;
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
        SFX_Manager.instance.PlaySfx(FB_Manager.instance.hit);

    }
    void WinMinigame()
    {
        SFX_Manager.instance.PlaySfx(FB_Manager.instance.win);
        ResetMG();
        OnWinFB?.Invoke();
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
