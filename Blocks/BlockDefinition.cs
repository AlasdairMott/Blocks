using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
    public class BlockDefinition
    {
        private readonly List<GeometryBase> _geometry = new List<GeometryBase>();
        public IReadOnlyCollection<GeometryBase> Geometry => _geometry;
        public int Index { get; private set; }

        public BlockDefinition(IEnumerable<GeometryBase> geometry, int index)
        {
            _geometry.AddRange(geometry);
            Index = index;
        }

        public BlockDefinition(InstanceDefinition instanceDefinition)
        {
            _geometry.AddRange(instanceDefinition.GetObjects().Select(o => o.Geometry));
            Index = instanceDefinition.Index;
        }
    }

    public static class InstanceExtensions
    {
        public static BlockDefinition ToDefinition(this InstanceDefinition instance) => new BlockDefinition(instance);
    }
}
