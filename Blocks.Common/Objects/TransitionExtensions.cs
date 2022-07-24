using System.Collections.Generic;

namespace Blocks.Common.Objects
{
    public static class TransitionExtensions
    {
        public static Transitions ToTransitions(this IEnumerable<Transition> transitions) => new Transitions(transitions);
    }
}
