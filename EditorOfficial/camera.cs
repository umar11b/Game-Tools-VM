using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EditorOfficial
{
    public class Camera
    {
        // Position and orientation
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; } = Vector3.Up;

        // Matrices
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        // Control settings
        private float _yaw;
        private float _pitch;
        private float _moveSpeed = 10f;
        private float _rotationSpeed = 0.005f;
        private float _zoomSpeed = 2f;

        // Input state
        private MouseState _previousMouse;
        private bool _firstClick = true;

        // Aspect ratio
        private float _aspectRatio;

        public Camera(float aspectRatio)
        {
            _aspectRatio = aspectRatio;
            Position = new Vector3(0, 5, 15);
            Target = Vector3.Zero;
            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            // === Rotation (right mouse drag) ===
            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (_firstClick)
                {
                    _previousMouse = mouse;
                    _firstClick = false;
                }

                float deltaX = mouse.X - _previousMouse.X;
                float deltaY = mouse.Y - _previousMouse.Y;

                _yaw -= deltaX * _rotationSpeed;
                _pitch -= deltaY * _rotationSpeed;
                _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);

                _previousMouse = mouse;
            }
            else
            {
                _firstClick = true;
            }

            // === Direction vectors ===
            Vector3 forward = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0));
            Vector3 right = Vector3.Cross(forward, Vector3.Up);

            // === Keyboard movement ===
            if (keyboard.IsKeyDown(Keys.W)) Move(forward * _moveSpeed * delta);
            if (keyboard.IsKeyDown(Keys.S)) Move(-forward * _moveSpeed * delta);
            if (keyboard.IsKeyDown(Keys.A)) Move(-right * _moveSpeed * delta);
            if (keyboard.IsKeyDown(Keys.D)) Move(right * _moveSpeed * delta);
            if (keyboard.IsKeyDown(Keys.Space)) Move(Vector3.Up * _moveSpeed * delta);
            if (keyboard.IsKeyDown(Keys.LeftControl)) Move(-Vector3.Up * _moveSpeed * delta);

            // === Zoom (scroll wheel) ===
            int scrollDelta = mouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;
            if (scrollDelta != 0)
                Move(forward * scrollDelta * _zoomSpeed * delta * 0.1f);

            Target = Position + forward;
            UpdateViewMatrix();
        }

        public void Move(Vector3 delta)
        {
            Position += delta;
        }

        public void Rotate(float yawDelta, float pitchDelta)
        {
            _yaw += yawDelta;
            _pitch = MathHelper.Clamp(_pitch + pitchDelta, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);
        }

        public void UpdateViewMatrix()
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
        }

        public void UpdateProjectionMatrix()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, 0.1f, 1000f);
        }

        public void UpdateAspectRatio(float newAspectRatio)
        {
            _aspectRatio = newAspectRatio;
            UpdateProjectionMatrix();
        }
    }
}
