using Blocks.Common.Objects;
using Blocks.Common.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators
{
    public class Vertex
    {
        public Point3d Location { get; set; }
        public BlockInstance BlockInstance { get; }
        public List<Vertex> Neighbours { get; set; } = new List<Vertex>();

        public Vertex(BlockInstance blockInstance)
        {
            Location = new Point3d(blockInstance.InsertionPoint.X, blockInstance.InsertionPoint.Y, 0);
            BlockInstance = blockInstance;
        }
    }
    public class GraphGenerator
    {
        public GraphGenerator() { }

        public Graph2d Generate(BlockAssembly assembly, GraphGeneratorParameters parameters)
        {
            var vertices = CreateStartingLayout(assembly.Edges);

            int i = 0;
            double maxForce = double.PositiveInfinity;

            var forces = new Vector3d[vertices.Length];
            var temperature = 1.0;

            while (i < parameters.MaxIterations)
            {
                for (int j = 0; j < vertices.Length; j++)
                {
                    var F_A = ComputeAttractiveForce(vertices[j], parameters.SpringConstant, parameters.RestingLength);
                    var F_R = ComputerRespulsiveForce(vertices[j],vertices, parameters.RepsulionRadius);
                    forces[j] = F_A + parameters.RepulsionFactor * F_R;
                    maxForce = Math.Max(maxForce, forces[j].Length);
                }

                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j].Location += temperature * forces[j];
                }

                temperature *= parameters.CoolingFactor;

                i++;
            }

            var lines = LinesFromVertices(vertices);

            return new Graph2d(
                vertices.Select(v => v.Location),
                lines, 
                vertices.Select(v => v.BlockInstance.BlockDefinition.Name));
        }

        public Vertex[] CreateStartingLayout(IEnumerable<Edge> edges)
        {
            var vertexDictionary = new Dictionary<string, Vertex>();

            foreach (var edge in edges)
            {
                if (!vertexDictionary.ContainsKey(edge.FromInstance.Id))
                {
                    vertexDictionary.Add(edge.FromInstance.Id, new Vertex(edge.FromInstance));
                }

                if (!vertexDictionary.ContainsKey(edge.ToInstance.Id))
                {
                    vertexDictionary.Add(edge.ToInstance.Id, new Vertex(edge.ToInstance));
                }

                var from = vertexDictionary[edge.FromInstance.Id];
                var to = vertexDictionary[edge.ToInstance.Id];

                from.Neighbours.Add(to);
                to.Neighbours.Add(from);
            }
            return vertexDictionary.Values.ToArray();
        }

        public Vector3d ComputeAttractiveForce(Vertex vertex, double springConstant, double restingLength)
        {
            Vector3d force = Vector3d.Zero;
            foreach (var neighbour in vertex.Neighbours)
            {
                var vector = new Vector3d(neighbour.Location - vertex.Location);
                var length = vector.Length;
                vector.Unitize();
                vector *= -springConstant * (length - restingLength);

                force += vector;
            }
            return force *= -1;
        }

        public Vector3d ComputerRespulsiveForce(Vertex vertex, IEnumerable<Vertex> others, double repulsionRadius)
        {
            Vector3d force = Vector3d.Zero;
            foreach (var other in others)
            {
                var vector = new Vector3d(other.Location - vertex.Location);
                var length = vector.Length;
                if (length > repulsionRadius) { continue; }

                //Magnitude should be smaller if the vectors are close to the repulsionRadius Limit.
                vector.Unitize();
                vector *= (repulsionRadius - length);

                force += vector;
            }
            return force * - 1;
        }

        public Line[] LinesFromVertices(IEnumerable<Vertex> vertices)
        {
            var edgesDictionary = new Dictionary<string, Line>();
            foreach (var vertex in vertices)
            {
                foreach (var neighbour in vertex.Neighbours)
                {
                    var key1 = vertex.BlockInstance.Id + neighbour.BlockInstance.Id;
                    var key2 = neighbour.BlockInstance.Id + vertex.BlockInstance.Id;
                    var keyExists = edgesDictionary.ContainsKey(key1) || edgesDictionary.ContainsKey(key2);

                    if (!keyExists)
                    {
                        var line = new Line(vertex.Location, neighbour.Location);
                        edgesDictionary.Add(key1, line);
                    }
                }
            }
            return edgesDictionary.Values.ToArray();
        }
    }
}
