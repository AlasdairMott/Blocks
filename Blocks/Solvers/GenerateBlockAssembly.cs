using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Solvers
{
    public class GenerateBlockAssembly
    {
        public GenerateBlockAssembly()
        {
        }

        public BlockAssembly PlaceGeometry(BlockPool pool, Mesh obstacles, int seed, int iterations)
        {
            var assembly = new BlockAssembly();

            var random = new Random(seed);

            var item = pool.ElementAt(random.Next(0, pool.Count()));
            assembly.AddInstance(new BlockInstance(item, Transform.Identity));

            for (var i = 0; i < iterations; i++)
            {
                var index = random.Next(0, assembly.Size);
                var existing = assembly.BlockInstances.ElementAt(index);

                var next = pool.First(b => b.Index == existing.BlockDefinition.Index);
                if (next.Relationships.Count() == 0) { continue; }

                var nextRelationship = next.Next(random);
                var nextTransform = existing.Transform * nextRelationship.Transform;

                var instance = new BlockInstance(nextRelationship.Definition, nextTransform);
                if (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                    !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh))
                {
                    assembly.AddInstance(instance);
                }
            }

            return assembly;
        }
    }
}
