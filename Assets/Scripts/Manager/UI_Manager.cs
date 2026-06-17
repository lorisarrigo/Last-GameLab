using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreResult { Failed, Reduced, MaxScore }

public class UI_Manager : MonoBehaviour
{
    [Header("Day & Economy")]
    [HideInInspector] public int currentDay;
    [SerializeField] TMP_Text dayCounter;

    [Header("Requests")]
    public TMP_Text requestTxtSpace;
    [SerializeField] List<string> Answers = new();

    [SerializeField] List<string> entry = new();
    [SerializeField] TMP_Text logTxt;

    [Header("Patience")]
    [SerializeField] float patienceTimer;
    float elapsed = 0;
    bool isFilling = false;
    public bool success;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;

    [SerializeField] PlanetRequirements selPlanetData;
    [SerializeField] bool isStamped = false;

    //eventi
    //public static event Action OnFinishedTimer;
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
        //NPC_Manager.OnAnswer += UpdateAnswer;
        Game_Manager.OnDay += UpdateDayCounter;
        Game_Manager.OnDay += ClearLog;
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnRequest -= UpdateRequest;
        NPC_Manager.OnTimer -= StartTimer;
        //NPC_Manager.OnAnswer -= UpdateAnswer;
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
    public void ApplyStampData(PlanetRequirements planetData)
    {
        selPlanetData = planetData;
        isStamped = true;
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
        int requiredParameters = 0;
        int guessedParameters = 0;

        if (npcRequirements.temperature != Temperature.None)
        {
            requiredParameters++;
            if (selPlanetData.temperature == npcRequirements.temperature) guessedParameters++;
        }
        if (npcRequirements.lifeQuantity != LifeQuantity.None)
        {
            requiredParameters++;
            if (selPlanetData.lifeQuantity == npcRequirements.lifeQuantity) guessedParameters++;
        }
        if (npcRequirements.population != Population.None)
        {
            requiredParameters++;
            if (selPlanetData.population == npcRequirements.population) guessedParameters++;
        }
        if (npcRequirements.permanance != Permanance.None)
        {
            requiredParameters++;
            if (selPlanetData.permanance == npcRequirements.permanance) guessedParameters++;
        }
        if (npcRequirements.sector != Sector.None)
        {
            requiredParameters++;
            if (selPlanetData.sector == npcRequirements.sector) guessedParameters++;
        }
        ScoreResult result;
        if (guessedParameters == requiredParameters) result = ScoreResult.MaxScore;
        else if (guessedParameters >= (requiredParameters / 2f)) result = ScoreResult.Reduced;
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
    //public void ResolveClient(bool clientHappy)
    //{
    //    if (!isFilling) return;

    //    isFilling = false;
    //    success = clientHappy;
    //    patienceBar.gameObject.SetActive(false);

    //    if (success)
    //    {
    //        requestTxtSpace.text = Answers[2];
    //        OnDeliver?.Invoke();
    //    }
    //    else
    //    {
    //        TriggerFailure();
    //    }
    //}
    void TriggerFailure()
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        ProcessFinalScore(ScoreResult.Failed);
        //requestTxtSpace.text = Answers[1];
        //OnFinishedTimer?.Invoke();
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
