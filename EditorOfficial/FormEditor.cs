using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace EditorOfficial
{
    public partial class FormEditor : Form
    {
        private GameEditor _game;
        private bool _selectionDirty = false;
        private string _currentProjectPath = null;

        public FormEditor()
        {
            InitializeComponent();
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            var handle = splitContainer1.Panel1.Handle; // ✅ ensure game renders on left
            _game = new GameEditor(handle);
            _game.Start();  // replaces Initialize() + LoadContent()
            _game.AddDefaultEntity();
            _game.OnEntitySelected += HandleEntitySelected;

            Application.Idle += GameLoop;
            toolStripStatusLabel1.Text = "Game engine initialized.";
        }

        private void GameLoop(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                _game.Tick();

                // ✅ defensive check ensures propertyGrid1 exists at runtime
                if (_selectionDirty && this.propertyGrid1 != null)
                {
                    this.propertyGrid1.Refresh();
                    _selectionDirty = false;
                }
            }
        }

        private void HandleEntitySelected(ModelEntity entity)
        {
            if (this.propertyGrid1 != null)
            {
                this.propertyGrid1.SelectedObject = entity;
                entity.PropertyChanged += (s, ev) => _selectionDirty = true;
            }
        }

        private bool AppStillIdle
        {
            get
            {
                NativeMethods.PeekMessage(out var msg, IntPtr.Zero, 0, 0, 0);
                return msg.message == 0;
            }
        }

        private void FormEditor_SizeChanged(object sender, EventArgs e)
        {
            // Optional resize handling
        }

        // 🧱 ---------- MENU ITEM HANDLERS ----------

        // "File > Project > Create"
        private void menuItemCreate_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Open City Editor Project (*.oce)|*.oce";
                dlg.Title = "Create New Project";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _currentProjectPath = dlg.FileName;

                    // Create new empty Level
                    _game.Level = new Level("Untitled");
                    SaveProject(_currentProjectPath);

                    toolStripStatusLabel1.Text = $"Created new project: {_currentProjectPath}";
                    this.Text = $"Editor - {Path.GetFileName(_currentProjectPath)}";
                }
            }
        }

        // "File > Project > Load"
        private void menuItemLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Open City Editor Project (*.oce)|*.oce";
                dlg.Title = "Open Project";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _currentProjectPath = dlg.FileName;
                    LoadProject(_currentProjectPath);

                    toolStripStatusLabel1.Text = $"Loaded project: {_currentProjectPath}";
                    this.Text = $"Editor - {Path.GetFileName(_currentProjectPath)}";
                }
            }
        }

        // "File > Project > Save"
        private void menuItemSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentProjectPath))
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Open City Editor Project (*.oce)|*.oce";
                    dlg.Title = "Save Project As";

                    if (dlg.ShowDialog() == DialogResult.OK)
                        _currentProjectPath = dlg.FileName;
                    else
                        return;
                }
            }

            SaveProject(_currentProjectPath);
            toolStripStatusLabel1.Text = $"Project saved: {_currentProjectPath}";
        }

        // 🧠 ---------- FILE I/O HELPERS ----------

        private void SaveProject(string path)
        {
            if (_game?.Level == null)
                return;

            try
            {
                var json = JsonSerializer.Serialize(_game.Level, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save project:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProject(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                var loadedLevel = JsonSerializer.Deserialize<Level>(json);

                if (loadedLevel != null)
                {
                    _game.Level = loadedLevel;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load project:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Dummy for unhandled menu item
        private void toolStripMenuItem1_Click(object sender, EventArgs e) { }
    }

    // ✅ Win32 helper
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd,
            uint messageFilterMin, uint messageFilterMax, uint flags);
    }
}
