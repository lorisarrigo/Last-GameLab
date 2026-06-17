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
    public int clientToday; //aggiungere una variabile di storage per i clienti del giorno perché se no rimangono 10 al giorno
    public GameObject NPC;
    [SerializeField] List<Material> NPC_Mat = new();
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

    //parameters
    public List<PlanetRequirements> clientDatabase = new();
    //public bool hot, tempered, cold;
    //public bool bountyful, present, none;
    //public bool monsters, indigenous, gods;
    //public bool week, months, years;
    //public bool alpha, beta, gamma;

    //eventi
    public static event Action OnRequest;
    public static event Action OnTimer;
    public static event Action OnAnswer;
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
    private void OnEnable()
    {
        UI_Manager.OnDeliver += Delivered;
    }
    private void OnDisable()
    {
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
            //ResetParameters();
            RandomClient();
            canMove = false;
            clientResolved = false;
            StartCoroutine(MoveNPC(Waypoints[0], Waypoints[1]));

            yield return new WaitUntil(() => canMove);

            StartCoroutine(MoveTicket());
            OnRequest?.Invoke();

            yield return new WaitUntil(() => clientResolved);
            OnAnswer?.Invoke();         // da spostare quando daremo 
            Ticket.SetActive(false);    // il biglietto trascinando sull'NPC

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

    //void ResetParameters()
    //{
    //    hot = false;
    //    tempered = false;
    //    cold = false;
    //    bountyful = false;
    //    present = false;
    //    none = false;
    //    monsters = false;
    //    indigenous = false;
    //    gods = false;
    //    week = false;
    //    months = false;
    //    years = false;
    //    alpha = false;
    //    beta = false;
    //    gamma = false;
    //}
    void RandomClient()
    {
        Renderer npc = NPC.GetComponent<Renderer>();
        if (clientToday > NPC_Mat.Count) clientToday = NPC_Mat.Count;
        int randomNPC = UnityEngine.Random.Range(0, clientToday);

        npc.material = NPC_Mat[randomNPC];

        curClient = NPC_Mat[randomNPC].name;

        if(randomNPC<Requests.Count)
        {
            curRequest = Requests[randomNPC];
            if (randomNPC < clientDatabase.Count) curRequirements = clientDatabase[randomNPC];
            //switch(randomNPC)
            //{
            //    case 0:
            //        bountyful = true;
            //        break;
            //    case 1:
            //        years = true;
            //        beta = true;
            //        break;
            //    case 2:
            //        cold = true;
            //        present = true;
            //        week = true;
            //        break;
            //    case 3:
            //        hot = true;
            //        gods = true;
            //        gamma = true;
            //        break;
            //    case 4:
            //        week = true;
            //        beta = true;
            //        break;
            //    case 5:
            //        present = true;
            //        monsters = true;
            //        week = true;
            //        alpha = true;
            //        break;
            //    case 6:
            //        tempered = true;
            //        bountyful = true;
            //        months = true;
            //        break;
            //    case 7:
            //        hot = true;
            //        none = true;
            //        months = true;
            //        break;
            //    case 8:
            //        present = true;
            //        alpha = true;
            //        break;
            //    case 9:
            //        cold = true;
            //        none = true;
            //        gods = true;
            //        years = true;
            //        break;
            //    default:
            //        break;
            //}
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
    void Delivered()
    {
        clientResolved = true;
    }
}
