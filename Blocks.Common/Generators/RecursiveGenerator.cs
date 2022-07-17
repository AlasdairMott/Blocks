using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators
{
    public interface IBlockAssemblyGenerator
    {
        BlockAssembly Generate();
    }
    
    public class RecursiveGenerator : IBlockAssemblyGenerator
    {
        private readonly Transitions _transitions;
        private readonly Mesh _obstacles;
        private readonly Random _random;
        private readonly int _depth;
        private BlockAssembly _assembly;

        public RecursiveGenerator(Transitions transitions, Mesh obstacles, int seed, int depth)
        {
            _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _random = new Random(seed);
            _depth = depth;
        }

        public BlockAssembly Generate()
        {
            _assembly = new BlockAssembly();

            // Choose a random starting block
            var item = _transitions.GetRandom(_random);
            var startingBlock = new BlockInstanceState(item.From, Transform.Identity, _transitions.FindFromBlockDefinition(item.From));
            _assembly.AddInstance(startingBlock);

            var instances = Step(startingBlock, _depth).ToList();

            return _assembly;
        }

        private IEnumerable<BlockInstanceState> Step(BlockInstanceState instance, int depth)
        {
            depth--;
            while (instance.Transitions.Any() && depth >= 0)
            {
                var transition = instance.Transitions.Pop(_random);

                // Check to see if we can place the next block
                if (CanPlace(instance, transition, out BlockInstanceState next))
                {
                    _assembly.AddInstance(next);
                    _assembly.AddEdge(new Edge(instance, next));

                    foreach (var item in Step(next, depth)) { yield return item; }
                }
            }
        }
        
        private bool CanPlace(BlockInstanceState existing, Transition transition, out BlockInstanceState next)
        {
            // find the transform for the new location
            var nextTransform = existing.Transform * transition.Transform;
            var newTransitions = _transitions.FindFromBlockDefinition(transition.To);
            next = new BlockInstanceState(transition.To, nextTransform, newTransitions);

            bool obstructed = Functions.CollisionCheck.CheckCollision(_assembly, next) ||
                Functions.CollisionCheck.CheckCollision(_obstacles, next.CollisionMesh);

            return !obstructed;
        }
    }
}
