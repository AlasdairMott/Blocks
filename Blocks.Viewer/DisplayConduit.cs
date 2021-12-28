using Eto.Forms;
using Rhino.Display;
using System;
using Rhino.Geometry;
using System.Drawing;

namespace Blocks.Viewer
{
    public class DisplayConduit : Rhino.Display.DisplayConduit
    {
        private BlocksViewport _parent;
        private BlockAssemblyInstance _instance = MainForm.BlockAssemblyReference;
        public DisplayConduit(BlocksViewport parent)
        {
            _parent = parent;
            _parent.BlockVisibiltyDropdown.SelectedIndexChanged += BlockVisibiltyDropdown_SelectedIndexChanged;
            _parent.DisplayStyleDropdown.SelectedIndexChanged += DisplayStyleDropdown_SelectedIndexChanged;

            MainForm.BlockAssemblyInstanceChanged += MainForm_BlockAssemblyInstanceChanged;
            MainForm.BlockAssemblyReferenceChanged += MainForm_BlockAssemblyReferenceChanged;
        }

        private void DisplayStyleDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var viewport = _parent.ViewportControl.Viewport;
            if ((sender as DropDown).SelectedIndex == 2)
            {
                viewport.SetProjection(DefinedViewportProjection.Top, "Top", true);
            } 
            else
            {
                viewport.SetProjection(DefinedViewportProjection.Perspective, null, true);
                viewport.Camera35mmLensLength = 50;
            }
            _parent.ViewportControl.Refresh();
        }

        private void BlockVisibiltyDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as DropDown).SelectedIndex)
            {
                case 0:
                    _instance = MainForm.BlockAssemblyReference; break;
                case 1:
                    _instance =  MainForm.BlockAssemblyInstance; break;
            }
            _parent.ViewportControl.Refresh();
        }

        private void MainForm_BlockAssemblyInstanceChanged(object sender, EventArgs e)
        {
            if (_parent.BlockVisibiltyDropdown.SelectedIndex == 1)
            {
                _instance = MainForm.BlockAssemblyInstance;
            }
        }
        private void MainForm_BlockAssemblyReferenceChanged(object sender, EventArgs e)
        {
            if (_parent.BlockVisibiltyDropdown.SelectedIndex == 0)
            {
                _instance = MainForm.BlockAssemblyReference;
            }
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (e.Viewport.Name != _parent.ViewportControl.Viewport.Name) return;

            base.CalculateBoundingBox(e);
            if (_instance != null)
            {
                var unitBox = new BoundingBox(new Polyline { Point3d.Origin, new Point3d(1, 1, 1) });
                e.IncludeBoundingBox(unitBox);
                e.IncludeBoundingBox(_instance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _parent.ViewportControl.Viewport.Name) return;

            base.PostDrawObjects(e);

            if (_instance != null)
            {
                var mode = (BlockAssemblyDisplayMode) _parent.DisplayStyleDropdown.SelectedIndex;
                _instance.PostDraw(e, mode);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _parent.ViewportControl.Viewport.Name) return;

            base.PreDrawObjects(e);

            if (_instance != null)
            {
                var mode = (BlockAssemblyDisplayMode)_parent.DisplayStyleDropdown.SelectedIndex;
                _instance.PreDraw(e, mode);
            }
        }

        public void ZoomExtents() { 
            if (_instance != null)
            {
                _parent.ViewportControl.Viewport.ZoomBoundingBox(_instance.BoundingBox);
                _parent.ViewportControl.Viewport.Camera35mmLensLength = 50;
            }
        }
    }
}
