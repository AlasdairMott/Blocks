using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks.Common.Objects
{
    public class Edge : Relationship
    {
        public BlockInstance FromInstance { get; private set; }
        public BlockInstance ToInstance { get; private set; }

        public Edge(BlockInstance from, BlockInstance to) : base(from, to) {
            FromInstance = from;
            ToInstance = to;
        }
    }
}
