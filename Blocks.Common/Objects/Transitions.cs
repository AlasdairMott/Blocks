using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// A collection of BlockDefinitions that can be chosen from when generating a BlockAssembly.
    /// </summary>
    public class Transitions : IEnumerable<Relationship>
    {
        private readonly Dictionary<Relationship, int> _transitions;
        private Dictionary<Relationship, double> _probabilities;
  
        private RelationshipComparer _comparer = new RelationshipComparer();
        public IReadOnlyDictionary<Relationship, int> Counts => _transitions;
        public IReadOnlyDictionary<Relationship, double> Probabilities
        {
            get => _probabilities ?? (_probabilities = ComputeProbabilities());
        }

        public Transitions()
        {
            _transitions = new Dictionary<Relationship, int>(_comparer);
        }

        public Transitions(IEnumerable<Relationship> transitions):this()
        {
            foreach (var transition in transitions) { Push(transition); }
        }

        public Transitions(BlockAssembly assembly) : 
            this (assembly.Edges.Select(r => new Relationship(r)).ToList()){
        }

        public Relationship this[int index]
        {
            get => _transitions.Keys.ElementAt(index);
        }

        public IEnumerator<Relationship> GetEnumerator() => _transitions.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Relationship GetRandom(Random random)
        {
            if (!_transitions.Any()) { throw new IndexOutOfRangeException("No transitions to choose from"); }

            double value = random.NextDouble();
            double total = 0.0;
            return _transitions.Keys.FirstOrDefault(t =>
            {
                total += Probabilities[t];
                return total >= value;
            });
        }

        public Transitions FindFromBlockDefinition(BlockDefinition definition)
        {
            var transitions_A_to_B = _transitions.Keys.Where(t => t.From.Name == definition.Name);
            var transitions_B_to_A = _transitions.Keys.Where(t => t.To.Name == definition.Name);

            var transitions = transitions_A_to_B.Concat(transitions_B_to_A.Select(t => t.Invert()));

            return transitions.Select(t => t.Clone()).ToTransitions();
        }

        public void Push(Relationship transition) {
            _probabilities = null;
            if (!_transitions.ContainsKey(transition))
            {
                _transitions.Add(transition, 1);
            }
            else
            {
                _transitions[transition]++;
            }
        }

        public Relationship Pop(Random random)
        {
            var next = GetRandom(random);
            _transitions.Remove(next);
            _probabilities = null;
            return next;
        }
        
        private Dictionary<Relationship, double> ComputeProbabilities()
        {
            double count = _transitions.Sum(t => t.Value);
            return _transitions.ToDictionary(t => t.Key, t => t.Value / count);
        }

        public int Count() => _transitions.Count();

        public bool Any() => _transitions.Any();
    }
}
