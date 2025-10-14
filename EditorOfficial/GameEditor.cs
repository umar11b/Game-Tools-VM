using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using EditorOfficial.Helpers;

namespace EditorOfficial
{
    /// <summary>
    /// Core editor runtime – handles rendering, camera, and entity management.
    /// </summary>
    public class GameEditor : Game
    {
        private Model _testModel;

        private static GameEditor _instance;
        public static GameEditor Instance => _instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Handle to WinForms panel
        private IntPtr _drawSurface;

        // Scene
        private Camera _camera;
        private BasicEffect _effect;
        private List<ModelEntity> _entities;

        // Debug font
        private SpriteFont _font;
        private Vector2 _fontPos = new Vector2(10, 10);

        // Axis visuals
        private VertexPositionColor[] _axisLines;

        public GameEditor(IntPtr drawSurface)
        {
            _instance = this;
            _drawSurface = drawSurface;

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
            _entities = new List<ModelEntity>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load debug font (optional)
            try { _font = Content.Load<SpriteFont>("Arial"); }
            catch { _font = null; }

            // Setup camera
            _camera = new Camera(GraphicsDevice.Viewport.AspectRatio);

            // Basic effect
            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false
            };

            // Axis lines
            _axisLines = new VertexPositionColor[]
            {
                new VertexPositionColor(Vector3.Zero, Color.Red),
                new VertexPositionColor(Vector3.UnitX * 100, Color.Red),
                new VertexPositionColor(Vector3.Zero, Color.Green),
                new VertexPositionColor(Vector3.UnitY * 100, Color.Green),
                new VertexPositionColor(Vector3.Zero, Color.Blue),
                new VertexPositionColor(Vector3.UnitZ * 100, Color.Blue)
            };
            _testModel = Content.Load<Model>("Teapot");

        }

        protected override void Update(GameTime gameTime)
        {
            // Exit on ESC
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // === Handle input ===
            InputController.Update();

            // === Camera movement test (WASD) ===
            if (InputController.KeyDown(Keys.W))
                _camera.Position += new Vector3(0, 0, -2);
            if (InputController.KeyDown(Keys.S))
                _camera.Position += new Vector3(0, 0, 2);
            if (InputController.KeyDown(Keys.A))
                _camera.Position += new Vector3(-2, 0, 0);
            if (InputController.KeyDown(Keys.D))
                _camera.Position += new Vector3(2, 0, 0);

            // === Update camera ===
            _camera.Update(gameTime);

            // === Update all entities ===
            foreach (var e in _entities)
                e.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;
            _effect.World = Matrix.Identity;

            // === Draw axes ===
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _axisLines, 0, 3);
            }
            if (_testModel != null)
            {
                Matrix world = Matrix.CreateScale(5f) * Matrix.CreateTranslation(Vector3.Zero);
                foreach (var mesh in _testModel.Meshes)
                {
                    foreach (BasicEffect meshEffect in mesh.Effects)
                    {
                        meshEffect.World = world;
                        meshEffect.View = _camera.View;
                        meshEffect.Projection = _camera.Projection;
                        meshEffect.EnableDefaultLighting();
                    }
                    mesh.Draw();
                }
            }


            // === Draw models ===
            foreach (var e in _entities)
                e.Draw(GraphicsDevice, _camera);

            // === Debug overlay ===
            if (_font != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, $"Camera Pos: {_camera.Position}", _fontPos, Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void UpdateAspectRatio(int width, int height)
        {
            if (GraphicsDevice == null || height == 0)
                return;

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            _camera.UpdateAspectRatio(width / (float)height);
        }

        // === Add methods for entities ===
        public void AddEntity(ModelEntity e) => _entities.Add(e);
        public void ResetScene() => _entities.Clear();
    }
}
