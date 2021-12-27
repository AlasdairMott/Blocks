using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators
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
                var nextInstance = ChooseBlock(assembly, transitions);

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
        /// <returns>The chosen block instance.</returns>
        private BlockInstance ChooseBlock(BlockAssembly assembly, Transitions transitions)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances[index];

            var options = transitions.FindFromBlockDefinition(existing.BlockDefinition);

            //Unable to use options.GetRandom() here since that will remove the inverted copies
            var nextTransition = options.GetRandom(_random);

            var nextTransform = existing.Transform * nextTransition.Transform;
            return new BlockInstance(nextTransition.To, nextTransform);
        }

        /// <summary>
        /// Check whether an instance can be places in an existing assembly.
        /// </summary>
        /// <param name="instance">The BlockInstanceS to add.</param>
        /// <param name="assembly">The BlockAssembly to add to.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <returns></returns>
        private bool CanPlace(BlockInstance instance, BlockAssembly assembly,Mesh obstacles)
        {
            return (!Functions.CollisionCheck.CheckCollision(assembly, instance) &&
                    !Functions.CollisionCheck.CheckCollision(obstacles, instance.CollisionMesh));
        }
    }
}
