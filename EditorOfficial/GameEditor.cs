using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using EditorOfficial.Helpers;

namespace EditorOfficial
{
    public class GameEditor : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IntPtr _drawSurface;
        private Camera _camera;
        private BasicEffect _effect;
        private List<ModelEntity> _entities = new();
        private Model _sun, _planet, _moon;
        private SpriteFont _font;
        private VertexPositionColor[] _axis;

        public GameEditor(IntPtr handle)
        {
            _drawSurface = handle;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreparingDeviceSettings += (s, e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _drawSurface;
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _camera = new Camera(GraphicsDevice.Viewport.AspectRatio);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Try loading a debug font if available
            try
            {
                _font = Content.Load<SpriteFont>("Arial16");
            }
            catch
            {
                _font = null;
            }

            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false
            };

            // Draw coordinate axis lines
            _axis = new[]
            {
                new VertexPositionColor(Vector3.Zero, Color.Red),
                new VertexPositionColor(Vector3.UnitX * 100, Color.Red),

                new VertexPositionColor(Vector3.Zero, Color.Green),
                new VertexPositionColor(Vector3.UnitY * 100, Color.Green),

                new VertexPositionColor(Vector3.Zero, Color.Blue),
                new VertexPositionColor(Vector3.UnitZ * 100, Color.Blue)
            };

            // Load assets (safe load)
            TryLoadModel("Sun", out _sun);
            TryLoadModel("Planet", out _planet);
            TryLoadModel("Moon", out _moon);
        }

        private void TryLoadModel(string assetName, out Model model)
        {
            try
            {
                model = Content.Load<Model>(assetName);
                Console.WriteLine($"Loaded model: {assetName}");
            }
            catch
            {
                Console.WriteLine($"⚠️ Could not load model: {assetName}");
                model = null;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _camera.Update(gameTime);

            foreach (var e in _entities)
                e.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(135, 206, 250));

            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;
            _effect.World = Matrix.Identity;

            // Draw XYZ axes
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _axis, 0, 3);
            }

            // Draw test models (Sun, Planet, Moon)
            DrawModel(_sun, Matrix.CreateScale(50f));
            DrawModel(_planet, Matrix.CreateTranslation(200, 0, 0));
            DrawModel(_moon, Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(250, 0, 0));

            // Overlay debug text
            if (_font != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, $"Camera: {_camera.Position}", new Vector2(10, 10), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world)
        {
            if (model == null) return;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.World = world;
                    e.View = _camera.View;
                    e.Projection = _camera.Projection;
                    e.EnableDefaultLighting();
                    e.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }

        // === Added for FormEditor integration ===
        public void UpdateAspectRatio(int width, int height)
        {
            if (GraphicsDevice == null || height == 0)
                return;

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            _camera.UpdateAspectRatio(width / (float)height);
        }

        public void ResetScene()
        {
            _entities.Clear();
        }
    }
}
