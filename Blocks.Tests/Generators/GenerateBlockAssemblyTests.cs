using Blocks.Common.Generators;
using Blocks.Common.Objects;
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
            /*
            //Arrange
            var transitions = TestingStubs.transitions.Values.Select(r => new Transition(r)).ToTransitions();
            transitions.ComputeProbabilities();
            var assembly = new BlockAssembly();
            var obstacles = new Mesh();
            var generator = new FromTransitionsGenerator(0);

            //Act

            //Assert
            */
        }
    }
}
