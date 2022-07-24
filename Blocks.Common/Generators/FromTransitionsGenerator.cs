using Blocks.Common.Objects;
using Rhino.Geometry;
using System;

namespace Blocks.Common.Generators
{
    public class FromTransitionsGenerator : IBlockAssemblyGenerator
    {
        private readonly Random _random;
        private readonly Transitions _transitions;
        private readonly Mesh _obstacles;
        private readonly int _steps;
        public FromTransitionsGenerator(Transitions transitions, Mesh obstacles,int seed, int steps)
        {
            _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _steps = steps;
            _random = new Random(seed);
        }

        /// <summary>
        /// Generate a block assembly from a collection of block definitions.
        /// </summary>
        /// <param name="transitions">Transitions to choose blocks from.</param>
        /// <param name="obstacles">Obstacles to avoid.</param>
        /// <param name="steps">The number of steps. Each step the algorithm will attempt to place down a BlockInstance.</param>
        /// <returns>A new BlockAssembly.</returns>
        public BlockAssembly Generate()
        {
            var assembly = new BlockAssembly();

            var item = _transitions.GetRandom(_random);
            assembly.AddInstance(new BlockInstance(item.From, Transform.Identity));

            for (var i = 0; i < _steps; i++)
            {
                var nextEdge = ChooseBlock(assembly, _transitions);

                if (CanPlace(nextEdge.ToInstance, assembly, _obstacles))
                {
                    assembly.AddInstance(nextEdge.ToInstance);
                    assembly.AddEdge(nextEdge);
                }
            }

            return assembly;
        }

        /// <summary>
        /// Try to find a BlockInstance that can be placed in the assembly.
        /// </summary>
        /// <param name="assembly">The BlockAssembly to add to.</param>
        /// <param name="transitions">Transitions to choose from.</param>
        /// <returns>The edge from an existing block to a new block instance.</returns>
        private TransitionInstance ChooseBlock(BlockAssembly assembly, Transitions transitions)
        {
            var index = _random.Next(0, assembly.Size);
            var existing = assembly.BlockInstances[index];

            var options = transitions.FindFromBlockDefinition(existing.BlockDefinition);

            //Unable to use options.GetRandom() here since that will remove the inverted copies
            var nextTransition = options.GetRandom(_random);

            var nextTransform = existing.Transform * nextTransition.Transform;
            return new TransitionInstance(existing, new BlockInstance(nextTransition.To, nextTransform));
        }

        /// <summary>
        /// Check whether an instance can be placed in an existing assembly.
        /// </summary>
        /// <param name="instance">The BlockInstance to add.</param>
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
