using Blocks.Common.Generators;
using Blocks.Common.Objects;
using Rhino.Geometry;
using System;

namespace Blocks.Viewer.Commands
{
    public static class Generate
    {
        public static void Run(int seed, int steps, bool useGroundPlane)
        {
            if (MainForm.Reference == null) return;

            Mesh groundPlane = useGroundPlane ? 
                Mesh.CreateFromPlane(new Plane(new Point3d(0,0,-10), Vector3d.ZAxis), new Interval(-1000, 1000), new Interval(-1000, 1000), 4, 4) : new Mesh();

            var transitions = new Transitions(MainForm.Reference.BlockAssembly);

            string name = "WFC";
            IBlockAssemblyGenerator generator;
            switch (name)
            {
                case "WFC":
                    generator = new EntangledCollisionsGenerator(transitions, groundPlane, seed, steps);
                    break;
                case "Recursive":
                    generator = new RecursiveGenerator(transitions, groundPlane, seed, steps);
                    break;
                default:
                    throw new ArgumentException($"Unknown generator: {name}");
            }
            
            var result = generator.Generate();

            //var generator = new FromTransitionsGenerator(seed);
            //var result = generator.Generate(transitions, groundPlane, steps);

            MainForm.SetGenerated(result);
        }
    }
}
