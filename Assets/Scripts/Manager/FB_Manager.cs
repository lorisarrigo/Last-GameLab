using TMPro;
using UnityEngine;

public class FB_Manager : MonoBehaviour
{
    

    [Header("Bird Things")]
    [Tooltip("the bird")]
    public GameObject bird;
    [Tooltip("the jumpForce of the bird")]
    public float jumpForce;
    [Tooltip("the rotation speed of the bird")]
    public float rotSpeed;
    [HideInInspector] public Vector2 startingBPos;

    [Header("tubes")]
    [Tooltip("The Tube Spawner Object")]
    public Tube_Spawner TS;
    [Tooltip("the size of the tubepool")]
    public int poolSize;
    [Tooltip("The Prefab to use for the Tubes")]
    public GameObject tubePrefab;
    [Tooltip("The spawn rate of the Tubes")]
    public float spawnRate;
    [Tooltip("The height spwn range (in negative & positive) of the Tubes")]
    public float heightOffset;
    [Tooltip("The speed of the Tubes")]
    public float speed;
    [Tooltip("The point in X where the Tubes deactivate")]
    public float hideInX;

    [Header("Score")]
    public TMP_Text score;
    [HideInInspector] public int currentScore;
    [HideInInspector] public int Goal;

    [Header("difficulties")]
    public int goal1;
    public int goal2;
    public int goal3;

    [Header("SFX")]
    public AudioClip win;
    public AudioClip hit;
    public static FB_Manager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        startingBPos = bird.transform.position;
    }
}
