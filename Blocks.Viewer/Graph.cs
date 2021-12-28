using Blocks.Common.Objects;
using GraphShape.Algorithms.Layout;
using QuikGraph;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Viewer
{
    public class Graph
    {
        public Point3d[] Vertices { get; private set; }
        public Line[] Edges { get; private set; }
        public string[] Labels { get; private set; }

        public Graph()
        {
            
        }

        public void Build(BlockAssembly assembly)
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();

            for (int i =0; i < assembly.BlockInstances.Count; i++)
            {
                var instance = assembly.BlockInstances[i];
                graph.AddVertex(instance.Id);
            }

            foreach (var assemblyEdge in assembly.Edges)
            {
                var edge = new Edge<string>(assemblyEdge.FromInstance.Id, assemblyEdge.ToInstance.Id);
                graph.AddEdge(edge);
            }

            var algorithm = new KKLayoutAlgorithm<string, Edge<string>, IBidirectionalGraph<string, Edge<string>>>(graph);
            algorithm.Compute();

            var points = new List<Point3d>();
            var names = new List<string>();
            var lines = new List<Line>();

            foreach (string s in algorithm.VerticesPositions.Keys)
            {
                names.Add(s);
                GraphShape.Point p = algorithm.VerticesPositions[s];
                points.Add(new Point3d(p.X, p.Y, 0));
            }

            foreach (Edge<string> edge_string in graph.Edges)
            {
                string key_from = edge_string.Source;
                string key_to = edge_string.Target;

                GraphShape.Point p_1 = algorithm.VerticesPositions[key_from];
                GraphShape.Point p_2 = algorithm.VerticesPositions[key_to];

                Line line = new Line(new Point3d(p_1.X, p_1.Y, 0), new Point3d(p_2.X, p_2.Y, 0));
                lines.Add(line);
            }

            Edges = lines.ToArray();
            Vertices = points.ToArray();
            Labels = names.Select(id => assembly.BlockInstances.First(b => b.Id == id).BlockDefinition.Name).ToArray();
        }
    }
}
