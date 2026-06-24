using UnityEngine;
using UnityEngine.EventSystems;
 
/// <summary>
/// Drag con il mouse per elementi UI (Canvas).
/// Attach su qualsiasi RectTransform (Image, Panel, ecc.).
/// Richiede un componente che implementi IPointerDownHandler (es. Image con Raycast Target attivo).
/// </summary>
public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Impostazioni")]
    [Tooltip("Se true, l'elemento viene portato in primo piano (last sibling) durante il drag.")]
    public bool bringToFront = true;
 
    [Tooltip("Limita il drag all'interno del RectTransform padre.")]
    public bool clampToParent = false;
 
    private RectTransform _rect;
    private Canvas _canvas;
    private Vector2 _pointerOffset;
 
    void Awake()
    {
        _rect = GetComponent<RectTransform>();
 
        // Cerca il Canvas nel parent (funziona sia con Screen Space Overlay che Camera)
        _canvas = GetComponentInParent<Canvas>();
 
        if (_canvas == null)
            Debug.LogWarning("[DraggableUI] Nessun Canvas trovato nei parent di " + gameObject.name);
    }
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (bringToFront)
            transform.SetAsLastSibling();
 
        // Calcola l'offset tra il punto di click e il centro del RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect,
            eventData.position,
            GetCamera(),
            out _pointerOffset
        );
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null) return;
 
        // Converti la posizione schermo in posizione locale nel Canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            GetCamera(),
            out Vector2 localPoint))
        {
            Vector2 newPos = localPoint - _pointerOffset;
 
            if (clampToParent)
                newPos = ClampToParent(newPos);
 
            _rect.localPosition = newPos;
        }
    }
 
    public void OnPointerUp(PointerEventData eventData) { }
 
    private Vector2 ClampToParent(Vector2 pos)
    {
        RectTransform parentRect = _rect.parent as RectTransform;
        if (parentRect == null) return pos;
 
        Vector2 parentSize = parentRect.rect.size;
        Vector2 selfSize   = _rect.rect.size;
 
        float minX = -parentSize.x / 2f + selfSize.x / 2f;
        float maxX =  parentSize.x / 2f - selfSize.x / 2f;
        float minY = -parentSize.y / 2f + selfSize.y / 2f;
        float maxY =  parentSize.y / 2f - selfSize.y / 2f;
 
        return new Vector2(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY));
    }
 
    private Camera GetCamera()
    {
        // Screen Space Overlay non usa camera; Camera/World Space sì
        return _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
    }
}