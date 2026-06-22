using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreResult { Failed, Reduced, MaxScore }

public class UI_Manager : MonoBehaviour
{
    public ScoreResult result;
    [Header("Day & Economy")]
    [HideInInspector] public int currentDay;
    [SerializeField] TMP_Text dayCounter;

    [Header("Requests")]
    public TMP_Text requestTxtSpace;
    [SerializeField] List<string> Answers = new();

    [SerializeField] List<string> entry = new();
    [SerializeField] TMP_Text logTxt;

    [Header("Planets")]
    [SerializeField] List<PlanetRequirements> planetDatabase = new();

    [Header("Patience")]
    [SerializeField] float patienceTimer;
    float elapsed = 0;
    bool isFilling = false;
    public bool success;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;

    [SerializeField] PlanetRequirements selPlanetData;
    [SerializeField] bool isStamped = false;
    public GameObject Timbro;

    //eventi
    public static event Action OnDeliver;

    public static UI_Manager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }
    private void OnEnable()
    {
        Game_Manager.OnPoint += UpdateGoal;
        NPC_Manager.OnRequest += UpdateRequest;
        NPC_Manager.OnTimer += StartTimer;
        Game_Manager.OnDay += UpdateDayCounter;
        Game_Manager.OnDay += ClearLog;
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnRequest -= UpdateRequest;
        NPC_Manager.OnTimer -= StartTimer;
        Game_Manager.OnDay -= UpdateDayCounter;
        Game_Manager.OnDay -= ClearLog;
    }
    private void Update()
    {
        if (isFilling && elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            patienceBar.fillAmount = elapsed / patienceTimer;
            patienceBarFB.fillAmount = elapsed / patienceTimer;
            if (elapsed <= 0)
            {
                TriggerFailure();
            }
        }
    }
    void UpdateRequest()
    {
        //richiesta corrente
        requestTxtSpace.text = NPC_Manager.instance.curRequest;
        
        //log
        string log = $" - {NPC_Manager.instance.curClient} requested: {NPC_Manager.instance.curRequest}";
        entry.Add(log);
        logTxt.text = string.Join("\n", entry);
    }
    void StartTimer()
    {
        success = false;
        elapsed = patienceTimer;
        patienceBar.fillAmount = 1;
        patienceBarFB.fillAmount = 1;
        patienceBar.gameObject.SetActive(true);
        isFilling = true;
        RemoveStampData();
    }
    public void SelectPlanet(int planetIndex)
    {
        if(planetIndex >= 0 && planetIndex < planetDatabase.Count)
        {
           //ApplyStampData(planetDatabase[planetIndex]);
        }
    }
    public void ApplyStampData(PlanetRequirements planetData)
    {
        selPlanetData = planetData;
        isStamped = true;
        DeliverAndCalculate();
    }
    public void RemoveStampData()
    {
        selPlanetData = new PlanetRequirements();
        isStamped = false;
    }
    public void DeliverAndCalculate()
    {
        if (!isStamped) return;
        PlanetRequirements npcRequirements = NPC_Manager.instance.curRequirements;

        int required = 0;
        int guessed = 0;
        if (npcRequirements.temperature != Temperature.None)
        {
            required++;
            if (selPlanetData.temperature == npcRequirements.temperature) guessed++;
        }
        if (npcRequirements.lifeQuantity != LifeQuantity.None)
        {
            required++;
            if (selPlanetData.lifeQuantity == npcRequirements.lifeQuantity) guessed++;
        }
        if (npcRequirements.population != Population.None)
        {
            required++;
            if (selPlanetData.population == npcRequirements.population) guessed++;
        }
        if (npcRequirements.permanance != Permanance.None)
        {
            required++;
            if (selPlanetData.permanance == npcRequirements.permanance) guessed++;
        }
        if (npcRequirements.sector != Sector.None)
        {
            required++;
            if (selPlanetData.sector == npcRequirements.sector) guessed++;
        }
        if (guessed == required) result = ScoreResult.MaxScore;
        else if (guessed >= (required / 2f) && guessed != required) result = ScoreResult.Reduced;
        else result = ScoreResult.Failed;

        ProcessFinalScore(result);
    }
    public void ProcessFinalScore(ScoreResult result)
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        string logResult = "";

        switch(result)
        {
            case ScoreResult.MaxScore:
                success = true;
                NPC_Manager.instance.curResult = "Perfect Evaluation";
                requestTxtSpace.text = Answers[2];
                logResult = "PERFECT  (Max Score)";
                break;
            case ScoreResult.Reduced:
                success = true;
                NPC_Manager.instance.curResult = "Partially Satisfied";
                requestTxtSpace.text = Answers[2];
                logResult = "PARTIAL  (Reduced Score)";
                break;
            case ScoreResult.Failed:
                success = false;
                NPC_Manager.instance.curResult = "Not Satisfied";
                requestTxtSpace.text = Answers[1];
                logResult = "FAIL  (0 Points)";
                break;
        }

        string log = $" - {NPC_Manager.instance.curClient} is {logResult}";
        entry.Add(log);
        logTxt.text = string.Join("\n", entry);
        OnDeliver?.Invoke();
    }

    void ClearLog()
    {
        entry.Clear();
        logTxt.text = string.Empty;
    }
    void TriggerFailure()
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        ProcessFinalScore(ScoreResult.Failed);
    }
    void UpdateGoal()
    {
        FB_Manager.instance.score.text = FB_Manager.instance.currentScore + "/" + FB_Manager.instance.Goal;
    }
    void UpdateDayCounter()
    {
        if (currentDay < 10)
            dayCounter.text = "day " + "\n n: 0" + currentDay;
        else
            dayCounter.text = "day " + "\n n: " + currentDay;
    }
    
}
