using Rhino.Geometry;
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
	}
}
