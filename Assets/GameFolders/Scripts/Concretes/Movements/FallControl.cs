using Abstracts.Input;
using Inputs;
using UnityEngine;

namespace Movements
{
    public class FallControl : MonoBehaviour
    {
        [Header("Fall Settings")]
        [SerializeField] float _fallMultiplier = 2f;
        [SerializeField] float _lowJumpMultiplier = 2f;

        Rigidbody2D _rb;
        IPlayerInput _input;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

#if UNITY_ANDROID || UNITY_IOS
            Joystick joystick = FindFirstObjectByType<Joystick>();
            _input = new MobileInput(joystick);
#else
            _input = new PcInput();
#endif
        }

        private void Update()
        {
            float velocityY = _rb.linearVelocity.y;

            // Rơi nhanh hơn khi đang rơi
            if (velocityY < 0)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _fallMultiplier * Time.deltaTime;
            }
            // Nhảy thấp nếu thả nút nhảy sớm
            else if (velocityY > 0.01f && !_input.IsJumpButtonDown)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _lowJumpMultiplier * Time.deltaTime;
            }
        }
    }
}