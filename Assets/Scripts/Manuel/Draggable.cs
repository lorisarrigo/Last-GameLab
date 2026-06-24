using UnityEngine;

/// <summary>
/// Rende un GameObject trascinabile con il mouse o il tocco (touch).
/// Attach questo script a qualsiasi oggetto con un Collider.
/// </summary>
public class Draggable : MonoBehaviour
{
    [Header("Impostazioni")]
    [Tooltip("Se true, l'oggetto viene trascinato sul piano XY (2D o UI). Se false, sul piano XZ (3D ground).")]
    public bool isTopDown = false;

    [Tooltip("Offset verticale per tenere l'oggetto sopra al pavimento in modalità 3D.")]
    public float heightOffset = 0.5f;

    [Tooltip("Se true, usa Rigidbody per il movimento (più fisico).")]
    public bool useRigidbody = false;

    [Tooltip("Velocità di interpolazione quando si usa il Rigidbody.")]
    public float lerpSpeed = 20f;

    [Header("Vincoli opzionali")]
    [Tooltip("Limita il drag all'interno di questi bounds (lascia a zero per ignorare).")]
    public Bounds dragBounds;
    public bool useBounds = false;

    // --- Privati ---
    private bool _isDragging = false;
    private Vector3 _offset;
    private Camera _cam;
    private Rigidbody _rb;
    private Vector3 _targetPosition;

    void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody>();

        if (_rb != null && useRigidbody)
        {
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        // Supporto touch (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                TryStartDrag(touch.position);
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                UpdateDrag(touch.position);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                StopDrag();
        }
        else
        {
            // Supporto mouse (desktop)
            if (Input.GetMouseButtonDown(0))
                TryStartDrag(Input.mousePosition);
            else if (Input.GetMouseButton(0))
                UpdateDrag(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0))
                StopDrag();
        }
    }

    void FixedUpdate()
    {
        // Movimento fluido tramite Rigidbody
        if (_isDragging && _rb != null && useRigidbody)
        {
            _rb.MovePosition(Vector3.Lerp(_rb.position, _targetPosition, lerpSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Controlla se il click/touch ha colpito questo oggetto e avvia il drag.
    /// </summary>
    private void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            _isDragging = true;
            _offset = transform.position - GetWorldPosition(screenPos);

            if (_rb != null && useRigidbody)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Aggiorna la posizione durante il drag.
    /// </summary>
    private void UpdateDrag(Vector2 screenPos)
    {
        if (!_isDragging) return;

        _targetPosition = GetWorldPosition(screenPos) + _offset;

        if (useBounds)
            _targetPosition = ClampToBounds(_targetPosition);

        // Se non usiamo il Rigidbody, muoviamo direttamente il transform
        if (!useRigidbody || _rb == null)
            transform.position = _targetPosition;
    }

    /// <summary>
    /// Termina il drag.
    /// </summary>
    private void StopDrag()
    {
        _isDragging = false;
    }

    /// <summary>
    /// Converte una posizione schermo in posizione mondo.
    /// </summary>
    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        if (isTopDown)
        {
            // Piano XY (2D o vista dall'alto con Y come altezza)
            Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, _cam.WorldToScreenPoint(transform.position).z);
            return _cam.ScreenToWorldPoint(screenPoint);
        }
        else
        {
            // Piano XZ (3D ground-level)
            Ray ray = _cam.ScreenPointToRay(screenPos);
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y - heightOffset, 0));

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                point.y = transform.position.y; // mantieni l'altezza originale
                return point;
            }

            return transform.position;
        }
    }

    /// <summary>
    /// Limita la posizione all'interno dei dragBounds.
    /// </summary>
    private Vector3 ClampToBounds(Vector3 pos)
    {
        return new Vector3(
            Mathf.Clamp(pos.x, dragBounds.min.x, dragBounds.max.x),
            Mathf.Clamp(pos.y, dragBounds.min.y, dragBounds.max.y),
            Mathf.Clamp(pos.z, dragBounds.min.z, dragBounds.max.z)
        );
    }

    // Opzionale: visualizza i bounds nell'editor
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawCube(dragBounds.center, dragBounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(dragBounds.center, dragBounds.size);
        }
    }
}

