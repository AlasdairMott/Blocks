using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks
{
    public class BlockAssembly
    {
        private readonly List<BlockInstance> _blockInstances = new List<BlockInstance>();
        public Mesh CollisionMesh { get; set; }
        public IReadOnlyList<BlockInstance> BlockInstances => _blockInstances;
    }
}
