using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Objects
{
    /// <summary>
    /// A collection of BlockDefinitions that can be chosen from when generating a BlockAssembly.
    /// </summary>
    public class Transitions : IEnumerable<Transition>
    {
        private readonly HashSet<Transition> _transitions;
        private RelationshipComparer _comparer = new RelationshipComparer();
        public Transitions()
        {
            _transitions = new HashSet<Transition>(_comparer);
        }

        public Transitions(IEnumerable<Transition> transitions):this()
        {
            _transitions.UnionWith(transitions);
        }

        public Transition this[int index]
        {
            get { return _transitions.ElementAt(index); }
            set { _transitions.Add(value); }
        }

        public IEnumerator<Transition> GetEnumerator() => _transitions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Transition GetRandom(Random random)
        {
            if (!_transitions.Any()) { throw new IndexOutOfRangeException("No transitions to choose from"); }
            var value = random.NextDouble();
            var shuffled = _transitions.OrderBy(t => random.NextDouble());
            var result = shuffled.FirstOrDefault(t => t.Probability > value);
            //if (result != null) { return result; }
            return shuffled.First();
        }

        public Transitions FindFromBlockDefinition(BlockDefinition definition)
        {
            var transitions_A_to_B = _transitions.Where(t => t.From.BlockDefinition.Index == definition.Index);
            var transitions_B_to_A = _transitions.Where(t => t.To.BlockDefinition.Index == definition.Index);

            return transitions_A_to_B.Concat(transitions_B_to_A.Select(t => t.Invert())).ToTransitions();
        }

        public Transition Find(Transition transition)
        {
            return _transitions.FirstOrDefault(r => _comparer.Equals(r, transition));
        }

        public void Add(Transition transition) {
            if (_transitions.Contains(transition))
            {
                var existing = Find(transition);
                existing.Probability += 1;
            }
            else
            {
                _transitions.Add(transition);
            }
        }

        public void NormalizeRelationships()
        {
            var mass = _transitions.Sum(t => t.Probability);
            foreach (var transition in _transitions)
            {
                transition.Probability /= mass;
            }
        }
    }

    public static class TransitionExtensions
    {
        public static Transitions ToTransitions(this IEnumerable<Transition> transitions) => new Transitions(transitions);
    }

    public class Transition : Relationship
    {
        public Transition(BlockInstance from, BlockInstance to) : base(from, to)
        {
        }

        public double Probability { get; set; } = 1.0;
        public new Transition Invert() => new Transition(To, From);
    }
}
