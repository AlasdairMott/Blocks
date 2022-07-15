using Blocks.Common.Parameters;

namespace Blocks.Viewer.Data
{
    public class Preferences
    {
        public static BlockAssemblyReaderParameters BlockAssemblyReaderParameters { get; set; }
        public static FromTransitionsGeneratorParameters FromTransitionsGeneratorParameters { get; set; }
        public static GraphGeneratorParameters GraphGeneratorParameters { get; set; }

        public static void CreateDefaults()
        {
            BlockAssemblyReaderParameters = new BlockAssemblyReaderParameters
            {
                EdgeReaderParameters = new BlockAssemblyEdgeReaderParameters(collisionArea:2.0, distanceThreshold:50.0),
            };
        }
    }
}
