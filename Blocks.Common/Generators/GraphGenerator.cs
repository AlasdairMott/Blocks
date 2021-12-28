using Blocks.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Blocks.Common.Generators
{
    public class GraphGenerator
    {
        public GraphGenerator() { }

        public Graph2d Generate(BlockAssembly assembly, int seed, double distance, double tolerance, int steps)
        {
            var vertices = assembly.BlockInstances.Select(b => b.InsertionPoint);
            var labels = assembly.BlockInstances.Select(b => b.BlockDefinition.Name);
            var edges = assembly.Edges.Select(e => new Line(e.FromInstance.InsertionPoint, e.ToInstance.InsertionPoint));

            return new Graph2d(vertices, edges, labels);
        }
    }
}
