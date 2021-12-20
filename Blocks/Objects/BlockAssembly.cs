using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Objects
{
    /// <summary>
    /// An arrangement of BlockInstances.
    /// </summary>
    public class BlockAssembly
    {
        private readonly List<BlockInstance> _blockInstances = new List<BlockInstance>();
        private readonly List<Relationship> _relationships = new List<Relationship>();

        public Mesh CollisionMesh { get; set; } = new Mesh();
        public IReadOnlyList<BlockInstance> BlockInstances => _blockInstances;
        public IReadOnlyCollection<Relationship> Relationships => _relationships;
        public int Size => _blockInstances.Count();

        /// <summary>
        /// Add a new block instance to this BlockAssembly.
        /// </summary>
        /// <param name="instance">The BlockInstance to add.</param>
        public void AddInstance(BlockInstance instance) {
            _blockInstances.Add(instance);
            CollisionMesh.Append(instance.CollisionMesh);
        }
        public void AddRelationship(Relationship relationship)
        {
            _relationships.Add(relationship);
        }

        public IEnumerable<Relationship> FindFromBlockDefinition(BlockDefinition definition)
        {
            var matching = _relationships.Where(t =>
                t.From.BlockDefinition.Index == definition.Index ||
                t.To.BlockDefinition.Index == definition.Index);

            return matching.Select(m =>
                m.From.BlockDefinition.Index == definition.Index ? m : m.Invert());
        }

        /// <summary>
        /// Return all geometry in this BlockAssembly, made from its instances.
        /// </summary>
        /// <returns>All geometry in this BlockAssembly.</returns>
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
