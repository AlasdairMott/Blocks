using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Solvers
{
    public class GenerateBlockAssembly
    {
        private readonly Random _random;
        public GenerateBlockAssembly(int seed)
        {
            _random = new Random(seed);
        }

        public BlockAssembly PlaceGeometry(BlockPool pool, Mesh obstacles, int steps)
        {
            var assembly = new BlockAssembly();

            var item = pool.ElementAt(_random.Next(0, pool.Count()));
            assembly.AddInstance(new BlockInstance(item, Transform.Identity));

            for (var i = 0; i < steps; i++)
            {
                ComputeNext(assembly, pool, obstacles);
            }

            return assembly;
        }

        private void ComputeNext(BlockAssembly assembly, BlockPool pool, Mesh obstacles)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances.ElementAt(index);

            var next = pool.First(b => b.Index == existing.BlockDefinition.Index);
            if (next.Relationships.Count() == 0) { return; }

            var nextRelationship = next.Next(_random);
            var nextTransform = existing.Transform * nextRelationship.Transform;

            var instance = new BlockInstance(nextRelationship.Definition, nextTransform);
            if (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh))
            {
                assembly.AddInstance(instance);
            }
        }
    }
}
