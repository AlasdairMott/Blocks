using System;
using Xunit;
using Rhino.Geometry;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Collections;
using Blocks.Objects;

namespace Blocks.Tests
{
	[Collection("RhinoTestingCollection")]
	public class RelationshipComparerTests
	{
		[Theory]
		[ClassData(typeof(TestInstanceDefinitionData))]
		public void RelationshipEquality_IsEqual(Relationship relationshipX, Relationship relationshipY, bool expected)
		{
			//Arrange
			var sut = new RelationshipComparer();

			//Act
			var equality = sut.Equals(relationshipX, relationshipY);

			//Assert
			Assert.Equal(expected, equality);
		}
	}

	public class TestInstanceDefinitionData : IEnumerable<object[]>
	{
		private readonly Dictionary<string, List<GeometryBase>> _geometries;
		private readonly Dictionary<string, BlockDefinition> _definitions;
		private readonly Dictionary<string, Transform> _transforms;
		private readonly Dictionary<string, BlockInstance> _instances;
		private readonly Dictionary<string, Relationship> _relationships;

		public TestInstanceDefinitionData()
        {
			_geometries = new Dictionary<string, List<GeometryBase>>
			{
				{ "A", new List<GeometryBase>() },
				{ "B", new List<GeometryBase>() }
			};
			_definitions = new Dictionary<string, BlockDefinition>
			{
				{ "A", new BlockDefinition(_geometries["A"], 0) },
				{ "B", new BlockDefinition(_geometries["B"], 1) }
			};
			_transforms = new Dictionary<string, Transform>
			{
				{"Identity", Transform.Identity},
				{"ChangePlane1", Transform.PlaneToPlane(Plane.WorldXY, Plane.WorldZX) },
				{"ChangePlane2", Transform.PlaneToPlane(new Plane(new Point3d(10,10,10), new Vector3d(12,4,-5)), Plane.WorldZX) }
			};
			_instances = new Dictionary<string, BlockInstance>
			{
				{"A", new BlockInstance(_definitions["A"], _transforms["Identity"])},
				{"B", new BlockInstance(_definitions["A"], _transforms["ChangePlane1"])},
				{"C", new BlockInstance(_definitions["B"], _transforms["ChangePlane2"])},
			};
			_relationships = new Dictionary<string, Relationship>
			{
				{ "A-A", new Relationship(_instances["A"], _instances["A"]) },
				{ "A-B", new Relationship(_instances["A"], _instances["B"]) },
				{ "B-A", new Relationship(_instances["B"], _instances["A"]) },
				{ "B-B", new Relationship(_instances["B"], _instances["B"]) },
				{ "B-C", new Relationship(_instances["B"], _instances["C"]) }
			};
		}
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[] { _relationships["A-A"], _relationships["A-A"], true };
			yield return new object[] { _relationships["A-B"], _relationships["B-A"], true };
			yield return new object[] { _relationships["B-C"], _relationships["B-C"], true };
			yield return new object[] { _relationships["B-A"], _relationships["A-A"], false };
			yield return new object[] { _relationships["B-B"], _relationships["B-C"], false };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
