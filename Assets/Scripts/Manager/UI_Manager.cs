using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreResult { Failed, Reduced, MaxScore }

public class UI_Manager : MonoBehaviour
{
    [Header("Day & Economy Visuals")]
    int day;
    [SerializeField] TMP_Text dayCounter;
    public TMP_Text moneyCounter;
    public TMP_Text plusMoney;


    [Header("Balance Screen Visuals")]
    [SerializeField] TMP_Text todayGains_Txt;
    [SerializeField] TMP_Text todayExpanses_Txt;
    [SerializeField] TMP_Text todayTotal_Txt;
    [SerializeField] TMP_Text overallTotal_Txt;

    [Header("Request & Dialogue")]
    public TMP_Text requestTxtSpace;
    [SerializeField] List<string> entry = new();
    [SerializeField] TMP_Text logTxt;

    public int npc;

    [Header("Patience Visual Bars")]
    public float maxPatience;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;

    float curPatience = 0;
    bool isFilling = false;
    [HideInInspector] public bool success;

    [Header("fonts")]
    public TMP_FontAsset normal;
    public TMP_FontAsset alien;
    //eventi
    public static event Action OnDeliver;
    public static UI_Manager instance;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
    }
    private void OnEnable()
    {
        Game_Manager.OnDay += UpdateDate; 
        Game_Manager.OnPoint += UpdateGoal;
        Game_Manager.OnWinFB += Translate;
        NPC_Manager.OnRequest += UpdateRequest;
        NPC_Manager.OnTimer += StartTimer;
        Game_Manager.OnRefreshUI += RefreshUIFields;
    }
    private void OnDisable()
    {
        Game_Manager.OnDay -= UpdateDate; 
        Game_Manager.OnPoint -= UpdateGoal;
        Game_Manager.OnWinFB -= Translate;
        NPC_Manager.OnRequest -= UpdateRequest;
        NPC_Manager.OnTimer -= StartTimer;
        Game_Manager.OnRefreshUI -= RefreshUIFields;
    }
    private void Update()
    {
        if (isFilling && curPatience > 0)
        {
            curPatience -= Time.deltaTime;
            float fillRatio = curPatience / maxPatience;
            patienceBar.fillAmount = fillRatio;
            patienceBarFB.fillAmount = fillRatio;
            if (curPatience <= 0) TriggerFailure();
        }

    }
    public float GetPatienceMultiplier() { return curPatience / (maxPatience / 2); }
    void UpdateDate()
    {
        day = Game_Manager.instance.currentDay;
        dayCounter.text = "day \n n: " + (day < 10 ? "0" : "") + day;
    }
    void UpdateRequest()
    {
        //richiesta corrente
        if(NPC_Manager.instance.randomNPC < 7) requestTxtSpace.font = normal;
        else 
        {
            requestTxtSpace.font = alien;
            TicketController.instance.stampButton.SetActive(false);
        }
        requestTxtSpace.text = NPC_Manager.instance.curRequest;

        //log
        string log = $" - {NPC_Manager.instance.curClient} requested: {NPC_Manager.instance.curRequest}";
        entry.Add(log);
        logTxt.text = string.Join("\n", entry);
    }
    void StartTimer()
    {
        success = false;
        curPatience = maxPatience;
        patienceBar.fillAmount = 1;
        patienceBarFB.fillAmount = 1;
        patienceBar.gameObject.SetActive(true);
        isFilling = true;
        Jew_Manager.instance.RemoveStampData();
    }
    void Translate()
    {
        requestTxtSpace.font = normal;
        TicketController.instance.stampButton.SetActive(true);
        BTN_Manager.instance.TranslateBtn.interactable = false;
    }

    public void ShowEvaluationResult(int answerIndex, int moneyAdded, string logResult)
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        if (moneyAdded > 0) StartCoroutine(LerpTransparency(moneyAdded));

        string answer = NPC_Manager.instance.GetNPCAnswer(npc,answerIndex);
        /*answer-->*/requestTxtSpace.text = answer;

        string log = $" - {NPC_Manager.instance.curClient} is {logResult}";
        entry.Add(log);
        logTxt.text = string.Join("\n", entry);

        moneyCounter.text = Jew_Manager.instance.currentMoney + " Æ";
        OnDeliver?.Invoke();
    }

    void RefreshUIFields()
    {
        //balance screen
        todayGains_Txt.text = $"today gains: {Jew_Manager.instance.todayGains} Æ";
        int totalExpanses = Jew_Manager.instance.todayExpanses + (25 * day);
        todayExpanses_Txt.text= $"Today expenses: {totalExpanses} Æ";
        todayTotal_Txt.text = $"today Total: {Jew_Manager.instance.todayTotal} Æ";
        overallTotal_Txt.text = $"overall Total: {Jew_Manager.instance.overallTotal} Æ";

        //log window
        entry.Clear();
        logTxt.text = string.Empty;
    }
    void TriggerFailure()
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        Jew_Manager.instance.CalculatePassaportScore(new PlanetRequirements(), NPC_Manager.instance.curRequirements, 0, maxPatience);
        TicketController.instance.isAndato = false;
        TicketController.instance.oggettoUI.SetActive(false);
        TicketController.instance.stampButton.gameObject.SetActive(false);
        TicketController.instance.confirmBtn.gameObject.SetActive(false);
        TicketController.instance.OpenStamp.gameObject.SetActive(false);
        TicketController.instance.changeStamp.gameObject.SetActive(false);
        NPC_Manager.instance.Ticket.SetActive(false);
    }
    void UpdateGoal()
    {
        FB_Manager.instance.score.text = FB_Manager.instance.currentScore + "/" + FB_Manager.instance.Goal;
    }
    IEnumerator LerpTransparency(int mon)
    {
        plusMoney.text = $"+ {mon} Æ";
        Color plusMoneyCol = plusMoney.color;

        plusMoney.gameObject.SetActive(true);
        plusMoneyCol.a = 1;
        plusMoney.color = plusMoneyCol;

        float fadeDuration = 1.5f;
        float timer = 0f;

        while (timer < 1)
        {
            timer += Time.deltaTime / fadeDuration;


            plusMoneyCol.a = Mathf.Lerp(1, 0, timer);

            plusMoney.color = plusMoneyCol;
            yield return null;
        }
        plusMoney.gameObject.SetActive(false);
    }
    
}
