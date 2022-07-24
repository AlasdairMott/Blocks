using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators.ConstraintSolvers
{
    public class EntangledNeighbourGeneratorBase
    {
        protected readonly Transitions _transitions;
        protected readonly Mesh _obstacles;
        protected readonly Random _random;
        protected readonly int _depth;
        protected BlockAssembly _assembly;
        protected Dictionary<int, State> _states;

        public EntangledNeighbourGeneratorBase(Transitions transitions, Mesh obstacles, int seed, int depth)
        {
            _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _random = new Random(seed);
            _depth = depth;
        }

        /// <summary>
        /// Add edges to the assembly.
        /// </summary>
        /// <param name="state">State that is being added to the assembly.</param>
        protected void AddEdges(State state)
        {
            foreach (var child in state.Children.Where(c => !c.Eliminated))
            {
                var edge = new TransitionInstance(state, child);
                _assembly.AddEdge(edge);

                AddEdges(child);
            }

            //foreach (var neighbourKey in state.EntangledNeighbours)
            //{
            //    if (_states.TryGetValue(neighbourKey, out State neighbour))
            //    {
            //        if (neighbour.Collapsed && !neighbour.Eliminated)
            //        {
            //            var edge = new Edge(state, neighbour);

            //            // might be adding duplicates here
            //            _assembly.AddEdge(edge);
            //        }
            //    }
            //}
        }
    }
}