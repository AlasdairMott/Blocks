using Blocks.Common.Objects;
using Blocks.Common.Parameters;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Readers
{
    /// <summary>
    /// Read a BlockAssembly from a Rhino file.
    /// </summary>
    public class BlockAssemblyReader
    {
        public BlockAssemblyReader()
        {
        }

        public BlockAssembly Read(List<BlockInstance> instances, BlockAssemblyReaderParameters parameters)
        {
            if (instances == null) { throw new ArgumentNullException(nameof(instances)); }
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }

            //Create an empty assembly
            var assembly = new BlockAssembly();
            assembly.AddInstances(instances);

            //Create connectivitiy graph
            var edgeReader = new BlockAssemblyEdgeReader();
            var edges = edgeReader.Read(assembly, parameters.EdgeReaderParameters);
            assembly.AddEdges(edges);

            return assembly;
        }

        public BlockAssembly Read(List<InstanceObject> instanceObjects, BlockAssemblyReaderParameters parameters)
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

            return Read(instanceObjectBlocks.Values.ToList(), parameters);
        }
    }
}
