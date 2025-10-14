using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;

namespace EditorOfficial
{
    [Serializable]
    public class ModelEntity
    {
        // === Serializable fields ===
        public string ModelName { get; set; }
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;

        // === Optional behavior values (we’ll use these for Sun/Planet/Moon later) ===
        public float SelfRotateSpeed { get; set; } = 0f;   // rotation around own axis
        public float OrbitSpeed { get; set; } = 0f;        // orbiting parent
        public Vector3 OrbitCenter { get; set; } = Vector3.Zero;
        public float OrbitRadius { get; set; } = 0f;

        // === Non-serialized runtime model ===
        [JsonIgnore]
        private Model _model;

        public ModelEntity() { }

        public ModelEntity(string modelName)
        {
            ModelName = modelName;
        }

        // === Load 3D content ===
        public void LoadContent(ContentManager content)
        {
            try
            {
                _model = content.Load<Model>(ModelName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Failed to load model '{ModelName}':\n{ex.Message}",
                    "Model Load Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning
                );
            }
        }

        // === Update rotation/orbit logic ===
        public void Update(GameTime gameTime)
        {
            // Rotate around own center
            var rot = Rotation;
            rot.Y += SelfRotateSpeed;
            Rotation = rot;


            // Orbit around parent (if applicable)
            if (OrbitSpeed != 0 && OrbitRadius > 0)
            {
                float angle = (float)(gameTime.TotalGameTime.TotalSeconds * OrbitSpeed);
                Position = new Vector3(
                    OrbitCenter.X + (float)Math.Cos(angle) * OrbitRadius,
                    OrbitCenter.Y,
                    OrbitCenter.Z + (float)Math.Sin(angle) * OrbitRadius
                );
            }
        }

        // === Draw model ===
        public void Draw(GraphicsDevice device, Camera camera)
        {
            if (_model == null)
                return;

            Matrix world =
                Matrix.CreateScale(Scale) *
                Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                Matrix.CreateTranslation(Position);

            foreach (var mesh in _model.Meshes)
            {
                foreach (BasicEffect meshEffect in mesh.Effects)
                {
                    meshEffect.World = world;
                    meshEffect.View = camera.View;
                    meshEffect.Projection = camera.Projection;
                    meshEffect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
