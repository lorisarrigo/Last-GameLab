using UnityEngine;
using UnityEngine.UI;

public class MaterialColorChanger : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("La RawImage su cui applicare il materiale")]
    public GameObject targetObject;
    public SpriteRenderer sr;
    public static MaterialColorChanger instance;
    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        sr = targetObject.GetComponent<SpriteRenderer>();
    }
    public void ApplicaMateriale(Sprite timbro)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[MaterialColorChanger] targetImage non assegnata!");
            return;
        }
        if (timbro == null)
        {
            Debug.LogWarning("[MaterialColorChanger] Materiale non assegnato!");
            return;
        }
        targetObject.SetActive(true);
        sr.sprite = timbro;
    }
}