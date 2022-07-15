using Blocks.Common.Objects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

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
        /// <param name="area">Area to consider for a collision. If area is 0, the instances only need to touch on an edge or point</param>
        /// <returns>True if the BlockInstances are touching, otherwise false.</returns>
        public static bool CheckTouching(BlockInstance instance, BlockInstance other, double area)
        {
            var meshA = GeometryHelpers.GetBlockInstanceMesh(instance);
            var meshB = GeometryHelpers.GetBlockInstanceMesh(other);

            if (area == 0.0) { return CheckCollision(meshB, meshA); }

            var clashes = MeshClash.Search(meshA.ExplodeAtUnweldedEdges(), meshB.ExplodeAtUnweldedEdges(), 0.1, 5);

            var polyline = new Polyline();

            foreach (var clash in clashes)
            {
                polyline.Add(clash.ClashPoint);
                polyline.MergeColinearSegments(Rhino.RhinoMath.ToRadians(1), false);
                polyline.RemoveNearlyEqualSubsequentPoints(0.1);

                if (polyline.Count >= 3 && polyline.IsValid) {

                    var closed_polyline = new Polyline(polyline);
                    closed_polyline.Add(closed_polyline[0]);
                    var computeArea = AreaMassProperties.Compute(closed_polyline.ToNurbsCurve());

                    if (computeArea != null && computeArea.Area > area)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
	}
}
