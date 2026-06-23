using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class StampButton : MonoBehaviour
{
    [Header("Parametri di questo pianeta")]
    public PlanetRequirements PlanetData;
    private Button BTN;
    private void Start()
    {
        BTN =GetComponent<Button>();
        BTN.onClick.AddListener(OnStampClicked);
    }
    void OnStampClicked()
    {
        //UI_Manager.instance.ApplyStampData(PlanetData);
        transform.parent.gameObject.SetActive(false);
    }
}
