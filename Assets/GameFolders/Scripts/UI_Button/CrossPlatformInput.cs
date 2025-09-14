using UnityEngine;

public class CrossPlatformInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public Rigidbody2D rb;

    [Header("Mobile Controls")]
    public Joystick mobileJoystick;
    public bool jumpButtonPressed = false; // Gán từ UI Button

    private Vector2 moveInput;
    private bool isJumping = false;

    void Update()
    {
        moveInput = Vector2.zero;

        // --- PC & WebGL (Keyboard) ---
#if UNITY_STANDALONE || UNITY_WEBGL
        float h = Input.GetAxisRaw("Horizontal");
        moveInput = new Vector2(h, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click at: " + Input.mousePosition);
        }
#endif

        // --- Mobile (Joystick + Button) ---
#if UNITY_ANDROID || UNITY_IOS
        if (mobileJoystick != null)
        {
            moveInput = new Vector2(mobileJoystick.Horizontal, 0);
            Debug.Log("Joystick Horizontal: " + mobileJoystick.Horizontal);
        }

        if (jumpButtonPressed)
        {
            isJumping = true;
            jumpButtonPressed = false; // reset sau khi nhận
        }

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
        // Di chuyển ngang
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

        // Nhảy
        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = false;
        }
    }

    // ✅ Gọi từ UI Button
    public void OnJumpButtonPressed()
    {
        jumpButtonPressed = true;
    }
}