using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // === Draw model ===
        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection)
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
                    meshEffect.View = view;
                    meshEffect.Projection = projection;
                    meshEffect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
