using Abstracts.Input;
using Movements;
using UnityEngine;
using Inputs;
using Animations;
using Mechanics;
using Managers;
using Fusion;

namespace Controllers
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Mobile Joystick (Optional)")]
        public Joystick mobileJoystick;

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
        }

        public override void Spawned()
        {
            // Khởi tạo Input dựa trên nền tảng ngay khi Object xuất hiện trên mạng
            // Đảm bảo chỉ khởi tạo cho người chơi có quyền điều khiển
            if (Object.HasInputAuthority)
            {
#if UNITY_ANDROID || UNITY_IOS
                _input = new MobileInput(mobileJoystick);
#else
                _input = new PcInput();
#endif
                Debug.Log("Đã triệu hồi nhân vật của Long thành công!");
            }
            else
            {
                // Tắt các thành phần thừa trên máy của người chơi khác
                Camera localCam = GetComponentInChildren<Camera>();
                if (localCam != null) localCam.enabled = false;

                AudioListener localListener = GetComponentInChildren<AudioListener>();
                if (localListener != null) localListener.enabled = false;
            }
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
            // FIX: Kiểm tra Object và InputAuthority trước khi chạy bất cứ thứ gì trong Update
            if (Object == null || !Object.HasInputAuthority || _input == null) return;

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
            
            if (_input.IsJumpButtonDown) _localJump = true;
            if (_input.IsDownButton) _localDown = true;
            if (_input.IsInteractButton) _localInteract = true;

            // 3. Audio Local (Sửa lại check null cực kỳ an toàn)
            bool canPlayWalkSound = _localHorizontal != 0 && (_groundCheck != null && _groundCheck.IsOnGround);
            
            if (SoundManager.Instance != null) // Check Instance trước khi gọi
            {
                if (canPlayWalkSound)
                    SoundManager.Instance.PlaySound(1);
                else
                    SoundManager.Instance.StopSound(1);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_isPaused) return;

            if (GetInput(out NetworkInputData data))
            {
                _rb?.HorizontalMove(data.horizontalAxis);

                if (data.jumpPressed && (_groundCheck != null && _groundCheck.IsOnGround))
                {
                    SoundManager.Instance?.PlaySound(0);
                    _rb?.Jump();
                    
#if UNITY_ANDROID || UNITY_IOS
                    if (_input is MobileInput mobile) mobile.ResetJump();
#endif
                }

                if (data.downPressed) _platform?.DisableCollider();
                if (data.interactPressed) _interact?.Interact();

                // Sync Visuals
                _anim?.JumpAnFallAnim(_groundCheck != null && _groundCheck.IsOnGround, _rb != null ? _rb.VelocityY : 0f);
                _anim?.HorizontalAnim(data.horizontalAxis);
                _flip?.FlipCharacter(data.horizontalAxis);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Chỉ gửi input nếu mình có quyền
            if (!Object.HasInputAuthority) return;

            var data = new NetworkInputData();
            data.horizontalAxis = _localHorizontal;
            data.jumpPressed = _localJump;
            data.downPressed = _localDown;
            data.interactPressed = _localInteract;

            input.Set(data);

            _localJump = false;
            _localDown = false;
            _localInteract = false;
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;

        public void OnJumpButtonPressed() 
        {
            _localJump = true; 
        }
    }
}