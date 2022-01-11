using Blocks.Common.Objects;
using Rhino.Geometry;
using System.Collections.Generic;

namespace Blocks.Common.Readers
{
    public class ReadBlockAssemblyEdges
    {
        public ReadBlockAssemblyEdges() { }

        public List<Edge> Read(BlockAssembly blockAssembly, double distanceThreshold)
        {
            var instances = blockAssembly.BlockInstances;
            var edges = new List<Edge>();

            //Create connectivitiy graph
            for (var i = 0; i < instances.Count; i++)
            {
                for (var j = i + 1; j < instances.Count; j++)
                {
                    //Compare instances - are they touching?
                    if (instances[i].DistanceTo(instances[j]) > distanceThreshold ||
                        !Functions.CollisionCheck.CheckTouching(instances[i], instances[j], 2.0))
                    {
                        continue;
                    }

                    //If touching, create an 'edge' (relationship).
                    var edge = new Edge(instances[i], instances[j]);
                    edges.Add(edge);
                }
            }

            return edges;
        }
    }
}
