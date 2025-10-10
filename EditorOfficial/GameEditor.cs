using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text.Json;

namespace EditorOfficial
{
    public class GameEditor : Game
    {
        private static GameEditor _instance;
        public static GameEditor Instance => _instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Handle to the WinForms panel
        private IntPtr _drawSurface;

        private Camera _camera;
        private BasicEffect _effect;
        private VertexPositionColor[] _axisLines;

        // === Level system ===
        private Level _level;

        public void RunOneFrame()
        {
            base.RunOneFrame();
        }

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _camera = new Camera(GraphicsDevice.Viewport.AspectRatio);
            _effect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            // === Initialize default level ===
            _level = new Level("Solar System");
            var teapot = new ModelEntity("teapot") { Position = new Vector3(0, 0, 0) };
            _level.AddEntity(teapot);
            _level.LoadContent(Content);

            // Axes
            _axisLines = new VertexPositionColor[]
            {
                new VertexPositionColor(Vector3.Zero, Color.Red),
                new VertexPositionColor(Vector3.UnitX * 10, Color.Red),
                new VertexPositionColor(Vector3.Zero, Color.Green),
                new VertexPositionColor(Vector3.UnitY * 10, Color.Green),
                new VertexPositionColor(Vector3.Zero, Color.Blue),
                new VertexPositionColor(Vector3.UnitZ * 10, Color.Blue)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            _camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 40, 70));

            _effect.World = Matrix.Identity;
            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _axisLines, 0, 3);
            }

            _level?.Draw(GraphicsDevice, _effect, _camera.View, _camera.Projection);

            base.Draw(gameTime);
        }

        // === Scene actions ===
        public void ResetScene()
        {
            _level = new Level("New Scene");
        }

        public void SaveScene(string path)
        {
            try
            {
                var json = JsonSerializer.Serialize(_level, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error saving scene:\n{ex.Message}");
            }
        }

        public void LoadScene(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                var json = File.ReadAllText(path);
                _level = JsonSerializer.Deserialize<Level>(json);
                _level?.LoadContent(Content);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error loading scene:\n{ex.Message}");
            }
        }

        public void UpdateAspectRatio(int width, int height)
        {
            if (GraphicsDevice == null || height == 0)
                return;

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }
    }
}
