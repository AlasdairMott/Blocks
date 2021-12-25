using Blocks.Objects;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Readers
{
    /// <summary>
    /// Read a BlockAssembly from a Rhino file.
    /// </summary>
    public class ReadBlockAssembly
    {
        public ReadBlockAssembly()
        {
        }

        public BlockAssembly Read(List<InstanceObject> instances, double distanceThreshold)
        {            
            //Create an empty assembly
            var assembly = new BlockAssembly();
            
            var instanceObjectBlocks = new Dictionary<InstanceObject, BlockInstance>();

            foreach (var instanceDefinition in instances.GroupBy(i => i.InstanceDefinition)) {
                
                //Create a new block definition
                var blockDefinition = new BlockDefinition(instanceDefinition.Key);

                //Add the block instances to the assembly
                foreach (var instanceObject in instanceDefinition) {
                    var instance = new BlockInstance(blockDefinition, instanceObject.InstanceXform); 
                    assembly.AddInstance(instance);
                    instanceObjectBlocks.Add(instanceObject, instance);
                }
            }

            //Create connectivitiy graph
            for (var i = 0; i < instances.Count; i++)
            {
                for (var j = i + 1; j < instances.Count; j++)
                {
                    //Compare instances - are they touching?
                    if (instances[j].InsertionPoint.DistanceTo(instances[i].InsertionPoint) > distanceThreshold ||
                        !Functions.CollisionCheck.CheckCollision(instances[i], instances[j]))
                    {
                        continue;
                    }

                    //If touching, create an 'edge' (relationship).
                    var block_i = instanceObjectBlocks[instances[i]];
                    var block_j = instanceObjectBlocks[instances[j]];

                    var relationship = new Relationship(block_i, block_j);
                    assembly.AddRelationship(relationship);
                }
            }

            return assembly;
        }
    }
}
