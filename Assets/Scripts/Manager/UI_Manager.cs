using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public int currentScore;
    public int Goal;
    [Header("patience")]
    [SerializeField] float patienceTimer;
    float elapsed = 0;
    bool isFilling = false;
    public bool success;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;
    [Header("Score")]
    [SerializeField] TMP_Text score;


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
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
        NPC_Manager.OnTimer -= StartTimer;
    }
    private void Update()
    {
        if (isFilling && elapsed > 0)
        {
            if(success)
            {
                OnDeliver?.Invoke();
                patienceBar.gameObject.SetActive(false);
                return;
            }
            elapsed -= Time.deltaTime;
            patienceBar.fillAmount = elapsed / patienceTimer;
            patienceBarFB.fillAmount = elapsed / patienceTimer;
            if (elapsed <= 0)
            {
                isFilling = false;
                OnFinishedTimer?.Invoke();
            }
        }
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
        //questo serve per il minigame
        score.text = currentScore + "/" + Goal;
    }
}
