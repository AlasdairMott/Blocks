using Rhino.Geometry;
using System.Collections.Generic;

namespace Blocks
{
    public class BlockInstance
    {
        public BlockDefinition BlockDefinition { get; set; }
        public Transform Transform { get; set;}
        public Mesh CollisionMesh { get; private set; } = new Mesh();

        public BlockInstance(BlockDefinition blockDefinition, Transform transform)
        {
            BlockDefinition = blockDefinition;
            Transform = transform;

            CollisionMesh = BlockDefinition.CollisionMesh.DuplicateMesh();
            CollisionMesh.Transform(Transform);
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
