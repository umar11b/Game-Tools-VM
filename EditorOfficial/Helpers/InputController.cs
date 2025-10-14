using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EditorOfficial.Helpers
{
    public static class InputController
    {
        private static KeyboardState _currentKeyboard;
        private static KeyboardState _previousKeyboard;
        private static MouseState _currentMouse;
        private static MouseState _previousMouse;

        public static Microsoft.Xna.Framework.Point MousePosition { get; private set; }
        public static Microsoft.Xna.Framework.Point PreviousMousePosition { get; private set; }
        public static Microsoft.Xna.Framework.Point MouseDelta =>
            new Microsoft.Xna.Framework.Point(MousePosition.X - PreviousMousePosition.X, MousePosition.Y - PreviousMousePosition.Y);

        public static bool LeftPressed => _currentMouse.LeftButton == ButtonState.Pressed;
        public static bool RightPressed => _currentMouse.RightButton == ButtonState.Pressed;
        public static bool MiddlePressed => _currentMouse.MiddleButton == ButtonState.Pressed;
        public static int ScrollDelta => _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;

        public static void Update()
        {
            _previousKeyboard = _currentKeyboard;
            _previousMouse = _currentMouse;

            _currentKeyboard = Keyboard.GetState();
            _currentMouse = Mouse.GetState();

            PreviousMousePosition = MousePosition;
            MousePosition = new Point(_currentMouse.X, _currentMouse.Y);
        }

        public static bool KeyDown(Keys key) => _currentKeyboard.IsKeyDown(key);
        public static bool KeyPressed(Keys key) =>
            _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);
        public static bool KeyReleased(Keys key) =>
            _currentKeyboard.IsKeyUp(key) && _previousKeyboard.IsKeyDown(key);
    }
}
