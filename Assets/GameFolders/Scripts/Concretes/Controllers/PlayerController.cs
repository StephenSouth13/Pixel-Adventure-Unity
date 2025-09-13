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
        [Header("Mobile Jump Button")]
        public bool jumpButtonPressed = false; // Gán từ UI Button

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

        [Header("Mobile Joystick (Optional)")]
        public Joystick mobileJoystick; // Gán từ Inspector nếu dùng mobile

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

        private void OnEnable()
        {
            GameManager.Instance.OnGamePaused += HandleGamePaused;
            GameManager.Instance.OnGameUnpaused += HandleGameUnpaused;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGamePaused -= HandleGamePaused;
            GameManager.Instance.OnGameUnpaused -= HandleGameUnpaused;
        }

        private void Update()
        {
            if (_input.IsExitButton)
            {
                SoundManager.Instance?.PlaySound(2);
                if (_isPaused)
                    GameManager.Instance?.UnpauseGame();
                else
                    GameManager.Instance?.PauseGame();
            }

            if (_isPaused) return;

            _horizontalAxis = _input.HorizontalAxis;

            if (_horizontalAxis != 0 && _groundCheck.IsOnGround)
                SoundManager.Instance?.PlaySound(1);
            else
                SoundManager.Instance?.StopSound(1);

            if ((_input.IsJumpButtonDown || jumpButtonPressed) && _groundCheck.IsOnGround)
                _isJumped = true;

            if (_input.IsDownButton)
                _platform?.DisableCollider();

            if (_input.IsInteractButton)
                _interact?.Interact();

            _anim?.JumpAnFallAnim(_groundCheck.IsOnGround, _rb.VelocityY);
            _anim?.HorizontalAnim(_horizontalAxis);
            _flip?.FlipCharacter(_horizontalAxis);
        }

        private void FixedUpdate()
        {
            _rb?.HorizontalMove(_horizontalAxis);

            if (_isJumped)
            {
                SoundManager.Instance?.PlaySound(0);
                _rb?.Jump();
                _isJumped = false;
                jumpButtonPressed = false; // reset sau khi xử lý
            }
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;

        // ✅ Gọi từ UI Button
        public void OnJumpButtonPressed()
        {
            jumpButtonPressed = true;
        }
    }
}