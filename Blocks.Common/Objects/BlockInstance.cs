using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// A block definition placed in world coordinates. Part of a BlockAssembly.
    /// </summary>
    public class BlockInstance
    {
        public BlockDefinition BlockDefinition { get; set; }
        public Transform Transform { get; set;}
        public Mesh CollisionMesh { get; private set; } = new Mesh();
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public Point3d InsertionPoint 
        { 
            get
            {
                var point = Point3d.Origin;
                point.Transform(Transform);
                return point;
            } 
        }

        public BlockInstance(BlockDefinition blockDefinition, Transform transform)
        {
            BlockDefinition = blockDefinition;
            Transform = transform;

            CollisionMesh = BlockDefinition.CollisionMesh.DuplicateMesh();
            CollisionMesh.Transform(Transform);
        }
        public double DistanceTo(BlockInstance other)
        {
            var point1 = Point3d.Origin;
            var point2 = Point3d.Origin;

            point1.Transform(Transform);
            point2.Transform(other.Transform);

            return point1.DistanceTo(point2);
        }
        public IEnumerable<GeometryBase> GetGeometry()
        {
            foreach (var g in BlockDefinition.Geometry)
            {
                var dup = g.Duplicate();
                dup.Transform(Transform);
                yield return dup;
            }
        }
    }

    public static class InstanceExtensions
    {
        public static BlockInstance ToInstance(this InstanceObject instance)
        {
            var blockDefinition = new BlockDefinition(instance.InstanceDefinition);
            return new BlockInstance(blockDefinition, instance.InstanceXform);
        }
    }
}
