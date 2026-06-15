using System;
using System.Collections;
using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] GameObject NPC;
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;

    public int curWaypointIndex;
    float t;
    Vector3 startPos;

    bool canMove;
    bool failed;

    [Header("Ticket")]
    [SerializeField] GameObject Ticket;
    [SerializeField] float ticketSpeed;
    [SerializeField] Transform ticketDeskPos;
    float ticketS;

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

        if (curWaypointIndex == 2 && !canMove)
        {
            t = 0;
            MoveTicket();
            if(Ticket.transform.position == ticketDeskPos.position)
                OnTimer?.Invoke();
            return;
        }

        if (failed)
        {
            tPos = Waypoints[0].transform.position;
            Ticket.SetActive(false);
            Ticket.transform.position = Waypoints[1].transform.position;
        }

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
    void MoveTicket()
    {
        Ticket.SetActive(true);

        ticketS += Time.deltaTime * ticketSpeed;

        Ticket.transform.position = Vector3.Lerp(Waypoints[1].transform.position, ticketDeskPos.position, ticketS);
    }
}
