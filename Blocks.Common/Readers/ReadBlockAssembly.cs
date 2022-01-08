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
            assembly.AddInstances(instances);

            //Create connectivitiy graph
            var edgeReader = new ReadBlockAssemblyEdges();
            var edges = edgeReader.Read(assembly, distanceThreshold);
            assembly.AddEdges(edges);

            return assembly;
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
