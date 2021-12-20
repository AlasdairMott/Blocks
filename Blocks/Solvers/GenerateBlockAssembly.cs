using Blocks.Objects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Solvers
{
    /// <summary>
    /// Generate a block assembly.
    /// </summary>
    public class GenerateBlockAssembly
    {
        private readonly Random _random;
        public GenerateBlockAssembly(int seed)
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

            var nextBlockDefinition = nextRelationship.To.BlockDefinition == existing.BlockDefinition ? nextRelationship.To : nextRelationship.From;
            var nextTransform = existing.Transform * nextRelationship.Transform;

            var instance = new BlockInstance(nextBlockDefinition.BlockDefinition, nextTransform);
            if (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh))
            {
                assembly.AddInstance(instance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generate a block assembly from a collection of block definitions.
        /// </summary>
        /// <param name="transitions">Transitions to choose blocks from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <param name="steps">The number of steps. Each step the algorithm will attempt to place down a BlockInstance.</param>
        /// <returns>A new BlockAssembly.</returns>
        public BlockAssembly Generate(Transitions transitions, Mesh obstacles, int steps)
        {
            var assembly = new BlockAssembly();

            var item = transitions.GetRandom(_random);
            assembly.AddInstance(new BlockInstance(item.From.BlockDefinition, Transform.Identity));

            for (var i = 0; i < steps; i++)
            {
                TryPlace(assembly, transitions, obstacles);
            }

            return assembly;
        }

        /// <summary>
        /// Try to place a BlockInstance in the assembly.
        /// </summary>
        /// <param name="assembly">The BlockAssembly to add to.</param>
        /// <param name="transitions">Transitions to choose from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool TryPlace(BlockAssembly assembly, Transitions transitions, Mesh obstacles)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances.ElementAt(index);

            var options = transitions.FindFromBlockDefinition(existing.BlockDefinition);

            var nextRelationship = options.GetRandom(_random);
            var nextTransform = existing.Transform * nextRelationship.Transform;

            var instance = new BlockInstance(nextRelationship.From.BlockDefinition, nextTransform);
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
