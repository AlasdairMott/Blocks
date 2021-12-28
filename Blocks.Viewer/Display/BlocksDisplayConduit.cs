using Rhino.Display;
using Rhino.Geometry;
using System;

namespace Blocks.Viewer.Display
{
    public class BlocksDisplayConduit : DisplayConduit
    {
        private BlocksViewport _parent;
        public IDrawable Instance { get; set; } = MainForm.BlockAssemblyReference;

        public BlocksDisplayConduit(BlocksViewport parent)
        {
            _parent = parent;

            MainForm.BlockAssemblyInstanceChanged += MainForm_BlockAssemblyInstanceChanged;
            MainForm.BlockAssemblyReferenceChanged += MainForm_BlockAssemblyReferenceChanged;
        }
        private void MainForm_BlockAssemblyInstanceChanged(object sender, EventArgs e)
        {
            if (_parent.BlockVisibiltyDropdown.SelectedIndex == 1)
            {
                Instance = MainForm.BlockAssemblyInstance;
            }
        }
        private void MainForm_BlockAssemblyReferenceChanged(object sender, EventArgs e)
        {
            if (_parent.BlockVisibiltyDropdown.SelectedIndex == 0)
            {
                Instance = MainForm.BlockAssemblyReference;
            }
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.CalculateBoundingBox(e);
            if (Instance != null)
            {
                var unitBox = new BoundingBox(new Polyline { Point3d.Origin, new Point3d(1, 1, 1) });
                e.IncludeBoundingBox(unitBox);
                e.IncludeBoundingBox(Instance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.PostDrawObjects(e);

            if (Instance != null)
            {
                Instance.PostDraw(e);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Id != _parent.ViewportControl.Viewport.Id) return;

            base.PreDrawObjects(e);

            if (Instance != null)
            {
                Instance.PreDraw(e);
            }
        }

        public void ZoomExtents() { 
            if (Instance != null)
            {
                _parent.ViewportControl.Viewport.ZoomBoundingBox(Instance.BoundingBox);
                _parent.ViewportControl.Viewport.Camera35mmLensLength = 50;
            }
        }
    }
}
