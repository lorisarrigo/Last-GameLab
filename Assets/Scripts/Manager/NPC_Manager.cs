using System;
using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [SerializeField] GameObject NPC;
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;

    public int curWaypointIndex;
    float t;
    Vector3 startPos;

    bool canMove;
    bool failed;

    //eventi
    public static event Action OnTimer;
    void Start()
    {
        startPos = Waypoints[0].transform.position;
    }
    private void OnEnable()
    {
        UI_Manager.OnFinishedTimer += NoPatience;
        UI_Manager.OnDeliver += Delivered;
    }
    private void OnDisable()
    {
        UI_Manager.OnFinishedTimer -= NoPatience;
        UI_Manager.OnDeliver -= Delivered;
    }

    void Update()
    {
        if (Waypoints.Length == 0 || curWaypointIndex >= Waypoints.Length) return;

        Vector3 tPos = Waypoints[curWaypointIndex].transform.position;
        t += Time.deltaTime * speed;

        if (curWaypointIndex == 2 && (!canMove))
        {
            t = 0;
            OnTimer?.Invoke();
            return;
        }

        if (failed)
            tPos = Waypoints[0].transform.position;

        NPC.transform.position = Vector3.Lerp(startPos, tPos, t);

        if(t >= 1)
        {
            t = 0;
            startPos = tPos;
            curWaypointIndex++;
        }
    }
    void NoPatience()
    {
        failed = true;
        canMove = true;
    }
    void Delivered()
    {
        canMove = true;
    }
}
