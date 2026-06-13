using System;
using UnityEngine;
public enum Minigame
{
    Flapping, 
    MinigameGO
    //nel caso serve aggiungiamo un'altro stato "Win"
}
public enum GameStates
{
    Flapping,
    MinigameGO,
    Running,
    Paused
}
public class Game_Manager : MonoBehaviour
{
    [SerializeField] Tube_Spawner TS;

    [Header("Minigame things")]
    [SerializeField] GameObject Translate_BTN;
    [SerializeField] GameObject miniGame;
    [SerializeField] GameObject bird;
    Vector2 startingBPos;

    [Header("difficoltà")]
    [SerializeField] int goal1;
    [SerializeField] int goal2;
    [SerializeField] int goal3;

    //Minigame minigame = Minigame.MinigameGO;
    public GameStates maingame = GameStates.Paused;

    int Difficulty;

    //eventi
    public static event Action OnPoint;
    void Start()
    {
        startingBPos = bird.transform.position;
    }
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
            case GameStates.Paused:
                Time.timeScale = 0;
                break;
            case GameStates.Running:
                Time.timeScale = 1.0f;
                break;
            case GameStates.Flapping:
                Time.timeScale = 1.0f;
                Translate_BTN.SetActive(false);
                miniGame.SetActive(true);
                break;
            case GameStates.MinigameGO:
                bird.transform.position = startingBPos;
                TS.ResetSpawner();
                miniGame.SetActive(false);
                Translate_BTN.SetActive(true);
                break;
        }
        #endregion
        #region Minigame Difficulty
        switch (Difficulty)
        {
            case 0:
                UI_Manager.instance.Goal = goal1;
                OnPoint?.Invoke();
                break;
            case 1:
                UI_Manager.instance.Goal = goal2;
                OnPoint?.Invoke();
                break;
            case 2:
                UI_Manager.instance.Goal = goal3;
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
        UI_Manager.instance.currentScore++;
        OnPoint?.Invoke();
        if (UI_Manager.instance.currentScore == UI_Manager.instance.Goal)
        {
            WinMinigame();
        }
    }
    void GameOverMinigame()
    {
        maingame = GameStates.MinigameGO;
        UI_Manager.instance.currentScore = 0;
        UI_Manager.instance.Goal = 0;
    }
    void WinMinigame()
    {
        maingame = GameStates.MinigameGO;
        UI_Manager.instance.currentScore = 0;
        UI_Manager.instance.Goal = 0;
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
