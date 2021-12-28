using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Blocks.Common.Objects
{
    public class Graph2d
    {
        public Point3d[] Vertices { get; private set; }
        public Line[] Edges { get; private set; }
        public string[] Labels { get; private set; }

        public Graph2d(IEnumerable<Point3d> vertices, IEnumerable<Line> edges, IEnumerable<string> labels)
        {
            Vertices = vertices.ToArray();
            Edges = edges.ToArray();
            Labels = labels.ToArray();

            if (Vertices.Length != Labels.Length)
            {
                throw new ArgumentException("Number of vertices and labels must match", nameof(labels));
            }
        }
    }
}
