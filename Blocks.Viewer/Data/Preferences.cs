using Blocks.Common.Parameters;

namespace Blocks.Viewer.Data
{
    public class Preferences
    {
        public static ReadAssemblyParameters ReadAssemblyParameters { get; set; }
        public static GenerateFromTransitionsParameters GenerateFromTransitionsParameters { get; set; }
        public static GraphGeneratorParameters GraphGeneratorParameters { get; set; }
    }
}
