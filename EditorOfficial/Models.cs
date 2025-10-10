using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorOfficial
{
    [Serializable]
    public class ModelEntity
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public string ModelName { get; set; }

        [NonSerialized]
        private Model _model;

        public ModelEntity(string modelName)
        {
            ModelName = modelName;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _model = content.Load<Model>(ModelName);
        }

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
