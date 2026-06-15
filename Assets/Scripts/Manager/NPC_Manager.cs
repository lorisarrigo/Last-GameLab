using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC")]
    public int clientToday;
    public GameObject NPC;
    [SerializeField] List<Material> NPC_Mat = new();
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;

    public int curWaypointIndex;
    Vector3 startPos;

    bool canMove;
    bool clientResolved;
    [Header("Requests")]
    [SerializeField] List<string> Requests = new();
    public string curRequest;
    [Header("Ticket")]
    [SerializeField] GameObject Ticket;
    [SerializeField] float ticketSpeed;
    [SerializeField] Transform ticketDeskPos;

    //eventi
    public static event Action OnRequest;
    public static event Action OnTimer;
    public static event Action OnEndDay;

    public static NPC_Manager instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
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
    public void StartDay(int clients)
    {
        StartCoroutine(DailyLoop(clients));
    }

    IEnumerator DailyLoop(int nClients)
    {
        while (nClients >0)
        {
            RandomClient();
            canMove = false;
            clientResolved = false;
            StartCoroutine(MoveNPC(Waypoints[0], Waypoints[1]));

            yield return new WaitUntil(() => canMove);

            StartCoroutine(MoveTicket());
            OnRequest?.Invoke();

            yield return new WaitUntil(() => clientResolved);

            Ticket.SetActive(false);

            if (UI_Manager.instance.success)
            {
                StartCoroutine(MoveNPC(Waypoints[1], Waypoints[2]));
            }
            else
            {
                StartCoroutine(MoveNPC(Waypoints[1], Waypoints[0]));
            }
            yield return new WaitForSeconds(3);
            nClients--;
        }
        OnEndDay?.Invoke();
    }
    void RandomClient()
    {
        Renderer npc = NPC.GetComponent<Renderer>();
        if (clientToday > NPC_Mat.Count) clientToday = NPC_Mat.Count;
        int randomNPC = UnityEngine.Random.Range(0, clientToday);
        switch (randomNPC)
        {
            case 0:
                npc.material = NPC_Mat[0];
                curRequest = Requests[0];
                break;
            case 1:
                npc.material = NPC_Mat[1];
                curRequest = Requests[1];
                break;
            case 2:
                npc.material = NPC_Mat[2];
                curRequest = Requests[2];
                break;
            case 3:
                npc.material = NPC_Mat[3];
                curRequest = Requests[3];
                break;
        }
    }
    IEnumerator MoveNPC(GameObject startP, GameObject endP)
    {
        float t = 0;
        NPC.transform.position = startP.transform.position;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            NPC.transform.position = Vector3.Lerp(startP.transform.position, endP.transform.position, t);
            yield return null;
        }

        if (endP == Waypoints[1]) canMove = true;
    }

    IEnumerator MoveTicket()
    {
        Ticket.transform.position = Waypoints[1].transform.position;
        Ticket.SetActive(true);
        float t =  0;
        while (t < 1)
        {
            t += Time.deltaTime * ticketSpeed;
            Ticket.transform.position = Vector3.Lerp(Waypoints[1].transform.position, ticketDeskPos.position, t);
            yield return null;
        }
        OnTimer?.Invoke();
    }
    void NoPatience()
    {
        clientResolved = true;
    }
    void Delivered()
    {
        clientResolved = true;
    }
}
