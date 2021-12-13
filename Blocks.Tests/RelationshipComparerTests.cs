using System;
using Xunit;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Collections;

namespace Blocks.Tests
{
	[Collection("RhinoTestingCollection")]
	public class RelationshipComparerTests
	{
		[Fact]
		public void Test1()
		{
			//Arrange
			var point1 = new Point3d(0, 0, 0);
			var point2 = new Point3d(0, 0, 600);

			//Act
			var distance = point1.DistanceTo(point2);

			//Assert
			Assert.InRange(distance, 0, 1000);
		}

		[Fact]
		public void Test2()
		{
			//Arrange
			var point1 = new Point3d(0, 0, 0);
			var xform = Transform.Translation(Vector3d.ZAxis * 10);

			//Act

			point1.Transform(xform);
			var distance = point1.DistanceTo(Point3d.Origin);

			//Assert
			Assert.InRange(distance, 0, 1000);
		}

		[Theory]
		[ClassData(typeof(TestInstanceDefinitionData))]
		public void RelationshipEquality_IsEqual(Relationship relationshipX, Relationship relationshipY)
		{
			//Arrange
			var sut = new RelationshipComparer();

			//Act
			var equality = sut.Equals(relationshipX, relationshipY);

			//Assert
			Assert.True(equality);
		}
	}

	public class TestInstanceDefinitionData : IEnumerable<object[]>
	{
		private readonly Dictionary<string, List<GeometryBase>> _geometries;
		private readonly Dictionary<string, BlockDefinition> _definitions;
		private readonly Dictionary<string, Transform> _transforms;
		private readonly Dictionary<string, Relationship> _relations;

		public TestInstanceDefinitionData()
        {
			_geometries = new Dictionary<string, List<GeometryBase>>
			{
				{ "A", new List<GeometryBase>() }
			};
			_definitions = new Dictionary<string, BlockDefinition>
			{
				{ "A", new BlockDefinition(_geometries["A"], 0) }
			};
			_transforms = new Dictionary<string, Transform>
			{
				{"Identity", Transform.Identity},
				{"ChangePlane1", Transform.PlaneToPlane(Plane.WorldXY, Plane.WorldZX) }

			};
			_relations = new Dictionary<string, Relationship>
			{
				{ "A", new Relationship(_definitions["A"], _transforms["Identity"]) },
				{ "B", new Relationship(_definitions["A"], _transforms["ChangePlane1"]) }
			};
		}
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[] { _relations["A"], _relations["A"] };
			yield return new object[] { _relations["B"], _relations["B"] };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

	//public class TestInstanceDefinition : IInstanceDefinition
	//   {
	//	public int Index { get; set; }

	//       public TestInstanceDefinition(int index)
	//       {
	//           Index = index;
	//       }
	//   }	
}
