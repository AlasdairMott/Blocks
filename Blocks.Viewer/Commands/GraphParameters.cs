using System;
using Eto.Forms;
using Eto.Drawing;
using Blocks.Viewer.Data;
using Rhino.UI;
using RhinoWindows;

namespace Blocks.Viewer.Commands
{
    public class GraphParameters : Command
    {
        private Dialogs.GraphParameters _dialog;
        public GraphParameters()
        {
            MenuText = "Graph Parameters";
            ToolBarText = "Graph Parameters";

            Preferences.GraphGeneratorParameters = new Common.Generators.GraphGeneratorParameters(0.05, 5, 0.001, 0.05, 20);
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            if (_dialog == null || _dialog.IsDisposed)
            {
                _dialog = new Dialogs.GraphParameters();
                //dialog.Owner = RhinoEtoApp.MainWindow;
            }

            if (!_dialog.Visible)
            {
                _dialog.Show();
            }

            //var message = Rhino.UI.Dialogs.ShowMessage("", "");
            //var form = new Blocks.Viewer.Dialogs.InvisibleForm();
            //form.Show(RhinoWinApp.MainWindow);
            //var window = Rhino.UI.RhinoEtoApp.MainWindow;
            //Rhino.UI.EtoExtensions.ShowSemiModal()
            //System.Windows.Forms.MessageBox.Show(, "test");
            //var message = System.Windows.Forms.MessageBox.Show("text");
        }
    }
}
