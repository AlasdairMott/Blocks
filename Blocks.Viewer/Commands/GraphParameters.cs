using Blocks.Viewer.Data;
using Eto.Forms;
using System;

namespace Blocks.Viewer.Commands
{
    public class GraphParameters : Command
    {
        private Dialogs.GraphParameters _dialog;
        public GraphParameters()
        {
            MenuText = "Graph Parameters";
            ToolBarText = "Graph Parameters";

            Preferences.GraphGeneratorParameters = new Common.Generators.GraphGeneratorParameters(0.200, 5, 0.5, 10.0, 0.995, 200);
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);

            if (_dialog == null || _dialog.IsDisposed)
            {
                _dialog = new Dialogs.GraphParameters();
            }

            if (!_dialog.Visible)
            {
                _dialog.Show();
            }
        }
    }
}
