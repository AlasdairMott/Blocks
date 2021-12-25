using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// The parent definition for an instance; defines the children BlockInstances. Its geometry is in local coordinates.
    /// </summary>
	public class BlockDefinition
	{
		private readonly List<GeometryBase> _geometry = new List<GeometryBase>();

		public IReadOnlyCollection<GeometryBase> Geometry => _geometry;
		public int Index { get; private set; }
		public Mesh CollisionMesh { get; private set; } = new Mesh();

		public BlockDefinition(IEnumerable<GeometryBase> geometry, int index)
		{
			_geometry.AddRange(geometry);
			Index = index;

			CreateCollisionMesh(-1);
		}

		public BlockDefinition(InstanceDefinition definition) : this(definition.GetObjects().Select(o => o.Geometry), definition.Index)
        {
        }

		private void CreateCollisionMesh(double offset)
		{
            if (!_geometry.Any()) { return; }
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
	}
    public static class DefinitionExtensions
    {
        public static BlockDefinition ToDefinition(this InstanceDefinition instance) => new BlockDefinition(instance);
    }
}
