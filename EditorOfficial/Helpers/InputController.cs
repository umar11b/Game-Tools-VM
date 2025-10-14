using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace EditorOfficial.Helpers
{
    public static class InputController
    {
        private static MouseState _currentMouse;
        private static MouseState _previousMouse;
        private static KeyboardState _currentKeyboard;
        private static KeyboardState _previousKeyboard;

        public static Point MousePosition { get; private set; }
        public static Point PreviousMousePosition { get; private set; }
        public static Point MouseDelta => new Point(MousePosition.X - PreviousMousePosition.X,
                                                   MousePosition.Y - PreviousMousePosition.Y);

        public static int ScrollDelta => _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;

        public static bool LeftPressed => _currentMouse.LeftButton == ButtonState.Pressed;
        public static bool RightPressed => _currentMouse.RightButton == ButtonState.Pressed;
        public static bool MiddlePressed => _currentMouse.MiddleButton == ButtonState.Pressed;

        public static void Update()
        {
            _previousKeyboard = _currentKeyboard;
            _previousMouse = _currentMouse;

            _currentKeyboard = Keyboard.GetState();
            _currentMouse = Mouse.GetState();

            PreviousMousePosition = MousePosition;
            MousePosition = new Point(_currentMouse.X, _currentMouse.Y);
        }

        // === Keyboard helpers ===
        public static bool KeyDown(Keys key) => _currentKeyboard.IsKeyDown(key);
        public static bool KeyPressed(Keys key) =>
            _currentKeyboard.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);
        public static bool KeyReleased(Keys key) =>
            !_currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyDown(key);
    }
}
