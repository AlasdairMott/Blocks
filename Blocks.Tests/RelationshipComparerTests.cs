using Blocks.Objects;
using System.Collections;
using System.Collections.Generic;
using Xunit;

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
		
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[] { Stubs.Relationships["A1-A1"], Stubs.Relationships["A1-A1"], true };
			yield return new object[] { Stubs.Relationships["A1-A2"], Stubs.Relationships["A2-A1"], true };
			yield return new object[] { Stubs.Relationships["A2-B1"], Stubs.Relationships["A2-B1"], true };
			yield return new object[] { Stubs.Relationships["A2-A1"], Stubs.Relationships["A1-A1"], false };
			yield return new object[] { Stubs.Relationships["A2-A2"], Stubs.Relationships["A2-B1"], false };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
