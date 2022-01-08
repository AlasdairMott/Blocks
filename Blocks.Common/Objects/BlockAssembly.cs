using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// An arrangement of BlockInstances.
    /// </summary>
    public class BlockAssembly
    {
        private readonly List<BlockInstance> _blockInstances = new List<BlockInstance>();
        private readonly List<Edge> _edges = new List<Edge>();

        public Mesh CollisionMesh { get; set; } = new Mesh();
        public IReadOnlyList<BlockInstance> BlockInstances => _blockInstances;
        public IReadOnlyList<Edge> Edges => _edges;
        public int Size => _blockInstances.Count();

        /// <summary>
        /// Add new block instances to this BlockAssembly.
        /// </summary>
        /// <param name="instances">The BlockInstances to add.</param>
        public void AddInstances(List<BlockInstance> instances)
        {
            foreach (var instance in instances) { AddInstance(instance); }
        }

        /// <summary>
        /// Add a new block instance to this BlockAssembly.
        /// </summary>
        /// <param name="instance">The BlockInstance to add.</param>
        public void AddInstance(BlockInstance instance) {
            _blockInstances.Add(instance);
            CollisionMesh.Append(instance.CollisionMesh);
        }

        internal void AddEdges(IEnumerable<Edge> edges)
        {
            if (!edges.Any()) { return; }
            if (!edges.Any(e => _blockInstances.Contains(e.FromInstance) && _blockInstances.Contains(e.ToInstance))){
                throw new ArgumentException("Block instance in edge was not in the BlockAssembly's instances", nameof(edges));
            }
            _edges.AddRange(edges);
        }

        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        public IEnumerable<Relationship> FindFromBlockDefinition(BlockDefinition definition)
        {
            var matching = _edges.Where(t =>
                t.From.Name == definition.Name ||
                t.To.Name == definition.Name);

            return matching.Select(m =>
                m.From.Name == definition.Name ? m : m.Invert());
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
