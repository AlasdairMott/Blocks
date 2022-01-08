using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;
using System.Linq;

namespace Blocks.Viewer.Display
{
    public class Skeleton : BlockAssemblyInstance, IDrawable, IDrawLabel
    {
        private Point3d[] _vertices;
        private Line[] _edges;
        private string[] _labels;

        public Skeleton(BlockAssembly assembly) : base(assembly)
        {
            var vertices = assembly.BlockInstances.Select(b => b.InsertionPoint);
            var labels = assembly.BlockInstances.Select(b => b.BlockDefinition.Name);
            var edges = assembly.Edges.Select(e => new Line(e.FromInstance.InsertionPoint, e.ToInstance.InsertionPoint));

            _vertices = vertices.ToArray();
            _edges = edges.ToArray();
            _labels = labels.ToArray();

            foreach (var line in _edges) BoundingBox.Union(line.BoundingBox);
        }

        public new void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawLines(_edges, Color.Black, 2);
            e.Display.DrawPoints(_vertices, PointStyle.Simple, 2, Color.Black);
            e.Display.DrawLines(_meshWires, Color.FromArgb(100, 0, 0, 0));
        }

        public new void PostDraw(DrawEventArgs e)
        {
        }

        public void DrawLabel(DrawEventArgs e)
        {
            for (int i = 0; i < _labels.Length; i++)
            {
                Display.DrawLabel.Draw(e.Display, _vertices[i], _labels[i]);
            }
        }
    }
}
