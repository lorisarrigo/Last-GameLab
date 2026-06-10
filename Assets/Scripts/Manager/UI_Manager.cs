using System;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public int currentScore;
    public int Goal;
    [Header("Score")]
    [SerializeField] TMP_Text score;
    //eventi
    public static event Action OnGoal;

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

    private void UpdateScore()
    {
        currentScore++;
        if(currentScore == Goal)
        {
            OnGoal?.Invoke();
        }
    }
}
