using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Rhino.Display;
using Rhino.UI.Controls;

namespace Blocks.Viewer
{
    public class BlocksViewport : Panel
    {
        public ViewportControl ViewportControl { get; private set; }
        public DisplayConduit DisplayConduit { get; private set; }
        public DropDown BlockVisibiltyDropdown { get; private set; }

        public BlocksViewport(string name) : base ()
        {
            ViewportControl = new ViewportControl();

            ViewportControl.Viewport.Name = name;
            SetDisplayMode(ViewportControl.Viewport);

            BlockVisibiltyDropdown = new DropDown()
            {
                Items = { "Reference", "Instance" },
                SelectedIndex = 0,
            };

            DisplayConduit = new DisplayConduit(this);
            DisplayConduit.Enabled = true;

            var layout = new TableLayout()
            {
                Rows = { BlockVisibiltyDropdown, ViewportControl }
            };

            Content = layout;
        }



        private void SetDisplayMode(RhinoViewport viewport)
        {
            viewport.ConstructionGridVisible = false;
            viewport.ConstructionAxesVisible = false;
            viewport.WorldAxesVisible = false;

            DisplayModeDescription displayMode = DisplayModeDescription.GetDisplayModes().FirstOrDefault(d => d.EnglishName == "Blocks.Viewer");
            if (displayMode == null)
            {
                var displayId = DisplayModeDescription.CopyDisplayMode(DisplayModeDescription.ShadedId, "Blocks.Viewer");
                displayMode = DisplayModeDescription.GetDisplayMode(displayId);

                displayMode.DisplayAttributes.FillMode = DisplayPipelineAttributes.FrameBufferFillMode.SolidColor;
                displayMode.DisplayAttributes.SetFill(System.Drawing.Color.White);

                DisplayModeDescription.UpdateDisplayMode(displayMode);
            }

            viewport.DisplayMode = displayMode;
        }
    }
}
