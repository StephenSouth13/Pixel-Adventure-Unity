using UnityEngine;

public class CrossPlatformInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public Rigidbody2D rb;

    [Header("Mobile Joystick")]
    public Joystick mobileJoystick;

    private Vector2 moveInput;

    void Update()
    {
        moveInput = Vector2.zero;

        // --- PC & WebGL (Keyboard + Mouse Click) ---
#if UNITY_STANDALONE || UNITY_WEBGL
        float h = Input.GetAxisRaw("Horizontal"); // A/D hoặc ← →
        float v = Input.GetAxisRaw("Vertical");   // W/S hoặc ↑ ↓
        moveInput = new Vector2(h, v);

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click at: " + Input.mousePosition);
        }
#endif

        // --- Mobile (Joystick Only) ---
#if UNITY_ANDROID || UNITY_IOS
        if (mobileJoystick != null)
        {
            moveInput = new Vector2(mobileJoystick.Horizontal, mobileJoystick.Vertical);
        }

        // ❌ Bỏ vuốt để di chuyển
        // ❌ Không dùng touch delta để lướt
        // ✅ Giữ lại tap để debug vị trí chạm
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                Debug.Log("Tap at: " + t.position);
            }
        }
#endif
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;
    }
}