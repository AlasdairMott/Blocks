using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators
{
    public class State : BlockInstanceState
    {
        public int Depth { get; }
        public int Key => GetHashCode();
        public bool Collapsed { get; set; } = false;
        public bool Eliminated { get; set; } = false;
        public HashSet<int> Entangled { get; set; } = new HashSet<int>();
        
        public State(BlockDefinition blockDefinition, Transform transform, Transitions transitions, int depth) : base(blockDefinition, transform, transitions)
        {
            Depth = depth;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Transform.GetHashCode();
                hash = (hash * 13) ^ BlockDefinition.Name.GetHashCode();
                return hash;
            }
        }

        public List<State> Children { get; } = new List<State>();

        public IEnumerable<State> AllChildren => Children.SelectMany(c => c.Children.Concat(c.AllChildren));
    }

    public class WFCDebugger
    {
        // each propogation step is a state
        // using a dictoinary of the integer keys to track each instance

        // track the number of entanglements each state has
        // color based on the number entanglements
        // alpha = 0.1 is max
        // alpha = 1 is 0 entanglements

        private List<double[]> _entropy;
        public IReadOnlyList<double[]> Entropy => _entropy;
        
        public void Bake(IEnumerable<State> states)
        {
            var docObjects = Rhino.RhinoDoc.ActiveDoc.Objects;
            foreach (var state in states)
            {
                
            }
        }

        public static System.Drawing.Color ColorFromState(State state) 
        {
            int r = Math.Abs(state.Key * 3 % 255);
            int g = Math.Abs(state.Key * 7 % 255);
            int b = Math.Abs(state.Key * 13 % 255);
            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }

    public class WFCGenerator : IBlockAssemblyGenerator
    {
        private readonly Transitions _transitions;
        private readonly Mesh _obstacles;
        private readonly Random _random;
        private readonly int _depth;
        private BlockAssembly _assembly;

        private Dictionary<int, State> _states;
        private RTree _rTree;
        private List<int> _rTreeCollisions = new List<int>();

        public WFCGenerator(Transitions transitions, Mesh obstacles, int seed, int depth)
        {
            _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _random = new Random(seed);
            _depth = depth;
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

            int i = 0;
            while (_states.Values.Any(s => !s.Collapsed))
            {
                i++;

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
        public bool CanPlace(State existing, Transition transition, int depth, out State next)
        {
            // each time we place a new state, check which states it will be entangled with
            var nextTransform = existing.Transform * transition.Transform;

            // Make sure not to add the transition that was just visited
            IEnumerable<Transition> newTransitions = _transitions.FindFromBlockDefinition(transition.To);
            //  newTransitions = newTransitions.Except(new List<Transition> { transition });

            next = new State(transition.To, nextTransform, newTransitions.ToTransitions(), _depth - depth);

            if (_states.ContainsKey(next.Key))
            {
                return false;
            }

            // use the rTree to determine quickly if the blocks intersect
            _rTreeCollisions.Clear();
            using (var treeB = new RTree())
            {
                treeB.Insert(next.BoundingBox, next.Key);
                RTree.SearchOverlaps(_rTree, treeB, tolerance: 0.1, callback: (sender, args) => _rTreeCollisions.Add(args.Id));
            }
            
            var collisions_compare = new List<int>();

            foreach (var other in _states.Values)
            //foreach (var other in _rTreeCollisions.Select(s => _states[s]))
            {
                var overlapping = Functions.CollisionCheck.CheckCollision(other.CollisionMesh, next.CollisionMesh);
                if (!overlapping) { continue; }

                collisions_compare.Add(other.Key);

                // Can't place if it collides with a collapsed state
                //if (other.Collapsed)
                //{
                        // need to clean up references to this key in the entanglements up to this point
                //    return false;
                //}

                // update the entangled states
                other.Entangled.Add(next.Key);
                next.Entangled.Add(other.Key);
            }

            //if (next.Key == 1996170078)
            //{
            //    Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(next.BoundingBox.ToBrep());

            //    int group = Rhino.RhinoDoc.ActiveDoc.Groups.Add();
            //    foreach (var item in _rTreeCollisions)
            //    {
            //        var brep = _uncertain[item].BoundingBox.ToBrep();
            //        var brepObject = Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
            //        Rhino.RhinoDoc.ActiveDoc.Groups.AddToGroup(group, brepObject);
            //    }

            //    Rhino.RhinoDoc.ActiveDoc.Objects.AddMesh(next.Mesh, new Rhino.DocObjects.ObjectAttributes { 
            //        ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
            //        ObjectColor = System.Drawing.Color.Red
            //    });

            //    foreach (var item in next.Entangled)
            //    {
            //        Rhino.RhinoDoc.ActiveDoc.Objects.AddMesh(_uncertain[item].Mesh, new Rhino.DocObjects.ObjectAttributes
            //        {
            //            ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
            //            ObjectColor = System.Drawing.Color.Maroon
            //        });
            //    }
            //}

            return true;
        }

        /// <summary>
        /// Choose Which state to collapse next.
        /// </summary>
        /// <returns>Returns a state.</returns>
        private State ChooseStateToCollapse()
        {
            State state = null;

            #region shallow depth and low entropy
            //double cumulativeDepth = _uncertain.Values.Sum(s => s.Depth);
            //double cumulativeEntropy = _uncertain.Values.Sum(s => s.Entangled.Count);
            //double maxEntropy = _uncertain.Values.Max(s => s.Entangled.Count);
            //double randomValue = _random.NextDouble();

            //double cumulative = 0;
            //return _uncertain.Values.First(s =>
            //{
            //    double p = (_depth-s.Depth+1) / cumulativeDepth;
            //    double q = (maxEntropy-s.Entangled.Count) / cumulativeEntropy;
            //    cumulative += (p + q);

            //    return cumulative > randomValue;
            //});

            #endregion

            #region shallow depth first
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
            #endregion

            #region completely random
            // Choose an uncollapsed state
            //var uncollapsed = _uncertain.Values.Where(s => !s.Collapsed).ToList();
            //int index = _random.Next(uncollapsed.Count());
            //state = uncollapsed[index];
            #endregion

            //var rdoc = Rhino.RhinoDoc.ActiveDoc.Objects;
            //rdoc.AddMesh(state.CollisionMesh, new Rhino.DocObjects.ObjectAttributes
            //{
            //    ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
            //    ObjectColor = System.Drawing.Color.Green
            //});

            return state;
        }

        /// <summary>
        /// Inform the entangled states that it has collapsed, thereby eliminating them.
        /// </summary>
        /// <param name="state">The state.</param>
        public void Collapse(State state) => Propogate(state, state.Entangled);

        /// <summary>
        /// Propogate the removed states to entangled states.
        /// </summary>
        /// <param name="state">The state to propogate changes to.</param>
        /// <param name="remove">A set of states to be removed.</param>
        private void Propogate(State state, HashSet<int> remove)
        {
            if (remove.Contains(state.Key))
            {
                state.Collapsed = true;
                state.Eliminated = true;

                foreach (var child in state.Children)
                {
                    Propogate(child, child.Key);
                }
            }

            // the states that will be removed
            var toRemove = new HashSet<int>(state.Entangled);
            toRemove.IntersectWith(remove);

            // remove the states
            state.Entangled.ExceptWith(remove);

            // if there are 0 entangled states, we can mark the state as collapsed and add it to the assembly
            if (state.Entangled.Count == 0 && !state.Eliminated)
            {
                state.Collapsed = true;
                _assembly.AddInstance(state);
            }

            foreach (int hash in toRemove)
            {
                if (!_states[hash].Collapsed)
                {
                    Propogate(_states[hash], toRemove);
                }
            }
        }

        private void Propogate(State state, int remove) => Propogate(state, new HashSet<int> { remove });

        private void AddEdges(State state)
        {
            foreach (var child in state.Children.Where(c => !c.Eliminated))
            {
                var edge = new Edge(state, child);
                _assembly.AddEdge(edge);

                AddEdges(child);
            }
        }
    }
}
