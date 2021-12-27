using Blocks.Common.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Readers
{
    /// <summary>
    /// Read a BlockAssembly from a Rhino file.
    /// </summary>
    public class ReadBlockAssembly
    {
        public ReadBlockAssembly()
        {
        }

        public BlockAssembly Read(List<BlockInstance> instances, double distanceThreshold)
        {
            //Create an empty assembly
            var assembly = new BlockAssembly();

            //Create connectivitiy graph
            for (var i = 0; i < instances.Count; i++)
            {
                assembly.AddInstance(instances[i]);
                for (var j = i + 1; j < instances.Count; j++)
                {
                    //Compare instances - are they touching?
                    if (TransformOriginDistance(instances[i].Transform, instances[j].Transform) > distanceThreshold ||
                        !Functions.CollisionCheck.CheckTouching(instances[i], instances[j]))
                    {
                        continue;
                    }

                    //If touching, create an 'edge' (relationship).
                    var relationship = new Relationship(instances[i], instances[j]);
                    assembly.AddRelationship(relationship);
                }
            }
            return assembly;
        }

        private double TransformOriginDistance(Transform a, Transform b)
        {
            var point1 = Point3d.Origin;
            var point2 = Point3d.Origin;

            point1.Transform(a);
            point2.Transform(b);

            return point1.DistanceTo(point2);
        }

        public BlockAssembly Read(List<InstanceObject> instanceObjects, double distanceThreshold)
        {                        
            var instanceObjectBlocks = new Dictionary<InstanceObject, BlockInstance>();

            foreach (var instanceDefinition in instanceObjects.GroupBy(i => i.InstanceDefinition)) {
                
                //Create a new block definition
                var blockDefinition = new BlockDefinition(instanceDefinition.Key);

                //Add the block instances to the assembly
                foreach (var instanceObject in instanceDefinition) {
                    var instance = new BlockInstance(blockDefinition, instanceObject.InstanceXform); 
                    instanceObjectBlocks.Add(instanceObject, instance);
                }
            }

            return Read(instanceObjectBlocks.Values.ToList(), distanceThreshold);
        }
    }
}
