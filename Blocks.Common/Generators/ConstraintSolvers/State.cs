using Blocks.Common.Objects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Generators.ConstraintSolvers
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
                hash = hash * 13 ^ BlockDefinition.Name.GetHashCode();
                return hash;
            }
        }

        public List<State> Children { get; } = new List<State>();

        public IEnumerable<State> AllChildren => Children.SelectMany(c => c.Children.Concat(c.AllChildren));
    }
}
