using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [Header("Day & Economy")]
    [HideInInspector] public int currentDay;
    [SerializeField] TMP_Text dayCounter;

    [Header("NPC & Requests")]
    [SerializeField] int clientLimit;

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

    //eventi
    public static event Action OnFinishedTimer;
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
        NPC_Manager.OnAnswer += UpdateAnswer;
        Game_Manager.OnDay += UpdateDayCounter;
        Game_Manager.OnDay += ClearLog;
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnRequest -= UpdateRequest;
        NPC_Manager.OnTimer -= StartTimer;
        NPC_Manager.OnAnswer -= UpdateAnswer;
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
        patienceBar.gameObject.SetActive(true);
        isFilling = true;
    }
    void UpdateAnswer()
    {
        string log = $" - {NPC_Manager.instance.curClient} is {NPC_Manager.instance.curResult}";
        entry.Add(log);
        logTxt.text = string.Join("\n", entry);
    }
    void ClearLog()
    {
        entry.Clear();
        logTxt.text = string.Empty;
    }
    public void ResolveClient(bool clientHappy)
    {
        if (!isFilling) return;

        isFilling = false;
        success = clientHappy;
        patienceBar.gameObject.SetActive(false);

        if (success)
        {
            requestTxtSpace.text = Answers[2];
            OnDeliver?.Invoke();
        }
        else
        {
            TriggerFailure();
        }
    }
    void TriggerFailure()
    {
        isFilling = false;
        patienceBar.gameObject.SetActive(false);
        requestTxtSpace.text = Answers[1];
        OnFinishedTimer?.Invoke();
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
