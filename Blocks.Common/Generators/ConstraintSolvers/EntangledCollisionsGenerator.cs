using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators.ConstraintSolvers
{

    /// <summary>
    /// Generates a block assembly by considering all possible states and elliminating the ones that result in collisions.
    /// </summary>
    public class EntangledCollisionsGenerator : EntangledNeighbourGeneratorBase, IBlockAssemblyGenerator
    {
        private RTree _rTree;
        private readonly EventHandler<RTreeEventArgs> _rTreeCallback;
        private readonly List<int> _rTreeCollisions;
        
        public EntangledCollisionsGenerator(Transitions transitions, Mesh obstacles, int seed, int depth) : base(transitions, obstacles, seed, depth)
        {
            _rTreeCollisions = new List<int>();
            _rTreeCallback = (sender, args) => _rTreeCollisions.Add(args.Id);
        }

        /// <summary>
        /// Generates a block assembly from a set of transitions.
        /// </summary>
        /// <returns>The block assembly.</returns>
        public BlockAssembly Generate()
        {
            _assembly = new BlockAssembly();
            _states = new Dictionary<int, State>();
            _rTree = new RTree();

            // Add a starting state
            var transition = _transitions.GetRandom(_random);
            var start = new State(
                transition.From, Transform.Identity,
                _transitions.FindFromBlockDefinition(transition.From), 0)
            {
                Collapsed = true
            };

            _assembly.AddInstance(start);
            _states.Add(start.Key, start);
            _rTree.Insert(start.BoundingBox, start.Key);

            // First recursiveley generate the states
            ProjectFutureStates(start, _depth);

            while (_states.Values.Any(s => !s.Collapsed))
            {
                // favour lower depth states
                State state = ChooseStateToCollapse();

                // Collapse the state
                Collapse(state);
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
            depth--;
            while (state.Transitions.Any() && depth >= 0)
            {
                var transition = state.Transitions.Pop(_random);

                if (CanPlace(state, transition, depth, out State next))
                {
                    // updates
                    state.Children.Add(next);
                    _states.Add(next.Key, next);

                    _rTree.Insert(next.BoundingBox, next.Key);

                    ProjectFutureStates(next, depth);
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
        public bool CanPlace(State existing, Relationship transition, int depth, out State next)
        {
            var nextTransform = existing.Transform * transition.Transform;

            // Make sure not to add the transition that was just visited
            IEnumerable<Relationship> newTransitions = _transitions.FindFromBlockDefinition(transition.To);
            newTransitions = newTransitions.Except(new List<Relationship> { transition });

            next = new State(transition.To, nextTransform, newTransitions.ToTransitions(), _depth - depth);

            // The key already exists
            if (_states.ContainsKey(next.Key)) { return false; }

            // The state collides with an obstacle
            if (Functions.CollisionCheck.CheckCollision(_obstacles, next.CollisionMesh)) { return false; }

            // the state collides with the existing assembly
            if (Functions.CollisionCheck.CheckCollision(_assembly, next)) { return false; }

            // use the rTree to determine quickly if the blocks intersect
            _rTreeCollisions.Clear();
            using (var treeB = new RTree())
            {
                treeB.Insert(next.BoundingBox, next.Key);
                RTree.SearchOverlaps(_rTree, treeB, tolerance: 0.1, callback: _rTreeCallback);
            }

            foreach (var other in _rTreeCollisions.Select(s => _states[s]))
            {
                bool overlapping = Functions.CollisionCheck.CheckCollision(other.CollisionMesh, next.CollisionMesh);
                if (!overlapping) { continue; }

                // update the entangled states
                other.EntangledCollisions.Add(next.Key);
                next.EntangledCollisions.Add(other.Key);
            }

            return true;
        }

        /// <summary>
        /// Choose Which state to collapse next.
        /// </summary>
        /// <returns>Returns a state.</returns>
        private State ChooseStateToCollapse()
        {
            State state = null;
            for (int j = 0; j <= _depth; j++)
            {
                var uncollapsed = _states.Values.Where(s => !s.Collapsed && s.Depth == j).ToList();
                if (uncollapsed.Any())
                {
                    int index = _random.Next(uncollapsed.Count());
                    state = uncollapsed[index];
                    break;
                }
            }
            return state;
        }

        /// <summary>
        /// Inform the entangled states that it has collapsed, thereby eliminating them.
        /// </summary>
        /// <param name="state">The state.</param>
        public void Collapse(State state) => Propogate(state, state.EntangledCollisions);

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

                foreach (var child in state.Children)
                {
                    Propogate(child, child.Key);
                }
            }

            // the states that will be removed
            var toRemove = new HashSet<int>(state.EntangledCollisions);
            toRemove.IntersectWith(remove);

            // remove the states
            state.EntangledCollisions.ExceptWith(remove);

            // if there are 0 entangled states, we can mark the state as collapsed and add it to the assembly
            if (state.EntangledCollisions.Count == 0 && !state.Eliminated)
            {
                state.Collapsed = true;
                _assembly.AddInstance(state);
            }

            foreach (int hash in toRemove)
            {
                if (!_states[hash].Collapsed && !_states[hash].Collapsed)
                {
                    Propogate(_states[hash], toRemove);
                }
            }
        }

        private void Propogate(State state, int remove) => Propogate(state, new HashSet<int> { remove });
    }
}
