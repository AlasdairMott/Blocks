using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Common.Generators
{
    /// <summary>
    /// Generate a block assembly.
    /// </summary>
    public class FromAssemblyGenerator
    {
        private readonly Random _random;
        public FromAssemblyGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public BlockAssembly Generate(BlockAssembly example, Mesh obstacles, int steps)
        {
            var assembly = new BlockAssembly();

            var item = example.BlockInstances.ElementAt(_random.Next(0, example.Size));
            assembly.AddInstance(new BlockInstance(item.BlockDefinition, Transform.Identity));

            for (var i = 0; i < steps; i++)
            {
                TryPlace(assembly, example, obstacles);
            }

            return assembly;
        }

        private bool TryPlace(BlockAssembly assembly, BlockAssembly example, Mesh obstacles)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances.ElementAt(index);

            var sampledRelationships = assembly.FindFromBlockDefinition(existing.BlockDefinition);

            var nextRelationship = sampledRelationships.OrderBy(r => _random.NextDouble()).FirstOrDefault();
            if (nextRelationship == null) { return false; }

            var nextBlockDefinition = nextRelationship.To == existing.BlockDefinition ? nextRelationship.To : nextRelationship.From;
            var nextTransform = existing.Transform * nextRelationship.Transform;

            var instance = new BlockInstance(nextBlockDefinition, nextTransform);
            if (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh))
            {
                assembly.AddInstance(instance);
                return true;
            }
            return false;
        }
    }
}
