using Blocks.Common.Objects;
using Blocks.Common.Parameters;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators.ConstraintSolvers
{
    public class EntangledNeighbourGenerator : EntangledNeighbourGeneratorBase, IBlockAssemblyGenerator
    {
        private BlockAssemblyEdgeReaderParameters _neighbourParameters;

        public EntangledNeighbourGenerator(Transitions transitions, Mesh obstacles, int seed, int depth, BlockAssemblyEdgeReaderParameters neighbourParameters)
            : base(transitions, obstacles, seed, depth)
        {
            _neighbourParameters = neighbourParameters;
        }

        /// <summary>
        /// Generates a block assembly from a set of transitions.
        /// </summary>
        /// <returns>The block assembly.</returns>
        /// <remarks>Solves 1 ply at a time. resolve the entandled collisions and neighbouring transitions.</remarks>
        public BlockAssembly Generate()
        {
            _assembly = new BlockAssembly();
            _states = new Dictionary<int, State>();

            // Add a starting state
            var transition = _transitions.GetRandom(_random);
            var startTransitions = _transitions.FindFromBlockDefinition(transition.From);
            var start = new State(transition.From, Transform.Identity, startTransitions, 0);
            AddTransitionKeys(start, startTransitions);
            _states.Add(start.Key, start);

            start.Collapsed = true;
            _assembly.AddInstance(start);

            for (int i = 0; i < _depth; i++)
            {
                var unExplored = _states.Values.Where(s => s.Transitions.Any()).ToList();
                foreach (var state in unExplored)
                {
                    ProjectFutureStates(state, i);
                }

                while (_states.Values.Any(s => !s.Collapsed))
                {
                    // Choose a state to collapse
                    State state = _states.Values
                        .Where(s => !s.Collapsed)
                        .OrderBy(s => s.EntangledNeighbours.Count + s.EntangledCollisions.Count).First();

                    // Collapse the state
                    Propogate(state, state.EntangledCollisions);
                }

                var eliminatedStates = _states.Values.Where(s => s.Eliminated).ToList();
                eliminatedStates.ForEach(s => _states.Remove(s.Key));
            }

            // add edges for the assembly blocks
            AddEdges(start);

            return _assembly;
        }

        /// <summary>
        /// Recursive search to find future states given a current state.
        /// </summary>
        /// <param name="state">Current state.</param>
        /// <param name="depth">Depth of search.</param>
        private void ProjectFutureStates(State state, int depth)
        {
            while (state.Transitions.Any())
            {
                var transition = state.Transitions.Pop(_random);

                if (CanPlace(state, transition, depth, out State next))
                {
                    // updates
                    state.Children.Add(next);
                    _states.Add(next.Key, next);
                }
            }
        }

        /// <summary>
        /// Checks if the state can be placed at the given location.
        /// </summary>
        /// <param name="existing">The existing state.</param>
        /// <param name="transition">The transition.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="next">The next state.</param>
        /// <returns>True if the state can be placed at the given location.</returns>
        public bool CanPlace(State existing, Transition transition, int depth, out State next)
        {
            var nextTransform = existing.Transform * transition.Transform;

            // Make sure not to add the transition that was just visited
            IEnumerable<Transition> newTransitions = _transitions.FindFromBlockDefinition(transition.To);
            newTransitions = newTransitions.Except(new List<Transition> { transition });
            
            next = new State(transition.To, nextTransform, newTransitions.ToTransitions(), _depth - depth);
            AddTransitionKeys(next, newTransitions);

            // The key already exists
            if (_states.ContainsKey(next.Key)) { return false; }

            // The state collides with an obstacle
            if (Functions.CollisionCheck.CheckCollision(_obstacles, next.CollisionMesh)) { return false; }

            // the state collides with the existing assembly
            if (Functions.CollisionCheck.CheckCollision(_assembly, next)) { return false; }
            
            foreach (var other in _states.Values)
            {
                bool overlapping = Functions.CollisionCheck.CheckCollision(other.CollisionMesh, next.CollisionMesh);
                if (overlapping)
                {
                    // update the entangled collision states
                    other.EntangledCollisions.Add(next.Key);
                    next.EntangledCollisions.Add(other.Key);
                }

                //Compare instances. Are they touching?
                if (next.DistanceTo(other) <= _neighbourParameters.DistanceThreshold &&
                    Functions.CollisionCheck.CheckTouching(next, other, _neighbourParameters.CollisionArea))
                {
                    // update the entangled neighbouring states
                    other.EntangledNeighbours.Add(next.Key);
                    next.EntangledNeighbours.Add(other.Key);
                }
            }

            return true;
        }

        private void AddTransitionKeys(State state, IEnumerable<Transition> transitions)
        {
            foreach (var transition in transitions)
            {
                var nextTransform = state.Transform * transition.Transform;
                var next = new State(transition.To, nextTransform, new Transitions(), 0);
                state.TransitionKeys.Add(next.Key);
            }
        }

        /// <summary>
        /// Propogate the removed states to entangled states.
        /// </summary>
        /// <param name="state">The state to propogate changes to.</param>
        /// <param name="remove">A set of collision states to be removed.</param>
        private void Propogate(State state, HashSet<int> remove)
        {
            if (remove.Contains(state.Key) && !state.Collapsed)
            {
                state.Collapsed = true;
                state.Eliminated = true;
            }

            // remove the states
            state.EntangledCollisions.ExceptWith(remove);
            state.EntangledNeighbours.ExceptWith(remove);

            // if there are 0 entangled states, we can mark the state as collapsed and add it to the assembly
            if (state.EntangledCollisions.Count == 0 && !state.Eliminated)
            {
                state.Collapsed = true;
                _assembly.AddInstance(state);

                // if it has collapsed (+ve) then discard any neighbouring transitions that don't match that state
                var toRemove = state.EntangledNeighbours.Except(state.TransitionKeys);
                foreach (int i in toRemove) { remove.Add(i); }
            }

            foreach (int hash in remove)
            {
                if (!_states[hash].Eliminated && !_states[hash].Collapsed)
                {
                    Propogate(_states[hash], remove);
                }
            }
        }
    }
}
