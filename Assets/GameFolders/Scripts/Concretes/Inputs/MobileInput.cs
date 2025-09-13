using Abstracts.Input;
using UnityEngine;

namespace Inputs
{
    public class MobileInput : IPlayerInput
    {
        private Joystick _joystick;

        public MobileInput(Joystick joystick)
        {
            _joystick = joystick;
        }

        public float HorizontalAxis => _joystick != null ? _joystick.Horizontal : 0f;
        public bool IsJumpButtonDown => false; // dùng UI Button riêng
        public bool IsJumpButton => false;     // thêm dòng này để khớp interface
        public bool IsExitButton => false;
        public bool IsDownButton => false;
        public bool IsInteractButton => false;
    }
}