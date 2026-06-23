using System.Collections.Generic;
using UnityEngine;

public class Jew_Manager : MonoBehaviour
{
    [Header("Wallet Data")]
    public int currentMoney;
    public int todayGains;
    public int todayExpanses;
    public int todayTotal;
    public int overallTotal;

    [Header("Planets Database")]
    [SerializeField] List<PlanetRequirements> planetDB = new();

    PlanetRequirements selPlanetData;
    bool isStamped = false;

    public static Jew_Manager instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }

    public void SelectPlanet(int planetIndex)
    {
        if (planetIndex >= 0 && planetIndex < planetDB.Count)
        {
            ApplyStampData(planetDB[planetIndex]);
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

    public void CalculatePassaportScore(PlanetRequirements planetData, PlanetRequirements npcRwq, float dummy, float maxPat)
    {
        selPlanetData = planetData;
        isStamped = true;
        DeliverAndCalculate();
    }

    public void ResetDailyGains()
    {
        todayGains = 0;
        todayExpanses = 0;
        todayTotal = 0;
    }
    void DeliverAndCalculate()
    {
        if (!isStamped) return;
        PlanetRequirements npcReq = NPC_Manager.instance.curRequirements;
        int required = 0;
        int guessed = 0;

        if (npcReq.temperature != Temperature.None) { required++; if (selPlanetData.temperature == npcReq.temperature) guessed++; }
        if (npcReq.lifeQuantity != LifeQuantity.None) { required++; if (selPlanetData.lifeQuantity == npcReq.lifeQuantity) guessed++; }
        if (npcReq.population != Population.None) { required++; if (selPlanetData.population == npcReq.population) guessed++; }
        if (npcReq.permanance != Permanance.None) { required++; if (selPlanetData.permanance == npcReq.permanance) guessed++; }
        if (npcReq.sector != Sector.None) { required++; if (selPlanetData.sector == npcReq.sector) guessed++; }

        ScoreResult result;
        if (guessed == required) result = ScoreResult.MaxScore;
        else if (guessed >= (required / 2f) && guessed != required) result = ScoreResult.Reduced;
        else result = ScoreResult.Failed;

        ProcessFinalScore(result);
    }
    void ProcessFinalScore(ScoreResult result)
    {
        float mult = UI_Manager.instance.GetPatienceMultiplier();
        float Y = 0;
        int addMoney = 0;
        string logResult = "";
        switch (result)
        {
            case ScoreResult.MaxScore:
                UI_Manager.instance.success = true;
                NPC_Manager.instance.curResult = "Perfect Evaluation";
                Y = 100;
                addMoney = Mathf.RoundToInt(Y * mult);

                currentMoney += addMoney;
                todayGains += addMoney;
                logResult = $"PERFECT  (added Money: {addMoney} Æ, total Money: {currentMoney} Æ)";
                UI_Manager.instance.ShowEvaluationResult(2, addMoney, logResult);
                break;

            case ScoreResult.Reduced:
                UI_Manager.instance.success = true;
                NPC_Manager.instance.curResult = "Partially Satisfied";
                Y = 50;
                addMoney = Mathf.RoundToInt(Y * mult);

                currentMoney += addMoney;
                todayGains += addMoney;
                logResult = $"PARTIAL  (added Money: {addMoney} Æ, total Money: {currentMoney} Æ)";
                UI_Manager.instance.ShowEvaluationResult(2, addMoney, logResult);
                break;

            case ScoreResult.Failed:
                UI_Manager.instance.success = false;
                NPC_Manager.instance.curResult = "Not Satisfied";
                logResult = $"FAIL  (added Money: 0 Æ total Money: {currentMoney} Æ)";
                UI_Manager.instance.ShowEvaluationResult(1, 0, logResult);
                break;
        }
    }
    public void CalculateEndDayExpanses(int currentDay)
    {
        todayExpanses = 125;
        int curExpanses = todayExpanses + (25 * currentDay);
        todayTotal = todayGains - curExpanses;
        overallTotal = currentMoney - curExpanses;
    }
}
