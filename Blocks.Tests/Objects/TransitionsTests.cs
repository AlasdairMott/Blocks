using Blocks.Objects;
using System.Linq;
using Xunit;

namespace Blocks.Tests.Objects
{
    [Collection("RhinoTestingCollection")]
    public class TransitionsTests
    {
        [Fact]
        public void FindFromBlockDefinition_ReturnsMatches()
        {
            //Arrange
            var definition = Stubs.TestingStubs.Definitions["A"];
            var transitions = new Transitions
            {
                new Transition(Stubs.TestingStubs.Relationships["A1-A2"]),
                new Transition(Stubs.TestingStubs.Relationships["A1-B1"]),
                new Transition(Stubs.TestingStubs.Relationships["A2-B1"]),
            };

            //Act
            var results = transitions.FindFromBlockDefinition(definition);

            //Assert
            Assert.Equal(4, results.Count());
        }

        [Fact]
        public void FindFromBlockDefinition_ReturnsEmpty()
        {
            //Arrange
            var definition = Stubs.TestingStubs.Definitions["C"];
            var transitions = new Transitions
            {
                new Transition(Stubs.TestingStubs.Relationships["A1-A2"]),
            };

            //Act
            var results = transitions.FindFromBlockDefinition(definition);

            //Assert
            Assert.Empty(results);
        }

        [Fact]
        public void FindFromBlockDefinition_ReturnsInverse()
        {
            //Arrange
            var definition = Stubs.TestingStubs.Definitions["A"];
            var existingTransition = new Transition(Stubs.TestingStubs.Relationships["B1-A2"]);
            var transitions = new Transitions { existingTransition };

            //Act
            var results = transitions.FindFromBlockDefinition(definition);
            var transition = results.Single();

            //Assert
            var tolerance = 0.01;
            Assert.Single(results);
            Assert.True(RelationshipComparer.CompareTransform(transition.Transform, existingTransition.Inverse, tolerance));

        }

        [Fact (Skip = "Not Implemented")]
        public void NormalizeRelationships_IsSuccessful()
        {

        }
    }

}
