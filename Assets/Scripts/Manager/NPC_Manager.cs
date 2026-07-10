using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Temperature { None, Hot, Tempered, Cold }
public enum LifeQuantity { None, Bountyful, Present, Little }
public enum Population { None, Monster, Indigenous, Gods }
public enum Permanance { None, Week, Months, Years }
public enum Sector { None, Alpha, Beta, Gamma }

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
    SpriteRenderer npc;
    bool canMove;
    bool clientResolved;
    public bool canSpeak;
    [Header("NPC & Requests")]
    public int clientToday; //da usare come massimo
    public int clientLeft;
    public int clientType;
    public GameObject NPC;
    [SerializeField] List<Sprite> NPC_Sprite = new();
    [SerializeField] List<Sprite> NPC_SpriteB = new();
    [SerializeField] List<string> Requests = new();
    [SerializeField] GameObject[] Waypoints;
    [SerializeField] float speed;
    public int randomNPC;
    public bool alien;

    [HideInInspector] public string curRequest;
    [HideInInspector] public string curClient;
    public string curResult;

    [SerializeField] float speakTime;
    public float speakGap;
    [Header("Ticket")]
    public GameObject Ticket;
    [SerializeField] float ticketSpeed;
    [SerializeField] Transform ticketDeskPos;

    [Header("Current Client Requirements")]
    public PlanetRequirements curRequirements;
    public List<PlanetRequirements> clientDatabase = new();

    [Header("Answers")]
    [SerializeField] NPCAnswers[] NPC_answers;
    
    [Header("Colorful Alien")]
    public int ColorfullAlienIndex;
    [SerializeField] Color[] NPC_Colors;
    [SerializeField] List<string> ColorfulRequests = new();
    public List<PlanetRequirements> ColorfulClientDatabase = new();
    [SerializeField] NPCAnswers[] ColorfulNPC_answers;

    [Header("SFX")]
    public AudioClip steps;
    public string GetNPCAnswer(int npc, int satisfaction)
    {
        if (npc < 0 || npc >= NPC_answers.Length)
        {
            Debug.LogError("indice fuori dal limite");
            return "...";
        }
        if (randomNPC != ColorfullAlienIndex)
        {
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
        else
        {
            NPCAnswers curNPC = ColorfulNPC_answers[npc];
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
    }

    //eventi
    public static event Action OnRequest;
    public static event Action OnTimer;
    public static event Action OnClient;
    public static event Action OnEndDay;

    public static NPC_Manager instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        npc = NPC.GetComponent<SpriteRenderer>();
    }
    private void OnEnable() { UI_Manager.OnDeliver += Delivered; }
    private void OnDisable() { UI_Manager.OnDeliver -= Delivered; }

    public void StartDay(int clients) { StartCoroutine(DailyLoop(clients)); }

    IEnumerator DailyLoop(int nClients)
    {

        while (nClients > 0)
        {
            OnClient?.Invoke();
            RandomClient();

            canMove = false;
            clientResolved = false;
            StartCoroutine(MoveNPC(Waypoints[0], Waypoints[1]));

            yield return new WaitUntil(() => canMove);
            canSpeak = true;
            StartCoroutine(MoveTicket());
            OnRequest?.Invoke();
            StartCoroutine(Speaking());

            yield return new WaitUntil(() => clientResolved);

            if (UI_Manager.instance.success) StartCoroutine(MoveNPC(Waypoints[1], Waypoints[2]));
            else StartCoroutine(MoveNPC(Waypoints[1], Waypoints[0]));

            yield return new WaitForSeconds(3);
            nClients--;
            clientLeft++;
        }
        OnEndDay?.Invoke();
    }

    void RandomClient()
    {
        npc = NPC.GetComponent<SpriteRenderer>();
        npc.color = Color.white;
        if (clientType > NPC_Sprite.Count) clientType = NPC_Sprite.Count;
        randomNPC = UnityEngine.Random.Range(0, clientType);

        npc.sprite = NPC_Sprite[randomNPC];

        if (randomNPC < Requests.Count && randomNPC != ColorfullAlienIndex)
        {
            UI_Manager.instance.npc = randomNPC;
            curRequest = Requests[randomNPC];
            if (randomNPC < clientDatabase.Count) curRequirements = clientDatabase[randomNPC];
        }
        
        if(randomNPC == ColorfullAlienIndex)
        {
            int randomColor = UnityEngine.Random.Range(0, NPC_Colors.Length);
            npc.color = NPC_Colors[randomColor];
            UI_Manager.instance.npc = randomColor;
            curRequest = ColorfulRequests[randomColor];
            if(randomColor < ColorfulClientDatabase.Count) curRequirements = ColorfulClientDatabase[randomColor]; 
        }
        switch (NPC_Manager.instance.randomNPC)
        {
            case 7:
                FB_Manager.instance.Goal = FB_Manager.instance.goal1;
                alien = true;
                break;
            case 8:
                FB_Manager.instance.Goal = FB_Manager.instance.goal2;
                alien = true;
                break;
            case 9:
                FB_Manager.instance.Goal = FB_Manager.instance.goal3;
                alien = true;
                break;
            case 12:
                FB_Manager.instance.Goal = FB_Manager.instance.goal2;
                alien = true;
                break;
            case 13:
                FB_Manager.instance.Goal = FB_Manager.instance.goal1;
                alien = true;
                break;
            case 15:
                FB_Manager.instance.Goal = FB_Manager.instance.goal3;
                alien = true;
                break;
            case 16:
                FB_Manager.instance.Goal = FB_Manager.instance.goal1;
                alien = true;
                break;
            case 17:
                FB_Manager.instance.Goal = FB_Manager.instance.goal1;
                alien = true;
                break;
            case 18:
                FB_Manager.instance.Goal = FB_Manager.instance.goal2;
                alien = true;
                break;
            case 19:
                FB_Manager.instance.Goal = FB_Manager.instance.goal3;
                alien = true;
                break;
            default:
                alien = false;
                break;
        }
    }
    IEnumerator MoveNPC(GameObject startP, GameObject endP)
    {
        SFX_Manager.instance.PlaySfx(steps);
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
    IEnumerator Speaking()
    {
        float timer = 0f;
        while (timer < speakTime && canSpeak)
        {
            npc.sprite = NPC_SpriteB[randomNPC];
            yield return new WaitForSeconds(speakGap);
            timer += speakGap;
            npc.sprite = NPC_Sprite[randomNPC];
            yield return new WaitForSeconds(speakGap);
            timer += speakGap;
        }
        npc.sprite = NPC_Sprite[randomNPC];
        canSpeak = false;
    }
    IEnumerator MoveTicket()
    {
        TicketController.instance.confirmBtn.SetActive(false);
        TicketController.instance.stampButton.SetActive(false);
        Ticket.transform.position = Waypoints[1].transform.position;
        Ticket.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * ticketSpeed;
            Ticket.transform.position = Vector3.Lerp(Waypoints[1].transform.position, ticketDeskPos.position, t);
            yield return null;
        }
        if (UI_Manager.instance.requestTxtSpace.font != UI_Manager.instance.alien) TicketController.instance.stampButton.SetActive(true);

        OnTimer?.Invoke();
    }
    void Delivered() { clientResolved = true; }
}