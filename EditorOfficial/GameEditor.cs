using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace EditorOfficial
{
    public class GameEditor : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        IntPtr _drawSurface;
        Camera _camera;
        BasicEffect _effect;
        public List<ModelEntity> Entities = new();
        public ModelEntity SelectedEntity;
        public event Action<ModelEntity> OnEntitySelected;
        public Level CurrentLevel { get; set; } = new Level("Untitled");


        public void AddDefaultEntity()
        {
            try
            {
                var teapot = Content.Load<Model>("Teapot");
                var entity = new ModelEntity(teapot);
                Entities.Add(entity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load default entity: {ex.Message}");
            }
        }


        public Level Level
        {
            get
            {
                // Convert the GameEditor’s live entities into a Level object
                var level = new Level("CurrentLevel");
                level.Entities = Entities;
                return level;
            }
            set
            {
                // Replace GameEditor’s active entities with the loaded Level
                Entities = value?.Entities ?? new List<ModelEntity>();
            }
        }


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

        public void Start()
        {
            if (!IsActive)
            {
                base.Initialize();

                // Ensure GraphicsDevice is ready before loading content
                if (GraphicsDevice != null)
                {
                    LoadContent();
                }
                else
                {
                    System.Windows.Forms.Timer retryTimer = new System.Windows.Forms.Timer();
                    retryTimer.Interval = 100; // check every 100ms
                    retryTimer.Tick += (s, e) =>
                    {
                        if (GraphicsDevice != null)
                        {
                            retryTimer.Stop();
                            retryTimer.Dispose();
                            LoadContent();
                        }
                    };
                    retryTimer.Start();
                }
            }
        }



        protected override void Initialize()
        {
            _camera = new Camera(GraphicsDevice?.Viewport.AspectRatio ?? 1f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _effect = new BasicEffect(GraphicsDevice) { VertexColorEnabled = true };

            var teapot = Content.Load<Model>("Teapot");
            var entity = new ModelEntity(teapot);
            Entities.Add(entity);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                HandleSelection();

            base.Update(gameTime);
        }

        private void HandleSelection()
        {
            if (Entities.Count == 0) return;
            var teapot = Entities[0];
            if (SelectedEntity != teapot)
            {
                SelectedEntity = teapot;
                teapot.Selected = true;
                OnEntitySelected?.Invoke(teapot);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Prevent null reference crash if device isn't ready
            if (GraphicsDevice == null)
                return;

            GraphicsDevice.Clear(new Color(25, 40, 80));

            foreach (var e in Entities)
                e.Draw(GraphicsDevice, _effect, _camera.View, _camera.Projection);

            base.Draw(gameTime);
        }


    }
}
