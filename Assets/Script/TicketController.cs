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
    public GameObject Button;

    [Header("Settings")]
    public float durateLerp = 0.5f;
    //public PlanetRequirements planetData;
    
    [HideInInspector] public bool isTimbrato = false;

    private bool isMoving = false;
    private bool isAndato = false;

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
                    oggettoUI.SetActive(true);
                    Button.SetActive(false);
                }
            }));
        }
        else
        {
            OnSecondoClick();
        }
    }

    public void OnChiudiUI(int planetindex)
    {
        if (isMoving) return;

        if (oggettoUI != null)
        {
            oggettoUI.SetActive(false);
            Button.SetActive(true);
        }
        StartCoroutine(LerpTo(zonaIniziale.position, onComplete: () =>
        {
            isAndato = false;

            StartCoroutine(LerpTo(zonaFinale.position, onComplete: () =>
            {
                OnArrivatoZonaFinale(planetindex);
            }));
        }));
    }

    void OnArrivatoZonaFinale(int index)
    {
        Jew_Manager.instance.SelectPlanet(index);
        Debug.Log("Arrivato in Zona 3!");
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