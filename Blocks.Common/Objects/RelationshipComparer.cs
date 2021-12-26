using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// Compare whether two relationships are the same.
    /// </summary>
    public class RelationshipComparer : IEqualityComparer<Relationship>
	{
		public bool Equals(Relationship x, Relationship y)
		{
			if (GetHashCode(x) != GetHashCode(y)) { return false; }

            var blocksEqual = (x.From.Name == y.From.Name &&
                               x.To.Name == y.To.Name) ||
                              (x.From.Name == y.To.Name &&
                               x.To.Name == y.From.Name);

            if (!blocksEqual) { return false; }
			
			return CompareTransform(x.Transform, y.Transform, 0.1) ||
                   CompareTransform(x.Transform, y.Inverse, 0.1);
		}

		public int GetHashCode(Relationship relationship)
		{
			unchecked
			{
				var hash = 1;
				hash += 13 * (int)Math.Round(TransformMass(relationship.Transform) + TransformMass(relationship.Inverse));
				hash += 17 * (relationship.From.Name.GetHashCode() + relationship.To.Name.GetHashCode());
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
