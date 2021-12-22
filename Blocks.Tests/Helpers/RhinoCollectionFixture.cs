using Xunit;

namespace Blocks.Tests.Helpers
{
    /// <summary>
    /// Collection Fixture - shared context across test classes
    /// </summary>
    /// <remarks>See https://github.com/tmakin/RhinoCommonUnitTesting for details</remarks>
    [CollectionDefinition("RhinoTestingCollection")]
    public class RhinoCollection : ICollectionFixture<RhinoTestingHelper>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
