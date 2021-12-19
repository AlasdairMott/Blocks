using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Objects
{
    /// <summary>
    /// The parent definition for an instance; defines the children BlockInstances. Its geometry is in local coordinates.
    /// </summary>
	public class BlockDefinition
	{
		private readonly List<GeometryBase> _geometry = new List<GeometryBase>();
		private RelationshipComparer _comparer = new RelationshipComparer();
		private HashSet<Relationship> _relationships;

		public IReadOnlyCollection<GeometryBase> Geometry => _geometry;
		public int Index { get; private set; }
		public Mesh CollisionMesh { get; private set; } = new Mesh();

		public IEnumerable<Relationship> Relationships => _relationships;

		public BlockDefinition(IEnumerable<GeometryBase> geometry, int index)
		{
			_geometry.AddRange(geometry);
			Index = index;
			_relationships =  new HashSet<Relationship>(_comparer);

			CreateCollisionMesh(-1);
		}

		public BlockDefinition(InstanceDefinition definition) : this(definition.GetObjects().Select(o => o.Geometry), definition.Index)
        {
        }

		private void CreateCollisionMesh(double offset)
		{
			var extrusions = _geometry.Where(g => g is Extrusion).Select(e => (e as Extrusion).ToBrep());
			var breps = _geometry.Where(g => g is Brep).Cast<Brep>();
			
			var tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

			foreach (var brep in breps.Concat(extrusions))
			{
				var offsets = Brep.CreateOffsetBrep(brep, offset, false, true, tolerance, out Brep[] blends, out Brep[] walls);
				var meshes = offsets.SelectMany(o => Mesh.CreateFromBrep(o, MeshingParameters.FastRenderMesh));
				CollisionMesh.Append(meshes);
			}
		}

		public void AddRelationships(IEnumerable<Relationship> relationships)
		{
			_relationships.UnionWith(relationships);
		}

		public void AddRelationship(Relationship relationship)
		{
			_relationships.Add(relationship);
		}

		public Relationship FindRelationship(Relationship relationship)
        {
			return _relationships.FirstOrDefault(r => _comparer.Equals(r, relationship));
		}

        /// <summary>
        /// Choose a Relationship at random from the collection of relationships.
        /// </summary>
        /// <param name="random">Random number generator to use.</param>
        /// <returns>A Relationship.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if there are no relationships in this BlockDefinition.</exception>
        public Relationship Next(Random random)
		{
			if (!_relationships.Any()) { throw new IndexOutOfRangeException("No relationships to choose from"); }
			var value = random.NextDouble();
			var shuffled = _relationships.OrderBy(r => random.NextDouble());
			var result = shuffled.FirstOrDefault(r => r.Strength > value);
			if (result != null) { return result; }
			return shuffled.First();
		}

        /// <summary>
        /// Normalize the strengths of the relationships in this definition so that they sum to 1.
        /// </summary>
        /// <remarks>Call this after all relationships have been added.</remarks>
		public void NormalizeRelationships()
		{
			var mass = _relationships.Sum(r => r.Strength);
			foreach (var relationship in _relationships)
			{
				relationship.Strength /= mass;
			}
		}
	}

	public static class InstanceExtensions
	{
		public static BlockDefinition ToDefinition(this InstanceDefinition instance) => new BlockDefinition(instance);
	}
}
