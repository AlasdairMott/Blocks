using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Blocks.Objects
{
    public class RelationshipComparer : IEqualityComparer<Relationship>
	{
		public bool Equals(Relationship x, Relationship y)
		{
			if (GetHashCode(x) != GetHashCode(y)) { return false; }
			if (x.Definition.Index != y.Definition.Index) { return false; }

			return CompareTransform(x.Transform, y.Transform, 0.1);
		}

		public int GetHashCode(Relationship relationship)
		{
			unchecked
			{
				var hash = 1;
				hash *= (int)Math.Round(TransformMass(relationship.Transform) * 13);
				hash *= relationship.Definition.Index;
				return hash;
			}
		}

		public static double TransformMass(Transform xform)
		{
			var sum = 0.0;
			for (int num = 3; num >= 0; num--)
			{
				for (int num2 = 3; num2 >= 0; num2--)
				{
					sum += xform[num, num2];
				}
			}

			return sum;
		}
		public static double TransformDifference(Transform x, Transform y)
		{
			var difference = 0.0;
			for (int num = 3; num >= 0; num--)
			{
				for (int num2 = 3; num2 >= 0; num2--)
				{
					difference += Math.Abs(x[num, num2] - y[num, num2]);
				}
			}

			return difference;
		}

		public static bool CompareTransform(Transform x, Transform y, double tolerance) => TransformDifference(x, y) <= tolerance;
	}
}
