using System.Collections.Generic;

namespace Blocks.Common.Objects
{
    public static class TransitionExtensions
    {
        public static Transitions ToTransitions(this IEnumerable<Relationship> transitions) => new Transitions(transitions);
    }
}
