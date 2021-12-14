using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
    public class BlockAssembly
    {
        private readonly List<BlockInstance> _blockInstances = new List<BlockInstance>();

        public Mesh CollisionMesh { get; set; } = new Mesh();
        public IReadOnlyList<BlockInstance> BlockInstances => _blockInstances;
        public int Size => _blockInstances.Count();

        public void AddInstance(BlockInstance instance) {
            _blockInstances.Add(instance);
            CollisionMesh.Append(instance.CollisionMesh);
        }
        
        public IEnumerable<GeometryBase> GetGeometry()
        {
            var geometries = new List<GeometryBase>();
            foreach (var instance in _blockInstances)
            {
                geometries.AddRange(instance.GetGeometry());
            }
            return geometries;
        }
    }
}
