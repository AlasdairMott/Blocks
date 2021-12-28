using Eto.Forms;
using Rhino.Display;
using Rhino.UI.Controls;
using System;
using System.Linq;
using Blocks.Viewer.Display;

namespace Blocks.Viewer
{
    public class BlocksViewport : Panel
    {
        public ViewportControl ViewportControl { get; private set; }
        public BlocksDisplayConduit BlockDisplayConduit { get; private set; }
        public DropDown BlockVisibiltyDropdown { get; private set; }
        public DropDown DisplayStyleDropdown { get; private set; }

        public BlocksViewport(string name) : base()
        {
            ViewportControl = new ViewportControl();
            ViewportControl.Viewport.Name = name;
            SetDisplayMode(ViewportControl.Viewport);

            BuildViewport();

            BlockDisplayConduit = new BlocksDisplayConduit(this);
            BlockDisplayConduit.Enabled = true;
        }

        private void BuildViewport()
        {
            BlockVisibiltyDropdown = new DropDown()
            {
                Items = { "Reference", "Instance" },
                SelectedIndex = 0,
            };
            BlockVisibiltyDropdown.SelectedIndexChanged += BlockVisibiltyDropdown_SelectedIndexChanged;

            DisplayStyleDropdown = new DropDown()
            {
                Items = { "Solid", "Skeleton", "Graph" },
                SelectedIndex = 0,
            };
            DisplayStyleDropdown.SelectedIndexChanged += DisplayStyleDropdown_SelectedIndexChanged;

            var dropdowns = new DynamicLayout();
            dropdowns.AddRow(new Control[2] { BlockVisibiltyDropdown, DisplayStyleDropdown });
            dropdowns.SizeChanged += Dropdowns_SizeChanged;

            var layout = new TableLayout()
            {
                Rows = { dropdowns, ViewportControl }
            };

            Content = layout;
        }

        private void BlockVisibiltyDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewportControl.Refresh();
        }

        private void DisplayStyleDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var viewport = ViewportControl.Viewport;

            if ((sender as DropDown).SelectedIndex == 2)
            {
                viewport.SetProjection(DefinedViewportProjection.Top, null, true);
            }
            else
            {
                viewport.SetProjection(DefinedViewportProjection.Perspective, null, true);
                viewport.Camera35mmLensLength = 50;
            }
            ViewportControl.Refresh();
        }

        private void Dropdowns_SizeChanged(object sender, EventArgs e)
        {
            var dynamicLayout = sender as DynamicLayout;
            foreach (var item in dynamicLayout.Children)
            {
                item.Width = dynamicLayout.Width / 2;
            }
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
