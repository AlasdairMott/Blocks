using Blocks.Common.Generators;
using Blocks.Common.Generators.ConstraintSolvers;
using Blocks.Common.Objects;
using Rhino.Geometry;
using System;

namespace Blocks.Viewer.Commands
{
    public static class Generate
    {
        public static void Run(string methodName, int seed, int steps, bool useGroundPlane)
        {
            if (MainForm.Reference == null) { return; }

            Mesh groundPlane = useGroundPlane ? 
                Mesh.CreateFromPlane(new Plane(new Point3d(0,0,-10), Vector3d.ZAxis), new Interval(-1000, 1000), new Interval(-1000, 1000), 4, 4) : new Mesh();

            var transitions = new Transitions(MainForm.Reference.BlockAssembly);

            IBlockAssemblyGenerator generator;
            switch (methodName)
            {
                case "FromAssemblyGenerator":
                    generator = new FromAssemblyGenerator(MainForm.Reference.BlockAssembly, groundPlane, seed, steps);
                    break;
                case "FromTransitionsGenerator":
                    generator = new FromTransitionsGenerator(transitions, groundPlane, seed, steps);
                    break;
                case "EntangledCollisionsGenerator":
                    if (steps > 4) { throw new Exception(); }
                    generator = new EntangledCollisionsGenerator(transitions, groundPlane, seed, steps);
                    break;
                case "EntangledNeighbourGenerator":
                    if (steps > 6) { throw new Exception(); }
                    var parameters = Data.Preferences.BlockAssemblyReaderParameters.EdgeReaderParameters;
                    generator = new EntangledNeighbourGenerator(transitions, groundPlane, seed, steps, parameters);
                    break;
                case "RecursiveGenerator":
                    if (steps > 10) { throw new Exception(); }
                    generator = new RecursiveGenerator(transitions, groundPlane, seed, steps);
                    break;
                default:
                    throw new ArgumentException($"Unknown generator: {methodName}");
            }
            
            var result = generator.Generate();

            MainForm.SetGenerated(result);
        }
    }
}
