using UnityEngine;

public class CrossPlatformInput : MonoBehaviour
{
    public float speed = 5.0f;
    public Rigidbody2D rb;
    public Joystick mobileJoystick;

    private Vector2 moveInput;

    void Update()
    {
        moveInput = Vector2.zero;

#if UNITY_STANDALONE || UNITY_WEBGL
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v);

        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse click at: " + Input.mousePosition);
#endif

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
        }
#endif
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;
    }
}