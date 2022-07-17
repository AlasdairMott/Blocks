using Blocks.Common.Objects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators
{
    public class State : BlockInstanceState
    {
        public HashSet<int> Entangled { get; set; } = new HashSet<int>();
        public int Depth { get; }
        public int Key => GetHashCode();
        public bool Collapsed { get; set; } = false;
        public bool Eliminated { get; set; } = false;
        
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

        public void Eliminate()
        {
            Collapsed = true;
            Eliminated = true;

            foreach (var child in Children)
            {
                child.Eliminate();
            }
        }

        public IEnumerable<State> AllChildren => Children.SelectMany(c => c.Children.Concat(c.AllChildren));

        public void Reify() => Collapsed = true;
    }
    public class WFCGenerator : IBlockAssemblyGenerator
    {
        private readonly Transitions _transitions;
        private readonly Mesh _obstacles;
        private readonly Random _random;
        private readonly int _depth;
        private BlockAssembly _assembly;

        //private List<State> _collapsed;
        private Dictionary<int, State> _uncertain;
        
        public WFCGenerator(Transitions transitions, Mesh obstacles, int seed, int depth)
        {
            _transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
            _obstacles = obstacles ?? throw new ArgumentNullException(nameof(obstacles));
            _random = new Random(seed);
            _depth = depth;
        }

        public BlockAssembly Generate()
        {
            _assembly = new BlockAssembly();
            _uncertain = new Dictionary<int, State>();

            // Add a starting state
            var transition = _transitions.GetRandom(_random);
            var start = new State(
                transition.From, Transform.Identity,
                _transitions.FindFromBlockDefinition(transition.From), 0)
            { 
                Collapsed = true 
            };

            _assembly.AddInstance(start);
            _uncertain.Add(start.Key, start);

            // First recursiveley generate the states
            ProjectFutureStates(start, _depth);

            #region debugging
            //foreach (var state in _uncertain.Values)
            //{
            //    int r = Math.Abs(state.Key * 3 % 255);
            //    int g = Math.Abs(state.Key * 7 % 255);
            //    int b = Math.Abs(state.Key * 13 % 255);
            //    var color = System.Drawing.Color.FromArgb(r, g, b);

            //    foreach (var key in state.Entangled)
            //    {
            //        var line = new Line(state.Location, _uncertain[key].Location);
            //        Rhino.RhinoDoc.ActiveDoc.Objects.AddLine(line, new Rhino.DocObjects.ObjectAttributes
            //        {
            //            ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
            //            ObjectColor = color
            //        });
            //    }
            //}
            #endregion

            int i = 0;
            while (_uncertain.Values.Any(s => !s.Collapsed))
            {
                i++;

                // favour lower depth states
                State state = null;
                for (int j = 0; j <= _depth; j++)
                {
                    var uncollapsed = _uncertain.Values.Where(s => !s.Collapsed && s.Depth == j).ToList();
                    if (uncollapsed.Any())
                    {
                        int index = _random.Next(uncollapsed.Count());
                        state = uncollapsed[index];
                        break;
                    }
                }

                // Choose an uncollapsed state
                //var uncollapsed = _uncertain.Values.Where(s => !s.Collapsed).ToList();
                //int index = _random.Next(uncollapsed.Count());
                //state = uncollapsed[index];

                //var rdoc = Rhino.RhinoDoc.ActiveDoc.Objects;
                //rdoc.AddMesh(state.CollisionMesh, new Rhino.DocObjects.ObjectAttributes
                //{
                //    ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
                //    ObjectColor = System.Drawing.Color.Green
                //});

                // Collapse the state
                Collapse(state);
            }

            #region debugging

            //foreach (State state in _assembly.BlockInstances)
            //{
            //    var rdoc = Rhino.RhinoDoc.ActiveDoc.Objects;

            //    var mesh = Functions.GeometryHelpers.GetBlockInstanceMesh(state);
            //    rdoc.AddMesh(mesh, new Rhino.DocObjects.ObjectAttributes 
            //    { 
            //        Name = $"{state.Depth}",
            //    });

            //    rdoc.AddTextDot($"{state.Depth}", state.Location);
            //}

            //foreach (State state in _uncertain.Values.Where(s => s.Eliminated))
            //{
            //    var rdoc = Rhino.RhinoDoc.ActiveDoc.Objects;

            //    var mesh = Functions.GeometryHelpers.GetBlockInstanceMesh(state);
            //    rdoc.AddMesh(mesh, new Rhino.DocObjects.ObjectAttributes
            //    {
            //        Name = $"{state.Depth}",
            //        ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject,
            //        ObjectColor = System.Drawing.Color.Red
            //    });
            //}

            #endregion

            // add edges for the assembly blocks
            AddEdges(start);

            return _assembly;
        }

        private void AddEdges(State state)
        {
            foreach (var child in state.Children.Where(c => !c.Eliminated))
            {
                var edge = new Edge(state, child);
                _assembly.AddEdge(edge);

                AddEdges(child);
            }
        }
        
        private void ProjectFutureStates(State state, int depth)
        {
            depth--;
            while (state.Transitions.Any() && depth >= 0)
            {
                var transition = state.Transitions.Pop(_random);
                
                if (CanPlace(state, transition, depth, out State next))
                {
                    state.Children.Add(next);
                    _uncertain.Add(next.Key, next);
                    ProjectFutureStates(next, depth);
                }
            }
        }

        public bool CanPlace(State existing, Transition transition, int depth, out State next)
        {
            // each time we place a new state, check which states it will be entangled with
            var nextTransform = existing.Transform * transition.Transform;

            // Make sure not to add the transition that was just visited
            IEnumerable<Transition> newTransitions = _transitions.FindFromBlockDefinition(transition.To);
            //  newTransitions = newTransitions.Except(new List<Transition> { transition });

            next = new State(transition.To, nextTransform, newTransitions.ToTransitions(), _depth - depth);

            if (_uncertain.ContainsKey(next.Key))
            {
                return false;
            }

            foreach (var other in _uncertain.Values)
            {
                var overlapping = Functions.CollisionCheck.CheckCollision(other.CollisionMesh, next.CollisionMesh);
                if (!overlapping) { continue; }

                // Can't place if it collides with a collapsed state
                if (other.Collapsed)
                {
                    return false;
                }

                // update the entangled states
                other.Entangled.Add(next.Key);
                next.Entangled.Add(other.Key);
            }

            return true;
        }
        
        public void Collapse(State state)
        {
            // inform the entangled states that it has collapsed, thereby eliminating them
            Propogate(state, new HashSet<int>());
        }
        
        private void Propogate(State state, HashSet<int> remaining)
        {
            // if there are 0 entangled states, we can mark the state as collapsed and add it to the assembly
            if (state.Entangled.Count == 0)
            {
                state.Collapsed = true;
                _assembly.AddInstance(state);
            } 
            else if (remaining.Contains(state.Key))
            {
                
                state.Eliminate();

                //state.Collapsed = true;
                //state.Eliminated = true;

                // add the all state's children to the list of hashsets to remove

                // CHange the "remaining" to a "elimataion?"
            }

            var toRemove = new HashSet<int>(state.Entangled);
            toRemove.Except(remaining);
            
            state.Entangled.IntersectWith(remaining);

            foreach (int hash in toRemove)
            {
                if (!_uncertain[hash].Collapsed)
                {
                    Propogate(_uncertain[hash], toRemove);
                }
            }
        }
    }
}
