using System;
using System.Collections;
using System.Collections.Generic;

namespace Blocks.Objects
{
    /// <summary>
    /// A collection of BlockDefinitions that can be chosen from when generating a BlockAssembly.
    /// </summary>
    public class BlockPool : IEnumerable<BlockDefinition>
    {
        private readonly List<BlockDefinition> _blocks = new List<BlockDefinition>();

        public BlockPool()
        {
        }

        public BlockPool(IEnumerable<BlockDefinition> blocks)
        {
            _blocks.AddRange(blocks);
        }

        public BlockDefinition this[int index]
        {
            get { return _blocks[index]; }
            set { _blocks.Insert(index, value); }
        }

        public IEnumerator<BlockDefinition> GetEnumerator() => _blocks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public BlockDefinition GetRandom(Random random) => throw new NotImplementedException();
    }
}
