using UnityEngine;
using Fusion; // Thêm cái này nếu muốn dùng các helper của Fusion

namespace Movements
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RbMovement : MonoBehaviour
    {
        [SerializeField] float _jumpForce = 12f; // Tăng nhẹ lên vì mạng thường có độ trễ vật lý
        [SerializeField] float _horizontalSpeed = 10f;
        private float _horizontalDirection = 1;

        Rigidbody2D _rb;

        public float VelocityY => _rb.linearVelocity.y;
        
        // Fix lỗi cho con quái Trunk: Để public set
        public float HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            // CỰC KỲ QUAN TRỌNG: 
            // Trong Multiplayer, ta nên để Rigidbody ở chế độ này để tránh bị rung (jitter)
            _rb.interpolation = RigidbodyInterpolation2D.None; 
        }

        public void Jump()
        {
            // Reset velocity Y về 0 trước khi nhảy để lực nhảy luôn đồng nhất
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        }

        public void HorizontalMove(float direction)
        {
            if (Mathf.Abs(direction) > 0.01f)
            {
                HorizontalDirection = Mathf.Sign(direction);
            }

            // Áp dụng vận tốc trực tiếp
            _rb.linearVelocity = new Vector2(direction * _horizontalSpeed, _rb.linearVelocity.y);
        }
    }
}