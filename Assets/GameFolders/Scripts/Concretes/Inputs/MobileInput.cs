using Abstracts.Input;
using UnityEngine;

namespace Inputs
{
    public class MobileInput : IPlayerInput
    {
        private Joystick _joystick;
        private bool _jumpPressed = false;

        public MobileInput(Joystick joystick)
        {
            _joystick = joystick;
        }

        public float HorizontalAxis => _joystick != null ? _joystick.Horizontal : 0f;
        public bool IsJumpButtonDown => _jumpPressed;
        public bool IsExitButton => false;
        public bool IsDownButton => false;
        public bool IsInteractButton => false;

        public void PressJump() => _jumpPressed = true;
        public void ResetJump() => _jumpPressed = false;
    }
}