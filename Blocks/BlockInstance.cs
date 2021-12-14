using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks
{
    public class BlockInstance
    {
        public BlockDefinition BlockDefinition { get; set; }
        public Transform Transform { get; set;}

        public BlockInstance(BlockDefinition blockDefinition, Transform transform)
        {
            BlockDefinition = blockDefinition;
            Transform = transform;
        }

        public IEnumerable<GeometryBase> GetGeometry()
        {
            var geometries = new List<GeometryBase>();
            foreach (var g in BlockDefinition.Geometry)
            {
                var dup = g.Duplicate();
                dup.Transform(Transform);
                geometries.Add(dup);
            }
            return geometries;
        }
    }
}
