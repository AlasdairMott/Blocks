using Blocks.Generators;
using Blocks.Objects;
using Rhino.Geometry;
using System;
using System.Linq;
using Xunit;

namespace Blocks.Tests
{
    [Collection("RhinoTestingCollection")]
    public class GenerateBlockAssemblyTests
    {
        [Fact(Skip = "Not Implemented")]
        public void TryPlace_ItPlaces()
        {
            //Arrange
            var transitions = Stubs.Relationships.Values.Select(r => new Transition(r)).ToTransitions();
            transitions.NormalizeRelationships();
            var assembly = new BlockAssembly();
            var obstacles = new Mesh();
            var sut = new GenerateFromTransitions(0);

            //Act
            //var choice = sut.ChooseBlock(assembly, transitions);

            //Assert
        }
    }
}
