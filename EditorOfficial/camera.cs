using Microsoft.Xna.Framework;

namespace EditorOfficial
{
    public class Camera
    {
        public Vector3 Position = new Vector3(0, 0, -20);
        public Vector3 Target = Vector3.Zero;
        public Matrix View => Matrix.CreateLookAt(Position, Target, Vector3.Up);
        public Matrix Projection { get; private set; }

        public Camera(float aspectRatio)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 1000f);
        }

        public void Update() { }
    }
}
