using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// Compare whether two transitions are the same.
    /// </summary>
    public class TransitionComparer : IEqualityComparer<Transition>
	{
		public bool Equals(Transition x, Transition y)
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

		public int GetHashCode(Transition transition)
		{
			unchecked
			{
				var hash = 1;
				hash += 13 * (int)Math.Round(TransformMass(transition.Transform) + TransformMass(transition.Inverse));
				hash += 17 * (transition.From.Name.GetHashCode() + transition.To.Name.GetHashCode());
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
