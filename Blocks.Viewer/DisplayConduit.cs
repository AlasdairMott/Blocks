using Eto.Forms;
using Rhino.Display;
using System;
using System.Drawing;

namespace Blocks.Viewer
{
    public class DisplayConduit : Rhino.Display.DisplayConduit
    {
        private BlocksViewport _parent;
        private BlockAssemblyInstance _instance = MainForm.BlockAssemblyReference;
        private DropDown _blockVisibiltyDropdown;
        public DisplayConduit(BlocksViewport parent)
        {
            _parent = parent;

            _blockVisibiltyDropdown = _parent.BlockVisibiltyDropdown;

            _blockVisibiltyDropdown.SelectedIndexChanged += BlockVisibiltyDropdown_SelectedIndexChanged;

            MainForm.BlockAssemblyInstanceChanged += MainForm_BlockAssemblyInstanceChanged;
            MainForm.BlockAssemblyReferenceChanged += MainForm_BlockAssemblyReferenceChanged;
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
            if (_blockVisibiltyDropdown.SelectedIndex == 1)
            {
                _instance = MainForm.BlockAssemblyInstance;
            }
        }
        private void MainForm_BlockAssemblyReferenceChanged(object sender, EventArgs e)
        {
            if (_blockVisibiltyDropdown.SelectedIndex == 0)
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
                e.IncludeBoundingBox(_instance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _parent.ViewportControl.Viewport.Name) return;

            base.PostDrawObjects(e);

            if (_instance != null)
            {
                e.Display.DrawMeshShaded(_instance.Mesh, _instance.Material);
                e.Display.DrawLines(_instance.MeshWires, Color.Black);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _parent.ViewportControl.Viewport.Name) return;

            base.PreDrawObjects(e);

            if (_instance != null)
            {
                e.Display.DrawMeshWires(_instance.Mesh, Color.Black, 3);
            }
        }

        public void ZoomExtents() { 
            if (_instance != null)
            {
                _parent.ViewportControl.Viewport.ZoomBoundingBox(_instance.BoundingBox);
            }
        }
    }
}
