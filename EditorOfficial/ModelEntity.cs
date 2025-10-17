using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;

namespace EditorOfficial
{
    public class ModelEntity : INotifyPropertyChanged
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private float _scale = 1f;
        private bool _selected;
        private string _diffuseTexture = "Metal";

        [Browsable(false)]
        public Model Model { get; set; }

        [Category("Appearance")]
        [Description("Diffuse texture")]
        [TypeConverter(typeof(DiffuseTextureConverter))]
        public string DiffuseTexture
        {
            get => _diffuseTexture;
            set
            {
                if (_diffuseTexture != value)
                {
                    _diffuseTexture = value;
                    OnPropertyChanged(nameof(DiffuseTexture));
                }
            }
        }

        [Category("State")]
        [Description("Is this entity selected")]
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        [Category("Transform")]
        [Description("World position")]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        [Category("Transform")]
        [Description("Rotation (pitch, yaw, roll)")]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        [Category("Transform")]
        [Description("Scale")]
        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        [Browsable(false)]
        public Matrix World =>
            Matrix.CreateScale(_scale) *
            Matrix.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z) *
            Matrix.CreateTranslation(_position);

        public ModelEntity(Model model)
        {
            Model = model;
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection)
        {
            if (Model == null) return;

            foreach (var mesh in Model.Meshes)
            {
                foreach (BasicEffect e in mesh.Effects)
                {
                    e.World = World;
                    e.View = view;
                    e.Projection = projection;
                    e.EnableDefaultLighting();

                    if (DiffuseTexture == "Metal")
                        e.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
                    else if (DiffuseTexture == "Grass")
                        e.DiffuseColor = new Vector3(0.3f, 0.8f, 0.3f);
                    else if (DiffuseTexture == "HeightMap")
                        e.DiffuseColor = new Vector3(0.5f, 0.5f, 1.0f);
                }

                mesh.Draw();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class DiffuseTextureConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new[] { "Metal", "Grass", "HeightMap" });
        }
    }
}
