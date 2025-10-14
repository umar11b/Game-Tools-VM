using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EditorOfficial
{
    public class Level
    {
        public string Name { get; set; }
        public List<ModelEntity> Entities { get; private set; } = new();

        public Level(string name) { Name = name; }

        public void Add(ModelEntity e) => Entities.Add(e);

        public void LoadContent(GraphicsDevice device) { }

        public void Update(GameTime time)
        {
            foreach (var e in Entities) e.Update(time);
        }

        public void Draw(GraphicsDevice device, Camera cam)
        {
            foreach (var e in Entities) e.Draw(device, cam);
        }
    }
}
