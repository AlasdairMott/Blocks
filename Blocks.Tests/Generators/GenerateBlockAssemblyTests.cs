using Blocks.Generators;
using Blocks.Objects;
using Blocks.Tests.Helpers;
using Blocks.Tests.Stubs;
using Rhino.Geometry;
using System.Linq;
using Xunit;

namespace Blocks.Tests.Generators
{
    [Collection("RhinoTestingCollection")]
    public class GenerateBlockAssemblyTests
    {
        private readonly Rhino3dmHelper _rhinoHelper = new Rhino3dmHelper("TestHelper.3dm");

        [Fact(Skip = "Not Implemented")]
        public void TryPlace_ItPlaces()
        {
            //Arrange
            var transitions = TestingStubs.Relationships.Values.Select(r => new Transition(r)).ToTransitions();
            transitions.NormalizeRelationships();
            var assembly = new BlockAssembly();
            var obstacles = new Mesh();
            var generator = new GenerateFromTransitions(0);

            //Act

            //Assert
        }
    }
}
