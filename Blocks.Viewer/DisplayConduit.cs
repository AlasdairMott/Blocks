using Rhino.Display;
using System.Drawing;

namespace Blocks.Viewer
{
    public class DisplayConduit : Rhino.Display.DisplayConduit
    {
        private RhinoViewport _viewport;
        public BlockAssemblyInstance Instance { get; set; }
        public DisplayConduit(RhinoViewport viewport)
        {
            _viewport = viewport;
        }
        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (e.Viewport.Name != _viewport.Name) return;

            base.CalculateBoundingBox(e);
            if (Instance != null)
            {
                e.IncludeBoundingBox(Instance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _viewport.Name) return;

            base.PostDrawObjects(e);

            if (Instance != null)
            {
                e.Display.DrawMeshShaded(Instance.Mesh, Instance.Material);
                e.Display.DrawLines(Instance.MeshWires, Color.Black);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            if (e.Viewport.Name != _viewport.Name) return;

            base.PreDrawObjects(e);

            if (Instance != null)
            {
                e.Display.DrawMeshWires(Instance.Mesh, Color.Black, 3);
            }
        }
    }
}
