using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public int currentScore;
    public int Goal;
    [Header("Score")]
    [SerializeField] TMP_Text score;

    public static UI_Manager instance;
    
    private void Awake()
    {
        if(instance != null)
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
    private void UpdateGoal()
    {
        score.text = currentScore + "/" + Goal;
    }
}
