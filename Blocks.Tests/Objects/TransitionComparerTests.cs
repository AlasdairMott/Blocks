using Blocks.Common.Objects;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Blocks.Tests.Objects
{
    [Collection("RhinoTestingCollection")]
	public class TransitionComparerTests
	{
		[Theory]
		[ClassData(typeof(TestInstanceDefinitionData))]
		public void transitionEquality_IsEqual(Transition transitionX, Transition transitionY, bool expected)
		{
			//Arrange
			var sut = new TransitionComparer();

			//Act
			var equality = sut.Equals(transitionX, transitionY);

			//Assert
			Assert.Equal(expected, equality);
		}
	}

	public class TestInstanceDefinitionData : IEnumerable<object[]>
	{
		
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[] { Stubs.TestingStubs.Transitions["A1-A1"], Stubs.TestingStubs.Transitions["A1-A1"], true };
			yield return new object[] { Stubs.TestingStubs.Transitions["A1-A2"], Stubs.TestingStubs.Transitions["A2-A1"], true };
			yield return new object[] { Stubs.TestingStubs.Transitions["A2-B1"], Stubs.TestingStubs.Transitions["A2-B1"], true };
			yield return new object[] { Stubs.TestingStubs.Transitions["A2-A1"], Stubs.TestingStubs.Transitions["A1-A1"], false };
			yield return new object[] { Stubs.TestingStubs.Transitions["A2-A2"], Stubs.TestingStubs.Transitions["A2-B1"], false };
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
