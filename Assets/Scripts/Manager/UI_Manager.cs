using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [Header("patience")]
    [SerializeField] float patienceTimer;
    float elapsed = 0;
    bool isFilling = false;
    public bool success;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;

    [Header("dialogue")]
    [SerializeField] TMP_Text requestTxtSpace;
    [SerializeField] List<string> Request = new ();
    [SerializeField] List<string> Answers = new ();
    bool requestChoosed = false;

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
        NPC_Manager.OnTimer += StartTimer;
        NPC_Manager.OnRequest += ChooseRequest;
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnTimer -= StartTimer;
        NPC_Manager.OnRequest -= ChooseRequest;
    }
    private void Update()
    {
        if (isFilling && elapsed > 0)
        {
            if(success)
            {
                OnDeliver?.Invoke();
                patienceBar.gameObject.SetActive(false);
                requestTxtSpace.text = Answers[2];
                return;
            }
            elapsed -= Time.deltaTime;
            patienceBar.fillAmount = elapsed / patienceTimer;
            patienceBarFB.fillAmount = elapsed / patienceTimer;
            if (elapsed <= 0)
            {
                isFilling = false;
                patienceBar.gameObject.SetActive(false);
                OnFinishedTimer?.Invoke();
                requestTxtSpace.text = Answers[1];
            }
        }
    }
    void ChooseRequest()
    {
        if (!requestChoosed)
        {
            int randomRequest = UnityEngine.Random.Range(1, 5);
            switch (randomRequest)
            {
                case 0:
                    requestTxtSpace.text = Request[1];
                    break;
                case 1:
                    requestTxtSpace.text = Request[2];
                    break;
                case 2:
                    requestTxtSpace.text = Request[3];
                    break;
                case 3:
                    requestTxtSpace.text = Request[4];
                    break;
            }
        }
        requestChoosed = true;
    }
    void StartTimer()
    {
        if (!isFilling)
        { 
            patienceBar.gameObject.SetActive(true);
            elapsed = patienceTimer;
        }
        isFilling = true;
    }
    void UpdateGoal()
    {
        FB_Manager.instance.score.text = FB_Manager.instance.currentScore + "/" + FB_Manager.instance.Goal;
    }
}
