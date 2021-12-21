using Blocks.Objects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Blocks.Generators
{
    public class GenerateFromTransitions
    {
        private readonly Random _random;
        public GenerateFromTransitions(int seed)
        {
            _random = new Random(seed);
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
            assembly.AddInstance(new BlockInstance(item.From, Transform.Identity));

            for (var i = 0; i < steps; i++)
            {
                var next = ChooseBlock(assembly, transitions);

                var nextTransform =  next.Existing.Transform * next.Relationship.Transform;
                var nextInstance = new BlockInstance(next.Relationship.To, nextTransform);

                if (CanPlace(nextInstance, assembly, obstacles))
                {
                    assembly.AddInstance(nextInstance);
                }
            }

            return assembly;
        }

        /// <summary>
        /// Try to find a BlockInstance that can be placed in the assembly.
        /// </summary>
        /// <param name="assembly">The BlockAssembly to add to.</param>
        /// <param name="transitions">Transitions to choose from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <returns>The chosen block instance.</returns>
        private (BlockInstance Existing, Relationship Relationship) ChooseBlock(BlockAssembly assembly, Transitions transitions)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances[index];

            var options = transitions.FindFromBlockDefinition(existing.BlockDefinition);

            return (existing, options.GetRandom(_random));
        }

        private bool CanPlace(BlockInstance instance, BlockAssembly assembly,Mesh obstacles)
        {
            return (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                    !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh));
        }
    }
}
