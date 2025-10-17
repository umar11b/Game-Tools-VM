using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EditorOfficial
{
    public class Level
    {
        public string Name { get; set; }
        public List<ModelEntity> Entities { get; set; } = new List<ModelEntity>();

        public Level() { }
        public Level(string name) { Name = name; }

        public void AddEntity(ModelEntity e) => Entities.Add(e);

        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection)
        {
            foreach (var e in Entities)
                e.Draw(device, effect, view, projection);
        }
    }
}
