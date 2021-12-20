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
        private readonly List<Transition> _transitions = new List<Transition>();
        private RelationshipComparer _comparer = new RelationshipComparer();
        public Transitions()
        {
        }

        public Transition this[int index]
        {
            get { return _transitions[index]; }
            set { _transitions.Insert(index, value); }
        }

        public IEnumerator<Transition> GetEnumerator() => _transitions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Transition GetRandom(Random random)
        {
            if (!_transitions.Any()) { throw new IndexOutOfRangeException("No tramsitions to choose from"); }
            var value = random.NextDouble();
            var shuffled = _transitions.OrderBy(t => random.NextDouble());
            var result = shuffled.FirstOrDefault(t => t.Probability > value);
            if (result != null) { return result; }
            return shuffled.First();
        }

        public Transitions FindFromBlockDefinition(BlockDefinition definition)
        {
            var matching = _transitions.Where(t =>
                t.From.BlockDefinition == definition ||
                t.To.BlockDefinition == definition);

            return matching.Select(m => 
                m.From.BlockDefinition == definition ? m : m.Invert()) as Transitions;
        }

        public Transition Find(Transition transition)
        {
            return _transitions.FirstOrDefault(r => _comparer.Equals(r, transition));
        }

        public void Add(Transition transition) => _transitions.Add(transition);

        public void NormalizeRelationships()
        {
            var mass = _transitions.Sum(t => t.Probability);
            foreach (var transition in _transitions)
            {
                transition.Probability /= mass;
            }
        }
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
