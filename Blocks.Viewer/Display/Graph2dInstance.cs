using Blocks.Common.Objects;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace Blocks.Viewer.Display
{
    public class Graph2dInstance : IDrawable, IDrawLabel
    {
        private Graph2d _graph;

        public BoundingBox BoundingBox { get; private set; } = BoundingBox.Empty;
        public Graph2dInstance(Graph2d graph)
        {
            _graph = graph;

            foreach (var line in graph.Edges)
            {
                var union = BoundingBox.Union(BoundingBox, line.BoundingBox);
                BoundingBox = union;
            }

            var min = BoundingBox.Min;
            BoundingBox.Union(min + Vector3d.ZAxis);
        }

        public void PreDraw(DrawEventArgs e)
        {
            e.Display.DrawLines(_graph.Edges, Color.Black, 2);
            e.Display.DrawPoints(_graph.Vertices, PointStyle.Square, 2, Color.Black);
        }

        public void PostDraw(DrawEventArgs e)
        {
        }

        public void DrawLabel(DrawEventArgs e)
        {
            for (int i = 0; i < _graph.Labels.Length; i++)
            {
                Display.DrawLabel.Draw(e.Display, _graph.Vertices[i], _graph.Labels[i]);
            }
        }
    }
}
