using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace GuardiansOfTheWetland.Player
{
    /// <summary>
    /// Reads the player's controls from keyboard and gamepad.
    /// Two keyboard layouts (WASD and arrow keys) plus a gamepad are supported
    /// as alternative interaction options (accessibility, WCAG 2.2 / UDL).
    ///
    /// Works with both the new Input System package and the legacy Input Manager,
    /// depending on what the Unity project has enabled.
    /// </summary>
    public static class PlayerInput
    {
        // Minimum stick tilt considered an intentional movement.
        private const float StickDeadZone = 0.3f;

        /// <summary>
        /// Horizontal direction: -1 = left, 0 = none, 1 = right.
        /// </summary>
        public static float GetHorizontal()
        {
            float direction = 0f;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    direction -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    direction += 1f;
                }
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                // Left stick and D-pad are both accepted.
                float stick = gamepad.leftStick.x.ReadValue() + gamepad.dpad.x.ReadValue();
                if (Mathf.Abs(stick) > StickDeadZone)
                {
                    direction += Mathf.Sign(stick);
                }
            }
#else
            // The default "Horizontal" axis already maps A/D, arrows and joystick.
            direction = Input.GetAxisRaw("Horizontal");
#endif

            return Mathf.Clamp(direction, -1f, 1f);
        }

        /// <summary>
        /// True only on the frame the jump button is pressed.
        /// </summary>
        public static bool JumpPressed()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            Gamepad gamepad = Gamepad.current;

            bool keyboardJump = keyboard != null &&
                (keyboard.spaceKey.wasPressedThisFrame ||
                 keyboard.wKey.wasPressedThisFrame ||
                 keyboard.upArrowKey.wasPressedThisFrame);

            bool gamepadJump = gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;

            return keyboardJump || gamepadJump;
#else
            return Input.GetButtonDown("Jump") ||
                   Input.GetKeyDown(KeyCode.W) ||
                   Input.GetKeyDown(KeyCode.UpArrow) ||
                   Input.GetKeyDown(KeyCode.JoystickButton0);
#endif
        }

        /// <summary>
        /// True only on the frame the jump button is released (used for variable jump height).
        /// </summary>
        public static bool JumpReleased()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            Gamepad gamepad = Gamepad.current;

            bool keyboardRelease = keyboard != null &&
                (keyboard.spaceKey.wasReleasedThisFrame ||
                 keyboard.wKey.wasReleasedThisFrame ||
                 keyboard.upArrowKey.wasReleasedThisFrame);

            bool gamepadRelease = gamepad != null && gamepad.buttonSouth.wasReleasedThisFrame;

            return keyboardRelease || gamepadRelease;
#else
            return Input.GetButtonUp("Jump") ||
                   Input.GetKeyUp(KeyCode.W) ||
                   Input.GetKeyUp(KeyCode.UpArrow) ||
                   Input.GetKeyUp(KeyCode.JoystickButton0);
#endif
        }
    }
}
