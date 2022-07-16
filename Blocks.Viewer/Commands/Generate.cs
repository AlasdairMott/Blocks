using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.Geometry;

namespace Blocks.Viewer.Commands
{
    public static class Generate
    {
        public static void Run(int seed, int steps, bool useGroundPlane)
        {
            if (MainForm.Reference == null) return;

            var generator = new FromTransitionsGenerator(seed);
            var transitions = new Transitions(MainForm.Reference.BlockAssembly);

            Mesh groundPlane = useGroundPlane ? 
                Mesh.CreateFromPlane(new Plane(new Point3d(0,0,-10), Vector3d.ZAxis), new Interval(-1000, 1000), new Interval(-1000, 1000), 4, 4) : new Mesh();

            var outputAssembly = generator.Generate(transitions, groundPlane, steps);

            MainForm.SetGenerated(outputAssembly);
        }
    }
}
