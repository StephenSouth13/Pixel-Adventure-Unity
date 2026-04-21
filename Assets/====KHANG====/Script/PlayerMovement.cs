using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 5f; // Speed of the player movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    private Vector2 movement; // Store the movement input
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // Phím A/D hoặc Trái/Phải
        movement.y = Input.GetAxisRaw("Vertical");   // Phím W/S hoặc Lên/Xuống 
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime); // Di chuyển nhân vật
    }
}

