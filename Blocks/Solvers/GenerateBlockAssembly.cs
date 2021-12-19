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

        /// <summary>
        /// Generate a block assembly from a collection of block definitions.
        /// </summary>
        /// <param name="pool">The BlockPool to choose blocks from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <param name="steps">The number of steps. Each step the algorithm will attempt to place down a BlockInstance.</param>
        /// <returns>A new BlockAssembly.</returns>
        public BlockAssembly Generate(BlockPool pool, Mesh obstacles, int steps)
        {
            var assembly = new BlockAssembly();

            var item = pool.ElementAt(_random.Next(0, pool.Count()));
            assembly.AddInstance(new BlockInstance(item, Transform.Identity));

            for (var i = 0; i < steps; i++)
            {
                TryPlace(assembly, pool, obstacles);
            }

            return assembly;
        }

        /// <summary>
        /// Try to place a BlockInstance in the assembly.
        /// </summary>
        /// <param name="assembly">The BlockAssembly to add to.</param>
        /// <param name="pool">The BlockPool to choose from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool TryPlace(BlockAssembly assembly, BlockPool pool, Mesh obstacles)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances.ElementAt(index);

            var next = pool.First(b => b.Index == existing.BlockDefinition.Index);
            if (next.Relationships.Count() == 0) { return false; }

            var nextRelationship = next.Next(_random);
            var nextTransform = existing.Transform * nextRelationship.Transform;

            var instance = new BlockInstance(nextRelationship.Definition, nextTransform);
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
