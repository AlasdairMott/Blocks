using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// A block definition placed in world coordinates. Part of a BlockAssembly.
    /// </summary>
    public class BlockInstance
    {
        private Lazy<IEnumerable<GeometryBase>> _geometry => new Lazy<IEnumerable<GeometryBase>>(() =>
            BlockDefinition.Geometry.Select(b =>
            {
                var dup = b.Duplicate();
                dup.Transform(Transform);
                return dup;
            })
        );
        private Lazy<Point3d> _location => new Lazy<Point3d>(() =>
        {
            var point = Point3d.Origin;
            point.Transform(Transform);
            return point;
        });

        public BlockDefinition BlockDefinition { get; set; }
        public Transform Transform { get; set;}
        public Mesh CollisionMesh { get; private set; } = new Mesh();
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public Point3d Location => _location.Value;
        public IEnumerable<GeometryBase> Geometry => _geometry.Value;

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
    }

    public class BlockInstanceState : BlockInstance 
    {   
        public Transitions Transitions { get; }
        
        public BlockInstanceState(BlockDefinition blockDefinition, Transform transform, Transitions transitions) : base(blockDefinition, transform)
        {
            Transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
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
