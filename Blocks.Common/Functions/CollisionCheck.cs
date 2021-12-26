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
        /// Check where two BlockInstances touch.
        /// </summary>
        /// <param name="instance">The first BlockInstance to check.</param>
        /// <param name="other">The second BlockInstance to check.</param>
        /// <returns>True if the BlockInstances are touching, otherwise false.</returns>
        public static bool CheckTouching(BlockInstance instance, BlockInstance other)
        {
            var meshA = GeometryHelpers.GetBlockInstanceMesh(instance);
            var meshB = GeometryHelpers.GetBlockInstanceMesh(other);

            return CheckCollision(meshB, meshA);
        }
	}
}
