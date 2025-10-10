using System;
using System.IO;
using System.Windows.Forms;
using EditorOfficial.Helpers; // ✅ for InputState

namespace EditorOfficial
{
    public partial class FormEditor : Form
    {
        private GameEditor _game;

        public FormEditor()
        {
            InitializeComponent();
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Initializing game engine...";

            try
            {
                var handle = splitContainer1.Panel2.Handle;
                _game = new GameEditor(handle);
                _game.RunOneFrame();

                // ✅ Hook WinForms input events to InputState
                HookInputEvents();

                // ✅ Run MonoGame frames when the app is idle
                Application.Idle += GameLoop;

                toolStripStatusLabel1.Text = "Game engine initialized.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing game: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Handles resizing of the embedded render surface
        private void FormEditor_SizeChanged(object sender, EventArgs e)
        {
            _game?.UpdateAspectRatio(splitContainer1.Panel2.Width, splitContainer1.Panel2.Height);
        }

        // ✅ Main MonoGame frame loop
        private void GameLoop(object sender, EventArgs e)
        {
            while (AppStillIdle)
                _game.Tick();
        }

        private bool AppStillIdle
        {
            get
            {
                NativeMethods.PeekMessage(out var msg, IntPtr.Zero, 0, 0, 0);
                return msg.message == 0;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _game?.Exit();
            base.OnFormClosing(e);
        }

        // === Menu actions ===
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _game?.ResetScene();
            toolStripStatusLabel1.Text = "New scene created.";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Scene Files (*.json)|*.json",
                FileName = "scene.json"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _game?.SaveScene(dialog.FileName);
                toolStripStatusLabel1.Text = $"Scene saved: {Path.GetFileName(dialog.FileName)}";
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Scene Files (*.json)|*.json"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _game?.LoadScene(dialog.FileName);
                toolStripStatusLabel1.Text = $"Loaded: {Path.GetFileName(dialog.FileName)}";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ✅ New: hook up WinForms input events to InputState helper
        private void HookInputEvents()
        {
            var panel = splitContainer1.Panel2;

            panel.MouseDown += (s, e) => InputState.OnMouseDown(e);
            panel.MouseUp += (s, e) => InputState.OnMouseUp(e);
            panel.MouseMove += (s, e) => InputState.OnMouseMove(e);
            panel.KeyDown += (s, e) => InputState.OnKeyDown(e.KeyCode);
            panel.KeyUp += (s, e) => InputState.OnKeyUp(e.KeyCode);

            // ✅ Let panel capture focus for keyboard
            panel.TabStop = true;
            panel.Focus();
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin,
            uint messageFilterMax, uint flags);
    }
}
