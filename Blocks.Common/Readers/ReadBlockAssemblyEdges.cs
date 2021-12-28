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
                    if (TransformOriginDistance(instances[i].Transform, instances[j].Transform) > distanceThreshold ||
                        !Functions.CollisionCheck.CheckTouching(instances[i], instances[j]))
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

        private double TransformOriginDistance(Transform a, Transform b)
        {
            var point1 = Point3d.Origin;
            var point2 = Point3d.Origin;

            point1.Transform(a);
            point2.Transform(b);

            return point1.DistanceTo(point2);
        }
    }
}
