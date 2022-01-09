using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.Geometry;

namespace Blocks.Viewer.Commands
{
    public static class Generate
    {
        public static void Run(int seed)
        {
            if (MainForm.Reference == null) return;

            var generator = new GenerateFromTransitions(seed);
            var transitions = new Transitions(MainForm.Reference.BlockAssembly);
            //var groundPlane = Mesh.CreateFromPlane(Plane.WorldXY, new Interval(-20, 20), new Interval(-20, 20), 4, 4);
            var groundPlane = new Mesh();
            var outputAssembly = generator.Generate(transitions, groundPlane, seed);

            MainForm.SetGenerated(outputAssembly);
        }
    }
}
