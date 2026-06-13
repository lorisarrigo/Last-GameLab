using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird_Controller : MonoBehaviour
{
    //references
    Rigidbody2D rb;
    InputMap inputs; 

    //eventi
    public static event Action OnColl;
    public static event Action OnPoint;

    //singleton
    public static Bird_Controller instance;
    void Awake()
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
    void OnEnable()
    {
        inputs.Enable();
        inputs.Bird.Tap.started += OnTap;
    }
    void OnDisable()
    {
        inputs.Disable();
        inputs.Bird.Tap.started -= OnTap;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        OnColl?.Invoke();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        OnPoint?.Invoke();
    }
    void Update()
    {
        RotationBird();
    }
    void RotationBird()
    {
        float angle = Mathf.Clamp(rb.linearVelocityY * 5f, -90, 30);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,angle), FB_Manager.instance.rotSpeed * Time.deltaTime);
    }
    void OnTap(InputAction.CallbackContext context)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * FB_Manager.instance.jumpForce, ForceMode2D.Impulse);
    }
}
