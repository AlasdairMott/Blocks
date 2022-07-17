using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Xunit;

namespace Blocks.Tests.Objects
{
    [Collection("RhinoTestingCollection")]
    public class StateTests
    {
        [Fact]
        public void SameBlockInstanceAndLocation_ItHasSameHashcode()
        {
            //Arrange
            var definition = Stubs.TestingStubs.Definitions["A"];
            var xform = Stubs.TestingStubs.Transforms["ChangePlane1"];
            var transitions = new Transitions();
            var instance1 = new State(definition, xform, transitions, 0);
            var instance2 = new State(definition, xform, transitions, 0);

            //Act
            int hashCode1 = instance1.GetHashCode();
            int hashCode2 = instance2.GetHashCode();

            //Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void SameBlockInstanceDifferentLocation_HashCodesDiffer()
        {
            //Arrange
            var definition = Stubs.TestingStubs.Definitions["A"];
            var xform1 = Stubs.TestingStubs.Transforms["ChangePlane1"];
            var xform2 = Stubs.TestingStubs.Transforms["ChangePlane2"];
            var transitions = new Transitions();
            var instance1 = new State(definition, xform1, transitions, 0);
            var instance2 = new State(definition, xform2, transitions, 0);

            //Act
            int hashCode1 = instance1.GetHashCode();
            int hashCode2 = instance2.GetHashCode();

            //Assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}
