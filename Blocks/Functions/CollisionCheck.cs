using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocks.Functions
{
    public static class CollisionCheck
    {
		public static bool CheckCollision(BlockAssembly assembly, BlockInstance instance)
		{
			//var intersection = Intersection.MeshPolyline(polygonPath.CollisionMesh, instance.GetPolyline().ToPolylineCurve(), out int[]faceIds );
			var intersection = MeshClash.Search(assembly.CollisionMesh, instance.CollisionMesh, 0.1, 1);

			if (intersection.Length > 0)
				return true;
			else return false;
		}
	}
}
