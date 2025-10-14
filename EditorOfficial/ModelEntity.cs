using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EditorOfficial
{
    public class ModelEntity
    {
        public string Name { get; set; }
        public Model Model { get; private set; }
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;

        public BoundingSphere Bounding => Model?.Meshes[0].BoundingSphere.Transform(WorldMatrix) ?? new BoundingSphere();

        private Matrix WorldMatrix =>
            Matrix.CreateScale(Scale) *
            Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
            Matrix.CreateTranslation(Position);

        public ModelEntity(string name, Model model)
        {
            Name = name;
            Model = model;
        }

        public void Update(GameTime gameTime) { /* transforms later */ }

        public void Draw(GraphicsDevice device, Camera cam)
        {
            foreach (var mesh in Model.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.World = WorldMatrix;
                    fx.View = cam.View;
                    fx.Projection = cam.Projection;
                    fx.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
