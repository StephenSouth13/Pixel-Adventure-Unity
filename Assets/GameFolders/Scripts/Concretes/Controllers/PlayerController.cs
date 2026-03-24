using Abstracts.Input;
using Movements;
using UnityEngine;
using Inputs;
using Animations;
using Mechanics;
using Managers;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Mobile Joystick (Optional)")]
        public Joystick mobileJoystick;

        float _horizontalAxis;
        bool _isJumped;
        bool _isPaused;

        IPlayerInput _input;
        CharacterAnimation _anim;
        RbMovement _rb;
        Flip _flip;
        GroundCheck _groundCheck;
        PlatformHandler _platform;
        InteractHandler _interact;

        private void Awake()
        {
            _rb = GetComponent<RbMovement>();
            _anim = GetComponent<CharacterAnimation>();
            _flip = GetComponent<Flip>();
            _groundCheck = GetComponent<GroundCheck>();
            _platform = GetComponent<PlatformHandler>();
            _interact = GetComponent<InteractHandler>();

#if UNITY_ANDROID || UNITY_IOS
            _input = new MobileInput(mobileJoystick);
#else
            _input = new PcInput();
#endif
        }

        // Chuyển sang Start để đảm bảo GameManager.Instance đã được khởi tạo xong (Awake của GameManager chạy trước)
        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePaused += HandleGamePaused;
                GameManager.Instance.OnGameUnpaused += HandleGameUnpaused;
                
                // Cập nhật trạng thái pause hiện tại của game ngay khi player xuất hiện
                _isPaused = GameManager.Instance.IsGamePaused;
            }
        }

        private void OnDestroy() // Sử dụng OnDestroy thay cho OnDisable để tránh lỗi khi chuyển Scene
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePaused -= HandleGamePaused;
                GameManager.Instance.OnGameUnpaused -= HandleGameUnpaused;
            }
        }

        private void Update()
        {
            // Kiểm tra null an toàn cho _input
            if (_input == null) return;

            if (_input.IsExitButton)
            {
                SoundManager.Instance?.PlaySound(2);
                if (_isPaused) GameManager.Instance?.UnpauseGame();
                else GameManager.Instance?.PauseGame();
            }

            if (_isPaused) return;

            _horizontalAxis = _input.HorizontalAxis;

            // Kiểm tra null cho SoundManager và GroundCheck
            if (_horizontalAxis != 0 && (_groundCheck != null && _groundCheck.IsOnGround))
                SoundManager.Instance?.PlaySound(1);
            else
                SoundManager.Instance?.StopSound(1);

            if (_input.IsJumpButtonDown && (_groundCheck != null && _groundCheck.IsOnGround))
                _isJumped = true;

            if (_input.IsDownButton)
                _platform?.DisableCollider();

            if (_input.IsInteractButton)
                _interact?.Interact();

            // Sử dụng toán tử ?. để tránh NullReferenceException nếu thiếu Component
            _anim?.JumpAnFallAnim(_groundCheck != null && _groundCheck.IsOnGround, _rb != null ? _rb.VelocityY : 0f);
            _anim?.HorizontalAnim(_horizontalAxis);
            _flip?.FlipCharacter(_horizontalAxis);
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;

            _rb?.HorizontalMove(_horizontalAxis);

            if (_isJumped)
            {
                SoundManager.Instance?.PlaySound(0);
                _rb?.Jump();
                _isJumped = false;

#if UNITY_ANDROID || UNITY_IOS
                if (_input is MobileInput mobile)
                    mobile.ResetJump();
#endif
            }
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;

        public void OnJumpButtonPressed()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (_input is MobileInput mobile)
                mobile.PressJump();
#endif
        }
    }
}