using System;
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
                patienceBar.gameObject.SetActive(false);
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
        FB_Manager.instance.score.text = FB_Manager.instance.currentScore + "/" + FB_Manager.instance.Goal;
    }
}
