using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorOfficial
{
    public class Level
    {
        public string Name { get; private set; }
        private List<ModelEntity> _entities = new List<ModelEntity>();

        public Level(string name)
        {
            Name = name;
        }

        public void AddEntity(ModelEntity entity)
        {
            _entities.Add(entity);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection)
        {
            foreach (var entity in _entities)
            {
                entity.Draw(device, effect, view, projection);
            }
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            foreach (var entity in _entities)
                entity.LoadContent(content);
        }
    }
}
