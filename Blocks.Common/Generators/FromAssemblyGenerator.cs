using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Common.Generators
{
    /// <summary>
    /// Generate a block assembly.
    /// </summary>
    public class FromAssemblyGenerator : IBlockAssemblyGenerator
    {
        private readonly Random _random;
        private readonly BlockAssembly _example;
        private readonly Mesh _obstacles;
        private readonly int _steps;

        public FromAssemblyGenerator(BlockAssembly example, Mesh obstacles, int seed, int steps)
        {
            _example = example ?? throw new ArgumentNullException(nameof(example));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _steps = steps;
            _random = new Random(seed);
        }

        public BlockAssembly Generate()
        {
            var assembly = new BlockAssembly();

            var item = _example.BlockInstances.ElementAt(_random.Next(0, _example.Size));
            assembly.AddInstance(new BlockInstance(item.BlockDefinition, Transform.Identity));

            for (var i = 0; i < _steps; i++)
            {
                TryPlace(assembly, _example, _obstacles);
            }

            return assembly;
        }

        private bool TryPlace(BlockAssembly assembly, BlockAssembly example, Mesh obstacles)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances.ElementAt(index);

            var sampledTransitions = example.FindFromBlockDefinition(existing.BlockDefinition);

            var nextTransition = sampledTransitions.OrderBy(r => _random.NextDouble()).FirstOrDefault();
            if (nextTransition == null) { return false; }

            //var nextBlockDefinition = nextTransition.To == existing.BlockDefinition ? nextTransition.To : nextTransition.From;
            var nextBlockDefinition = nextTransition.To;
            var nextTransform = existing.Transform * nextTransition.Transform;

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
