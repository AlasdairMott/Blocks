using Rhino.Display;
using System.Drawing;

namespace Blocks.Viewer
{
    public class DisplayConduit : Rhino.Display.DisplayConduit
    {
        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            base.CalculateBoundingBox(e);

            if (MainForm.BlockAssemblyInstance != null)
            {
                e.IncludeBoundingBox(MainForm.BlockAssemblyInstance.BoundingBox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            base.PostDrawObjects(e);

            if (MainForm.BlockAssemblyInstance != null)
            {
                e.Display.DrawMeshShaded(MainForm.BlockAssemblyInstance.Mesh, MainForm.BlockAssemblyInstance.Material);
                e.Display.DrawLines(MainForm.BlockAssemblyInstance.MeshWires, Color.Black);
            }
        }

        protected override void PreDrawObjects(DrawEventArgs e)
        {
            base.PreDrawObjects(e);

            if (MainForm.BlockAssemblyInstance != null)
            {
                e.Display.DrawLines(MainForm.BlockAssemblyInstance.MeshWires, Color.Black, 4);
            }
        }
    }
}
