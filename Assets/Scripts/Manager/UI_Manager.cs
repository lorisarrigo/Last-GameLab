using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public int currentScore;
    public int Goal;
    [Header("patience")]
    [SerializeField] float patienceTimer;
    public float elapsed = 0;
    public bool isFilling = false;
    [SerializeField] Image patienceBar;
    [SerializeField] Image patienceBarFB;
    [Header("Score")]
    [SerializeField] TMP_Text score;

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
    }
    private void OnDisable()
    {
        Game_Manager.OnPoint -= UpdateGoal;
    }
    private void Start()
    {
        elapsed = patienceTimer;
    }
    private void Update()
    {
        if (isFilling && elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            patienceBar.fillAmount = elapsed / patienceTimer;
            patienceBarFB.fillAmount = elapsed / patienceTimer;
            if (elapsed <= 0)
                isFilling = false;
        }
    }
    private void UpdateGoal()
    {
        //questo serve per il minigame
        score.text = currentScore + "/" + Goal;
    }
}
