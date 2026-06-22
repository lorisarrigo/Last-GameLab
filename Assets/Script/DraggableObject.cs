using UnityEngine;
 
/// <summary>
/// Drag 3D via Rigidbody.MovePosition — non dipende da forze né constraint.
/// Richiede: Rigidbody + qualsiasi Collider sull'oggetto.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DraggableObject : MonoBehaviour
{
    [Header("Drag")]
    [Tooltip("Velocità con cui l'oggetto segue il mouse (più alto = più reattivo).")]
    public float followSpeed = 20f;
 
    [Tooltip("Distanza dalla camera. Lascia 0 per calcolarla automaticamente dal punto di click.")]
    public float dragDistance = 0f;
 
    // --- privati ---
    private Rigidbody _rb;
    private Camera _cam;
    private bool _isDragging = false;
    private float _currentDistance;
    private RigidbodyConstraints _savedConstraints;
    private bool _savedGravity;
    private bool _savedKinematic;
 
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main ?? FindFirstObjectByType<Camera>();
    }
 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
 
            foreach (var hit in hits)
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    _currentDistance = dragDistance > 0 ? dragDistance
                                      : Vector3.Distance(_cam.transform.position, hit.point);
                    StartDrag();
                    break;
                }
            }
        }
 
        if (Input.GetMouseButtonUp(0) && _isDragging)
            StopDrag();
    }
 
    void FixedUpdate()
    {
        if (!_isDragging) return;
 
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Vector3 target = ray.GetPoint(_currentDistance);
 
        // MovePosition ignora i Freeze Position constraint — muove sempre
        Vector3 newPos = Vector3.Lerp(_rb.position, target, followSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);
    }
 
    private void StartDrag()
    {
        _isDragging = true;
 
        // Salva stato
        _savedConstraints = _rb.constraints;
        _savedGravity     = _rb.useGravity;
        _savedKinematic   = _rb.isKinematic;
 
        // Durante il drag: kinematic = true, così MovePosition è preciso
        // e la fisica non interferisce
        _rb.isKinematic = true;
        _rb.useGravity  = false;
    }
 
    private void StopDrag()
    {
        _isDragging = false;
 
        // Ripristina tutto
        _rb.isKinematic = _savedKinematic;
        _rb.useGravity  = _savedGravity;
        _rb.constraints = _savedConstraints;
 
        // Azzera la velocità residua per evitare che schizzi via
        if (!_savedKinematic)
        {
            _rb.linearVelocity  = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}