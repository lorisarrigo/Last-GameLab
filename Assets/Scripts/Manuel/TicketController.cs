using System.Collections;
using UnityEngine;

public class TicketController : MonoBehaviour
{
    [Header("Zone")]
    public Transform zonaTarget;     // Zona 2
    public Transform zonaIniziale;   // Zona 1
    public Transform zonaFinale;     // Zona 3

    [Header("UI")]
    public GameObject oggettoUI;
    public GameObject stampButton;
    public GameObject confirmBtn;
    public GameObject OpenStamp;
    public GameObject changeStamp;

    [Header("Settings")]
    public float durateLerp = 0.5f;

    [Header("Planet")]
    public int planetchoose;

    [HideInInspector] public bool isTimbrato = false;

    private bool isMoving = false;
    public bool isAndato = false;


    public static TicketController instance;

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
            return;
        }
            instance = this;
    }
    void Start()
    {
        if (zonaIniziale == null)
        {
            GameObject go = new GameObject("ZonaIniziale_Auto");
            go.transform.position = transform.position;
            zonaIniziale = go.transform;
        }

        if (oggettoUI != null)
            oggettoUI.SetActive(false);
    }

    void Update()
    {
        if (isTimbrato && isAndato && !isMoving)
        {
            isTimbrato = false;
            StartCoroutine(LerpTo(zonaIniziale.position, onComplete: () =>
            {
                isAndato = false;
            }));
        }
    }

    public void OnTicketClick()
    {
        if (isMoving) return;

        if (!isAndato)
        {
            StartCoroutine(LerpTo(zonaTarget.position, onComplete: () =>
            {
                isAndato = true;
                if (oggettoUI != null)
                {
                    OpenStamp.SetActive(true);
                    stampButton.SetActive(false);
                }
            }));
        }
        else
        {
            OnSecondoClick();
        }
    }
    public void ChangeStamp()
    {
        confirmBtn.SetActive(false);
        if(isMoving) return;
        StartCoroutine(LerpTo(zonaTarget.position, onComplete: () =>
        {
            if (oggettoUI != null)
            {
                OpenStamp.SetActive(true);
                stampButton.SetActive(false);
            }
        }));
    }
    public void OnChiudiUI(int planetindex)
    {
        if (isMoving) return;

        if (oggettoUI != null)
        {
            oggettoUI.SetActive(false);
        }
        StartCoroutine(LerpTo(zonaIniziale.position, onComplete: () =>
        {
            confirmBtn.SetActive(true);
            planetchoose = planetindex;
            changeStamp.SetActive(true);
        }));
    }
    public void ReadyToSend()
    {
        isAndato = false;
        changeStamp.SetActive(false);
        int definitiveplanet = planetchoose;
        {
            StartCoroutine(LerpTo(zonaFinale.position, onComplete: () =>
            {
                OnArrivatoZonaFinale(definitiveplanet);
                confirmBtn.SetActive(false);
                stampButton.SetActive(true);
                MaterialColorChanger.instance.targetObject.SetActive(false);
            }));
        }
    }
    void OnArrivatoZonaFinale(int index)
    {
        Jew_Manager.instance.SelectPlanet(index);
        NPC_Manager.instance.Ticket.SetActive(false);
        MaterialColorChanger.instance.targetObject.SetActive(false);
    }

    void OnSecondoClick()
    {
        Debug.Log("Secondo click — aggiungi la tua funzione!");
    }

    IEnumerator LerpTo(Vector3 destinazione, System.Action onComplete = null)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < durateLerp)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, destinazione, elapsed / durateLerp);
            yield return null;
        }

        transform.position = destinazione;
        isMoving = false;
        onComplete?.Invoke();
    }
}