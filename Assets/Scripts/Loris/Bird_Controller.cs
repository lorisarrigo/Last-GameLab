using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird_Controller : MonoBehaviour
{
    //references
    Rigidbody2D rb;
    InputMap inputs; 

    //Movement values
    [SerializeField] float jumpForce;
    [SerializeField] float rotSpeed;

    //eventi
    public static event Action OnColl;

    //singleton
    public static Bird_Controller instance;
    private void Awake()
    {
        #region singleton
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        #endregion
        inputs = new InputMap();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Bird.Tap.started += OnTap;
    }
    private void OnDisable()
    {
        inputs.Disable();
        inputs.Bird.Tap.started -= OnTap;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnColl?.Invoke();
    }
    private void Update()
    {
        RotationBird();
    }
    private void RotationBird()
    {
        float angle = Mathf.Clamp(rb.linearVelocityY * 5f, -90, 30);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,angle), rotSpeed * Time.deltaTime);
    }
    private void OnTap(InputAction.CallbackContext context)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
