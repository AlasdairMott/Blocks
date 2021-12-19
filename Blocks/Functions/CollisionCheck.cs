using Blocks.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System.Linq;

namespace Blocks.Functions
{
    public static class CollisionCheck
    {
		public static bool CheckCollision(Mesh meshA, Mesh meshB)
        {
			var intersection = MeshClash.Search(meshA, meshB, 0.1, 1);

			if (intersection.Length > 0)
				return true;
			else return false;
		}
		public static bool CheckCollision(BlockAssembly assembly, BlockInstance instance)
		{
			return CheckCollision(assembly.CollisionMesh, instance.CollisionMesh);
		}
        public static bool CheckCollision(InstanceObject instance, InstanceObject other)
        {
			var view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

			var meshA = GetCollisionMesh(instance);
			var meshB = GetCollisionMesh(other);

			return CheckCollision(meshA, meshB);
		}

		public static Mesh GetCollisionMesh(InstanceObject instance)
        {
			var geometry = instance.InstanceDefinition.GetObjects().Select(o => o.Geometry);
			var mesh = new Mesh();
			foreach (var geo in geometry)
            {
				var geometryMesh = GetGeometryMesh(geo);
				if (geometryMesh != null) mesh.Append(geometryMesh);
			}
			mesh.Transform(instance.InstanceXform);
			return mesh;
		}

		/// <summary>
		/// Extract the mesh from any piece of geometry
		/// </summary>
		/// <param name="geometry">Geometry to extract the mesh from</param>
		/// <returns></returns>
		/// <remarks>from https://discourse.mcneel.com/t/get-mesh-for-any-geometry-object-in-3dm-file/133006/3 </remarks>
		public static Mesh GetGeometryMesh(GeometryBase geometry)
		{
			Mesh rc = null;

			if (geometry is Mesh mesh)
			{
				rc = mesh;
			}
			else if (geometry is Extrusion extrusion)
			{
				var extrusionMesh = extrusion.GetMesh(MeshType.Any);
				if (null != extrusionMesh)
					rc = extrusionMesh;
			}
			else if (geometry is Brep brep)
			{
				var brepMesh = new Mesh();
				foreach (var brepFace in brep.Faces)
				{
					var brepFaceMesh = brepFace.GetMesh(MeshType.Any);
					if (null != brepFaceMesh)
						brepMesh.Append(brepFaceMesh);
				}
				if (brepMesh.IsValid)
					rc = brepMesh;
			}

			if (null != rc)
			{
				rc.Normals.ComputeNormals();
				rc.Compact();
				if (!rc.IsValid)
					rc = null;
			}

			return rc;
		}
	}
}
