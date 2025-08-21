using Abstracts.Input;
using Movements;
using System.Collections;
using UnityEngine;
using Inputs;
using Animations;
using Mechanics;
using Managers;
using System;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        bool _isJumped;
        float _horizontalAxis;
        IPlayerInput _input;
        CharacterAnimation _anim;
        RbMovement _rb;
        Flip _flip;
        GroundCheck _groundCheck;
        PlatformHandler _platform;
        InteractHandler _interact;
        private bool _isPaused;

        private void Awake()
        {
            _rb = GetComponent<RbMovement>();
            _anim = GetComponent<CharacterAnimation>();
            _flip = GetComponent<Flip>();
            _groundCheck = GetComponent<GroundCheck>();
            _platform = GetComponent<PlatformHandler>();
            _interact = GetComponent<InteractHandler>();
            _input = new PcInput();

            // Debug warnings nếu thiếu component
            if (_rb == null) Debug.LogWarning("⚠️ Player is missing RbMovement component.");
            if (_anim == null) Debug.LogWarning("⚠️ Player is missing CharacterAnimation component.");
            if (_flip == null) Debug.LogWarning("⚠️ Player is missing Flip component.");
            if (_groundCheck == null) Debug.LogWarning("⚠️ Player is missing GroundCheck component.");
            if (_platform == null) Debug.LogWarning("⚠️ Player is missing PlatformHandler component.");
            if (_interact == null) Debug.LogWarning("⚠️ Player is missing InteractHandler component.");
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePaused += HandleGamePaused;
                GameManager.Instance.OnGameUnpaused += HandleGameUnpaused;
            }
        }

        private void OnDisable()
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
            if (_input.IsExitButton)
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySound(2);

                if (_isPaused)
                    GameManager.Instance?.UnpauseGame();
                else
                    GameManager.Instance?.PauseGame();
            }

            if (_isPaused) return;

            _horizontalAxis = _input.HorizontalAxis;

            if (_groundCheck != null && SoundManager.Instance != null)
            {
                if (_horizontalAxis != 0 && _groundCheck.IsOnGround)
                    SoundManager.Instance.PlaySound(1);
                else
                    SoundManager.Instance.StopSound(1);
            }

            if (_input.IsJumpButtonDown && _groundCheck != null && _groundCheck.IsOnGround)
            {
                _isJumped = true;
            }

            if (_input.IsDownButton && _platform != null)
                _platform.DisableCollider();

            if (_input.IsInteractButton && _interact != null)
                _interact.Interact();

            if (_anim != null && _groundCheck != null && _rb != null)
            {
                _anim.JumpAnFallAnim(_groundCheck.IsOnGround, _rb.VelocityY);
                _anim.HorizontalAnim(_horizontalAxis);
            }

            if (_flip != null)
                _flip.FlipCharacter(_horizontalAxis);
        }

        private void FixedUpdate()
        {
            if (_rb != null)
            {
                _rb.HorizontalMove(_horizontalAxis);  // chỉ chạy nếu có RbMovement
                if (_isJumped)
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlaySound(0);

                    _rb.Jump();
                    _isJumped = false;
                }
            }
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;
    }
}
