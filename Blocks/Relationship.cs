using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Blocks
{
	public class Relationship
	{
		public InstanceDefinition Definition { get; set; }
		public Transform Transform { get; set; }
		public double Strength { get; set; }

		public Relationship(InstanceDefinition instanceDefinition, Transform transform)
		{
			Definition = instanceDefinition;
			Transform = transform;
		}
	}

	public class RelationshipComparer : IEqualityComparer<Relationship>
	{
		public bool Equals(Relationship x, Relationship y)
		{
			if (GetHashCode(x) != GetHashCode(y)) { return false; }
			if (x.Definition != y.Definition) { return false; }

			//xform1.DecomposeSimilarity

			var difference = x.Transform.CompareTo(y.Transform);
			if (difference < 1) return false;
			return true;
		}

		public int GetHashCode(Relationship obj)
		{
			unchecked
			{
				var hash = 1;
				hash *= (int)Math.Round(obj.Transform.Determinant * 13);
				hash *= obj.Definition.Index;
				return hash;
			}
		}
	}
}
