using System;
using System.Collections.Generic;
using UnityEngine;
public enum Minigame
{
    Running, 
    GameOver,
    //nel caso serve aggiungiamo un'altro stato "Win"
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

    Minigame minigame = Minigame.GameOver;

    int Difficulty;

    //eventi
    //public static event Action OnUpdGoal;
    public static event Action OnPoint;
    private void Start()
    {
        startingBPos = bird.transform.position;
    }
    private void OnEnable()
    {
        Bird_Controller.OnPoint += UpdateScore;
        Bird_Controller.OnColl += GameOverMinigame;
    }
    private void OnDisable()
    {
        Bird_Controller.OnPoint -= UpdateScore;
        Bird_Controller.OnColl -= GameOverMinigame;
    }

    private void Update()
    {
        #region Minigame States
        switch (minigame)
        { 
            case Minigame.Running:
                Time.timeScale = 1.0f;
                Translate_BTN.SetActive(false);
                miniGame.SetActive(true);
                break;
            case Minigame.GameOver:
                Time.timeScale = 0;
                bird.transform.position = startingBPos;
                TS.ResetSpawner();
                miniGame.SetActive(false);
                Translate_BTN.SetActive(true);
                break;
        }
        #endregion
        #region minigame difficulty
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
        minigame = Minigame.Running;
        Difficulty = UnityEngine.Random.Range(0, 3);
    }
    private void UpdateScore()
    {
        UI_Manager.instance.currentScore++;
        OnPoint?.Invoke();
        if (UI_Manager.instance.currentScore == UI_Manager.instance.Goal)
        {
            WinMinigame();
        }
    }
    private void GameOverMinigame()
    {
        minigame = Minigame.GameOver;
        UI_Manager.instance.currentScore = 0;
        UI_Manager.instance.Goal = 0;
    }
    private void WinMinigame()
    {
        minigame = Minigame.GameOver;
        UI_Manager.instance.currentScore = 0;
        UI_Manager.instance.Goal = 0;
    }
}
