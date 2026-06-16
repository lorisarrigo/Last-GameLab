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
        NPC_Manager.OnRequest += ChooseRequest;
        NPC_Manager.OnTimer += StartTimer;
        Game_Manager.OnDay += UpdateDayCounter;
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnRequest -= ChooseRequest;
        NPC_Manager.OnTimer -= StartTimer;
        Game_Manager.OnDay -= UpdateDayCounter;
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
    void ChooseRequest()
    {
        requestTxtSpace.text = NPC_Manager.instance.curRequest;
    }
    void StartTimer()
    {
        success = false;
        elapsed = patienceTimer;
        patienceBar.gameObject.SetActive(true);
        isFilling = true;
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
        if(currentDay < 10)
            dayCounter.text = "day " + "\n sn: 0" + currentDay;
        else
            dayCounter.text = "day " + "\n n: " + currentDay;
    }
}
