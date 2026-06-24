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

[System.Serializable]
public struct NPCAnswers
{
    [TextArea(2, 4)]
    public string satisfiedAnswer;
    [TextArea(2, 4)]
    public string neutralAnswer;
    [TextArea(2, 4)]
    public string unsatisfiedAnswer;
}

public class NPC_Manager : MonoBehaviour
{
    bool canMove;
    bool clientResolved;

    [Header("NPC & Requests")]
    public int clientToday;
    public int clientType;
    public GameObject NPC;
    [SerializeField] List<Sprite> NPC_Sprite = new();
    [SerializeField] List<string> Requests = new();
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;
    public int randomNPC;

    [HideInInspector] public string curRequest;
    [HideInInspector] public string curClient;
    public string curResult;

    [Header("Ticket")]
    public GameObject Ticket;
    [SerializeField] float ticketSpeed;
    [SerializeField] Transform ticketDeskPos;

    [Header("Current Client Requirements")]
    public PlanetRequirements curRequirements;
    public List<PlanetRequirements> clientDatabase = new();
    [Header("Answers")]
    [SerializeField] NPCAnswers[] NPC_answers;
    public string GetNPCAnswer(int npc, int satisfaction)
    {
        if(npc < 0 || npc >= NPC_answers.Length)
        {
            Debug.LogError("indice fuori dal limite");
            return"...";
        }
        NPCAnswers curNPC = NPC_answers[npc];
        switch (satisfaction)
        {
            case 2:
                return curNPC.satisfiedAnswer;
            case 1:
                return curNPC.neutralAnswer;
            case 0:
                return curNPC.unsatisfiedAnswer;
            default:
                Debug.LogWarning("Punteggio non riconosciuto!");
                return "...";
        }
    }
    //eventi
    public static event Action OnRequest;
    public static event Action OnTimer;
    public static event Action OnEndDay;

    public static NPC_Manager instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
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
        if (clientType > NPC_Sprite.Count) clientType = NPC_Sprite.Count;
        randomNPC = UnityEngine.Random.Range(0, clientType);

        npc.sprite = NPC_Sprite[randomNPC];

        if(randomNPC<Requests.Count)
        {
            UI_Manager.instance.npc = randomNPC;
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
        TicketController.instance.confirmBtn.SetActive(false);
        TicketController.instance.stampButton.SetActive(false);
        Ticket.transform.position = Waypoints[1].transform.position;
        Ticket.SetActive(true);
        float t =  0;
        while (t < 1)
        {
            t += Time.deltaTime * ticketSpeed;
            Ticket.transform.position = Vector3.Lerp(Waypoints[1].transform.position, ticketDeskPos.position, t);
            yield return null;
        }
        if(UI_Manager.instance.requestTxtSpace.font != UI_Manager.instance.alien)TicketController.instance.stampButton.SetActive(true);

        OnTimer?.Invoke();
    }
    void Delivered() { clientResolved = true; }
}