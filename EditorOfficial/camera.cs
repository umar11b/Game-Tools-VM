using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EditorOfficial.Helpers;
using System;

namespace EditorOfficial
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; } = Vector3.Up;

        private float _yaw, _pitch;
        private float _distance = 300f;
        private float _aspect;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        private const float RotateSpeed = 0.005f;
        private const float PanSpeed = 1.0f;
        private const float ZoomSpeed = 2.0f;

        public Camera(float aspect)
        {
            _aspect = aspect;
            Reset();
        }

        public void Update(GameTime gameTime)
        {
            InputController.Update();

            // rotation
            if (InputController.RightPressed)
            {
                _yaw -= InputController.MouseDelta.X * RotateSpeed;
                _pitch -= InputController.MouseDelta.Y * RotateSpeed;
                _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);
            }

            // pan (MMB)
            if (InputController.MiddlePressed)
            {
                var right = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(_yaw));
                var up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(_pitch));
                Target -= right * InputController.MouseDelta.X * PanSpeed;
                Target += up * InputController.MouseDelta.Y * PanSpeed;
            }

            // zoom (wheel)
            if (InputController.ScrollDelta != 0)
            {
                _distance -= InputController.ScrollDelta * ZoomSpeed * 0.01f;
                _distance = MathHelper.Clamp(_distance, 50, 2000);
            }

            // keyboard move
            if (InputController.KeyDown(Keys.W)) Target += Vector3.Forward * 2;
            if (InputController.KeyDown(Keys.S)) Target += Vector3.Backward * 2;
            if (InputController.KeyDown(Keys.A)) Target += Vector3.Left * 2;
            if (InputController.KeyDown(Keys.D)) Target += Vector3.Right * 2;

            var rotation = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0);
            var offset = Vector3.Transform(Vector3.Backward, rotation) * _distance;
            Position = Target + offset;
            UpdateViewMatrix();
        }

        public void UpdateViewMatrix() => View = Matrix.CreateLookAt(Position, Target, Up);
        public void UpdateProjectionMatrix() =>
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspect, 0.1f, 10000f);

        public void UpdateAspectRatio(float aspect) { _aspect = aspect; UpdateProjectionMatrix(); }

        public void Reset()
        {
            _yaw = 0; _pitch = 0; _distance = 400;
            Target = Vector3.Zero;
            UpdateProjectionMatrix();
        }
    }
}
