using System;
using System.Windows.Forms;

namespace EditorOfficial
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Launch the form
            var editorForm = new FormEditor();

            // Run the form’s UI on the main thread
            Application.Run(editorForm);
        }
    }
}
