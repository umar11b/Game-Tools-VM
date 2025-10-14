using EditorOfficial.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace EditorOfficial
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; } = Vector3.Up;

        private float _yaw;
        private float _pitch;
        private float _distance = 300f;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        private const float RotationSpeed = 0.005f;
        private const float PanSpeed = 0.5f;
        private const float ZoomSpeed = 2f;

        private float _aspectRatio;

        public Camera(float aspectRatio)
        {
            _aspectRatio = aspectRatio;
            Reset();
        }

        // 👇 This is the method you’re looking for:
        public void Update(GameTime gameTime)
        {
            InputController.Update(); // poll input

            // Rotate camera (Right Mouse)
            if (InputController.RightPressed)
            {
                _yaw -= InputController.MouseDelta.X * RotationSpeed;
                _pitch -= InputController.MouseDelta.Y * RotationSpeed;
                _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);
            }

            // Zoom camera (Scroll wheel)
            if (InputController.ScrollDelta != 0)
            {
                _distance -= InputController.ScrollDelta * ZoomSpeed * 0.01f;
                _distance = MathHelper.Clamp(_distance, 50f, 2000f);
            }

            // Pan camera (Middle Mouse)
            if (InputController.MiddlePressed)
            {
                var right = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(_yaw));
                var up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(_pitch));
                Vector3 pan = (-right * InputController.MouseDelta.X + up * InputController.MouseDelta.Y) * PanSpeed;
                Target += pan;
            }

            // Recalculate position
            Matrix rotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Vector3 offset = Vector3.Transform(Vector3.Backward, rotation) * _distance;
            Position = Target + offset;

            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
        }

        public void UpdateProjectionMatrix()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, 0.1f, 5000f);
        }

        public void UpdateAspectRatio(float newAspectRatio)
        {
            _aspectRatio = newAspectRatio;
            UpdateProjectionMatrix();
        }

        public void Reset()
        {
            _yaw = 0f;
            _pitch = 0f;
            _distance = 300f;
            Target = Vector3.Zero;
            UpdateProjectionMatrix();
        }
    }
}
