using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace EditorOfficial.Helpers
{
    public static class InputState
    {
        public static bool RightMouseDown { get; private set; }
        public static int MouseX { get; private set; }
        public static int MouseY { get; private set; }
        public static int DeltaX { get; private set; }
        public static int DeltaY { get; private set; }

        private static int _lastX;
        private static int _lastY;

        public static bool W, A, S, D, Space, Ctrl;

        public static void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) RightMouseDown = true;
        }

        public static void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) RightMouseDown = false;
        }

        public static void OnMouseMove(MouseEventArgs e)
        {
            MouseX = e.X;
            MouseY = e.Y;
            DeltaX = MouseX - _lastX;
            DeltaY = MouseY - _lastY;
            _lastX = MouseX;
            _lastY = MouseY;
        }

        public static void OnKeyDown(Keys key)
        {
            if (key == Keys.W) W = true;
            if (key == Keys.S) S = true;
            if (key == Keys.A) A = true;
            if (key == Keys.D) D = true;
            if (key == Keys.Space) Space = true;
            if (key == Keys.ControlKey) Ctrl = true;
        }

        public static void OnKeyUp(Keys key)
        {
            if (key == Keys.W) W = false;
            if (key == Keys.S) S = false;
            if (key == Keys.A) A = false;
            if (key == Keys.D) D = false;
            if (key == Keys.Space) Space = false;
            if (key == Keys.ControlKey) Ctrl = false;
        }

        public static void ResetMouseDelta()
        {
            DeltaX = 0;
            DeltaY = 0;
        }
    }
}
