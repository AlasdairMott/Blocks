using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;

namespace Blocks.Viewer.Display
{
    public class Graph2dInstance : IDrawable
    {
        public Graph2d Graph { get; private set; }

        public BoundingBox BoundingBox { get; private set; } = BoundingBox.Empty;

        public Graph2dInstance(Graph2d graph)
        {
            Graph = graph;

            foreach (var line in graph.Edges) BoundingBox.Union(line.BoundingBox);
        }

        public void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawLines(Graph.Edges, Color.Black, 2);
        }

        public void PostDraw(DrawEventArgs e)
        {
        }
    }
}
