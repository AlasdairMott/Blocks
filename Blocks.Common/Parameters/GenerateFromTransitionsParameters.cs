using Blocks.Common.Objects;
using Rhino.Geometry;

namespace Blocks.Common.Parameters
{
    public class GenerateFromTransitionsParameters
    {
        public Transitions Transitions { get; set; }
        public Mesh Obstacles { get; set; }
        public int Seed { get; set; }
        public int Steps { get; set; }
    }
}
