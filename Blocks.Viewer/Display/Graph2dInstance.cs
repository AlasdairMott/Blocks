using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;

namespace Blocks.Viewer.Display
{
    public class Graph2dInstance : IDrawable
    {
        private Graph2d _graph;

        public BoundingBox BoundingBox { get; private set; } = BoundingBox.Empty;

        public Graph2dInstance(Graph2d graph)
        {
            _graph = graph;

            foreach (var line in graph.Edges) BoundingBox.Union(line.BoundingBox);
        }

        public void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawLines(_graph.Edges, Color.Black, 2);
        }

        public void PostDraw(DrawEventArgs e)
        {
        }
    }
}
