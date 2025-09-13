using UnityEngine;

public class CrossPlatformInput : MonoBehaviour
{
    public float speed = 5.0f;
    public Rigidbody2D rb;

    private Vector2 moveInput;
    public Joystick mobileJoystick;
    void Update()
    {
        moveInput = Vector2.zero;

        // --- PC & WebGL (Keyboard) ---
#if UNITY_STANDALONE || UNITY_WEBGL
        float h = Input.GetAxisRaw("Horizontal"); // A/D hoặc ← →
        float v = Input.GetAxisRaw("Vertical");   // W/S hoặc ↑ ↓
        moveInput = new Vector2(h, v);

        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse click at: " + Input.mousePosition);
#endif

        // --- Mobile (Touch) ---
#if UNITY_ANDROID || UNITY_IOS


        if (mobileJoystick != null)
        {
            moveInput = new Vector2(mobileJoystick.Horizontal, mobileJoystick.Vertical);
        }
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                Debug.Log("Tap at: " + t.position);

            if (t.phase == TouchPhase.Moved)
                moveInput = t.deltaPosition.normalized; // vuốt để di chuyển
        }
        
#endif
    }

    void FixedUpdate()
    {
        
        rb.linearVelocity = moveInput.normalized * speed;
    }
}