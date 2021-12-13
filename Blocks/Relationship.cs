using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
	public class Relationship
	{
		public BlockDefinition Definition { get; set; }
		public Transform Transform { get; set; }
		public double Strength { get; set; }

		public Relationship(BlockDefinition instanceDefinition, Transform transform)
		{
			Definition = instanceDefinition;
			Transform = transform;
		}
	}

	//public interface IInstanceDefinition
	//{
	//	Guid Id { get; }
	//}

	public class BlockDefinition
    {
		private readonly List<GeometryBase> _geometry = new List<GeometryBase>();
		public IReadOnlyCollection<GeometryBase> Geometry => _geometry;
		public int Index { get; private set; }

        public BlockDefinition(IEnumerable<GeometryBase> geometry, int index)
        {
            _geometry.AddRange(geometry);
            Index = index;
        }

		public BlockDefinition(InstanceDefinition instanceDefinition)
        {
			_geometry.AddRange(instanceDefinition.GetObjects().Select(o => o.Geometry));
			Index = instanceDefinition.Index;
        }
    }

	//public class BlockDefinitionGeometry : InstanceDefinitionGeometry, IInstanceDefinition
	//{
	//	public BlockDefinitionGeometry(): base() { }
	//}

	//public class BlockDefinitionGeometry : IInstanceDefinition
	//{
	//	private readonly InstanceDefinitionGeometry _instanceDefinitionGeometry;

	//	public BlockDefinitionGeometry(InstanceDefinitionGeometry instanceDefinitionGeometry)
	//	{
	//		_instanceDefinitionGeometry = instanceDefinitionGeometry;
	//	}

	//	public Guid Id => _instanceDefinitionGeometry.Id;
	//	public override int GetHashCode() => _instanceDefinitionGeometry.GetHashCode();
	//}


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
