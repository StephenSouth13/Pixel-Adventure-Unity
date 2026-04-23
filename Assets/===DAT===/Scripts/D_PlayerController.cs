using Abstracts.Input;
using Movements;
using UnityEngine;
using Inputs;
using Animations;
using Mechanics;
using Managers;
using TMPro;

namespace Controllers
{
    public class D_PlayerController : MonoBehaviour
    {
        // === UI  ===
        public Transform canvas; 
        public TextMeshProUGUI playerName;
        // === end ===
        float _localHorizontal;
        bool _localJump;
        bool _localDown;
        bool _localInteract;
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
            // Nếu có joystick mobile thì truyền vào
            _input = new MobileInput(FindObjectOfType<Joystick>());
#else
            _input = new PcInput();
#endif
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePaused += HandleGamePaused;
                GameManager.Instance.OnGameUnpaused += HandleGameUnpaused;
                _isPaused = GameManager.Instance.IsGamePaused;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePaused -= HandleGamePaused;
                GameManager.Instance.OnGameUnpaused -= HandleGameUnpaused;
            }
        }

        private void Update()
        {
            if (_input == null) return;

            // 1. Xử lý Pause
            if (_input.IsExitButton)
            {
                SoundManager.Instance?.PlaySound(2);
                if (_isPaused) GameManager.Instance?.UnpauseGame();
                else GameManager.Instance?.PauseGame();
            }

            if (_isPaused) return;

            // 2. Gom Input vào biến tạm
            _localHorizontal = _input.HorizontalAxis;
            _localDown = _input.IsDownButton;
            _localInteract = _input.IsInteractButton;

            // 3. Audio Local
            bool canPlayWalkSound = _localHorizontal != 0 && (_groundCheck != null && _groundCheck.IsOnGround);
            if (SoundManager.Instance != null)
            {
                if (canPlayWalkSound)
                    SoundManager.Instance.PlaySound(1);
                else
                    SoundManager.Instance.StopSound(1);
            }
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;

            // Di chuyển trực tiếp bằng input local
            _rb?.HorizontalMove(_localHorizontal);
            _localJump = _input.IsJumpButtonDown;
            if (_localJump && _groundCheck.IsOnGround)
            {
                // SoundManager.Instance?.PlaySound(0);
                _rb?.Jump();
                _localJump = false; // reset
            }

            if (_localDown) _platform?.DisableCollider();
            if (_localInteract) _interact?.Interact();

            // Sync Visuals
            _anim?.JumpAnFallAnim(_groundCheck != null && _groundCheck.IsOnGround, _rb != null ? _rb.VelocityY : 0f);
            _anim?.HorizontalAnim(_localHorizontal);
            _flip?.FlipCharacter(_localHorizontal);
        }
        private void LateUpdate()
        {
            canvas.position = gameObject.transform.position;
        }
        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;

        public void OnJumpButtonPressed()
        {
            _localJump = true;
        }
    }
}
