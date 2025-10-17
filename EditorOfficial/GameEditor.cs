using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms; // For Timer fallback

namespace EditorOfficial
{
    public class GameEditor : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IntPtr _drawSurface;
        private Camera _camera;
        private BasicEffect _effect;

        // Logical level data
        private Level _level = new Level("Untitled");
        public Level Level
        {
            get => _level;
            set
            {
                _level = value ?? new Level("Untitled");
                Entities = _level.Entities ?? new List<ModelEntity>();
                ReloadLevelModels();
            }
        }

        public List<ModelEntity> Entities
        {
            get => Level.Entities;
            set => Level.Entities = value;
        }

        public ModelEntity SelectedEntity;
        public event Action<ModelEntity> OnEntitySelected;

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

                if (GraphicsDevice != null)
                {
                    LoadContent();
                }
                else
                {
                    // Fallback timer to wait for GraphicsDevice
                    var retryTimer = new System.Windows.Forms.Timer
                    {
                        Interval = 100
                    };
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
            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false
            };

            // If this is a brand-new level, try adding a default model
            if (Entities.Count == 0)
                TryAddDefaultEntity();

            ReloadLevelModels();
        }

        private void TryAddDefaultEntity()
        {
            try
            {
                var model = Content.Load<Model>("Teapot");
                Entities.Add(new ModelEntity(model));
            }
            catch
            {
                // If Teapot not found, scene will simply be empty
            }
        }

        private void ReloadLevelModels()
        {
            if (Content == null)
                return;

            foreach (var entity in Entities)
            {
                if (entity.Model == null)
                {
                    try
                    {
                        // Future-proof: replace with entity.ModelPath when serialization supports it
                        entity.Model = Content.Load<Model>("Teapot");
                    }
                    catch
                    {
                        // Ignore missing models gracefully
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                HandleSelection();

            base.Update(gameTime);
        }

        private void HandleSelection()
        {
            if (Entities.Count == 0)
                return;

            var entity = Entities[0];
            if (SelectedEntity != entity)
            {
                SelectedEntity = entity;
                entity.Selected = true;
                OnEntitySelected?.Invoke(entity);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GraphicsDevice == null)
                return;

            GraphicsDevice.Clear(new Color(25, 40, 80));

            foreach (var entity in Entities)
            {
                entity.Draw(GraphicsDevice, _effect, _camera.View, _camera.Projection);
            }

            base.Draw(gameTime);
        }
    }
}
