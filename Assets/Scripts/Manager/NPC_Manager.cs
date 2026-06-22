using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Temperature {None, Hot, Tempered, Cold }
public enum LifeQuantity {None, Bountyful, Present, Little }
public enum Population {None, Monster, Indigenous, Gods }
public enum Permanance {None, Week, Months, Years }
public enum Sector {None, Alpha, Beta, Gamma }

[System.Serializable]
public struct PlanetRequirements
{
    public Temperature temperature;
    public LifeQuantity lifeQuantity;
    public Population population;
    public Permanance permanance;
    public Sector sector;

    public PlanetRequirements(Temperature temp = Temperature.None, LifeQuantity lifeQ = LifeQuantity.None, Population pop = Population.None, Permanance per = Permanance.None, Sector sec = Sector.None)
    {
        temperature = temp;
        lifeQuantity = lifeQ;
        population = pop;
        permanance = per;
        sector = sec;
    }
}

public class NPC_Manager : MonoBehaviour
{
    bool canMove;
    bool clientResolved;

    [Header("NPC & Requests")]
    public int clientToday; 
    public GameObject NPC;
    [SerializeField] List<Sprite> NPC_Sprite = new();
    [SerializeField] List<string> Requests = new();
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;


    [HideInInspector] public string curRequest;
    [HideInInspector] public string curClient;
    public string curResult;

    [Header("Ticket")]
    [SerializeField] GameObject Ticket;
    [SerializeField] float ticketSpeed;
    [SerializeField] Transform ticketDeskPos;

    [Header("Current Client Requirements")]
    public PlanetRequirements curRequirements;
    public List<PlanetRequirements> clientDatabase = new();

    //eventi
    public static event Action OnRequest;
    public static event Action OnTimer;
    public static event Action OnAnswer;
    public static event Action OnEndDay;

    public static NPC_Manager instance;
    void Awake()
    {
        if (instance != null) { Destroy(this); return; }
        instance = this;
    }

    private void OnEnable() { UI_Manager.OnDeliver += Delivered; }
    private void OnDisable() { UI_Manager.OnDeliver -= Delivered; }
    public void StartDay(int clients) { StartCoroutine(DailyLoop(clients)); }

    IEnumerator DailyLoop(int nClients)
    {
        while (nClients > 0)
        {
            RandomClient();
            canMove = false;
            clientResolved = false;
            StartCoroutine(MoveNPC(Waypoints[0], Waypoints[1]));

            yield return new WaitUntil(() => canMove);

            StartCoroutine(MoveTicket());
            OnRequest?.Invoke();

            yield return new WaitUntil(() => clientResolved);
            OnAnswer?.Invoke();         
            Ticket.SetActive(false);    

            if (UI_Manager.instance.success) StartCoroutine(MoveNPC(Waypoints[1], Waypoints[2]));
            else StartCoroutine(MoveNPC(Waypoints[1], Waypoints[0]));

            yield return new WaitForSeconds(3);
            nClients--;
        }
        OnEndDay?.Invoke();
    }

    void RandomClient()
    {
        SpriteRenderer npc = NPC.GetComponent<SpriteRenderer>();
        if (clientToday > NPC_Sprite.Count) clientToday = NPC_Sprite.Count;
        int randomNPC = UnityEngine.Random.Range(0, clientToday);

        npc.sprite = NPC_Sprite[randomNPC];

        if(randomNPC<Requests.Count)
        {
            curRequest = Requests[randomNPC];
            if (randomNPC < clientDatabase.Count) curRequirements = clientDatabase[randomNPC];
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
    void Delivered() { clientResolved = true; }
}
