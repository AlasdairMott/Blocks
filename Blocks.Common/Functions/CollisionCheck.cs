using Blocks.Common.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System.Linq;

namespace Blocks.Common.Functions
{
    public static class CollisionCheck
    {
        /// <summary>
        /// Check whether two meshes collide.
        /// </summary>
        /// <param name="meshA">The first mesh to collide.</param>
        /// <param name="meshB">The second mesh to collide.</param>
        /// <returns>True if the meshes collide, otherwise false.</returns>
		public static bool CheckCollision(Mesh meshA, Mesh meshB)
        {
			var intersection = MeshClash.Search(meshA, meshB, 0.1, 1);

			if (intersection.Length > 0)
				return true;
			else return false;
		}

        /// <summary>
        /// Check whether a BlockInstance collides with a BlockAssembly.
        /// </summary>
        /// <param name="assembly">The BlockAssembly to collide with.</param>
        /// <param name="instance">The BlockInstance to collide with.</param>
        /// <returns>True if there is a collision, otherwise false.</returns>
		public static bool CheckCollision(BlockAssembly assembly, BlockInstance instance)
		{
			return CheckCollision(assembly.CollisionMesh, instance.CollisionMesh);
		}

        /// <summary>
        /// Check whether two InstanceObjects collide.
        /// </summary>
        /// <param name="instance">The first InstanceObject to collide.</param>
        /// <param name="other">The second InstanceObject to collide.</param>
        /// <returns>True if the InstanceObjects collide, otherwise false.</returns>
        public static bool CheckCollision(InstanceObject instance, InstanceObject other)
        {
			var view = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

			var meshA = GeometryHelpers.GetInstanceObjectMesh(instance);
			var meshB = GeometryHelpers.GetInstanceObjectMesh(other);

			return CheckCollision(meshA, meshB);
		}
	}
}
