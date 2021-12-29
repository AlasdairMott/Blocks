using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System;
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

            BoundingBox.Union(new Point3d(0, 0, 1));
            BoundingBox.Union(new Point3d(0, 0, -1));
        }

        public void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawLines(_graph.Edges, Color.Black, 2);
            e.Display.DrawPoints(_graph.Vertices, PointStyle.Square, 2, Color.Black);
            foreach (var edge in _graph.Edges)
            {
                e.Display.Draw2dText($"{Math.Round(edge.Length,2)}", Color.Black, edge.PointAt(0.5), true, 18);
            }
        }

        public void PostDraw(DrawEventArgs e)
        {
        }
    }
}
