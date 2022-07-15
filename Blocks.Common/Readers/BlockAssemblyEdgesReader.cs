using Blocks.Common.Objects;
using Blocks.Common.Parameters;
using System.Collections.Generic;

namespace Blocks.Common.Readers
{
    public class BlockAssemblyEdgeReader
    {
        public BlockAssemblyEdgeReader() { }

        public List<Edge> Read(BlockAssembly blockAssembly, BlockAssemblyEdgeReaderParameters parameters)
        {
            var instances = blockAssembly.BlockInstances;
            var edges = new List<Edge>();

            //Create connectivitiy graph
            for (var i = 0; i < instances.Count; i++)
            {
                for (var j = i + 1; j < instances.Count; j++)
                {
                    //Compare instances - are they touching?
                    if (instances[i].DistanceTo(instances[j]) > parameters.DistanceThreshold ||
                        !Functions.CollisionCheck.CheckTouching(instances[i], instances[j], parameters.CollisionArea))
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
