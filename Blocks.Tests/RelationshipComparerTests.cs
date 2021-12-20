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
			_instances = new Dictionary<string, BlockInstance>
			{
				{"A", new BlockInstance(_definitions["A"], _transforms["Identity"])},
				{"B", new BlockInstance(_definitions["A"], _transforms["ChangePlane1"])},
			};
			_relationships = new Dictionary<string, Relationship>
			{
				{ "A-A", new Relationship(_instances["A"], _instances["A"]) },
				{ "A-B", new Relationship(_instances["A"], _instances["B"]) },
				{ "B-A", new Relationship(_instances["A"], _instances["B"]) },
				{ "B-B", new Relationship(_instances["A"], _instances["B"]) }
			};
		}
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[] { _relationships["A-A"], _relationships["A-A"], true };
			yield return new object[] { _relationships["A-B"], _relationships["B-A"], true };
			yield return new object[] { _relationships["B-A"], _relationships["A-A"], false };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
