using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// A collection of BlockDefinitions that can be chosen from when generating a BlockAssembly.
    /// </summary>
    public class Transitions : IEnumerable<Transition>
    {
        private readonly Dictionary<Transition, int> _transitions;
        private Dictionary<Transition, double> _probabilities;
  
        private TransitionComparer _comparer = new TransitionComparer();
        public IReadOnlyDictionary<Transition, int> Counts => _transitions;
        public IReadOnlyDictionary<Transition, double> Probabilities
        {
            get => _probabilities ?? (_probabilities = ComputeProbabilities());
        }

        public Transitions()
        {
            _transitions = new Dictionary<Transition, int>(_comparer);
        }

        public Transitions(IEnumerable<Transition> transitions):this()
        {
            foreach (var transition in transitions) { Push(transition); }
        }

        public Transitions(BlockAssembly assembly) : 
            this (assembly.Edges.Select(r => new Transition(r)).ToList()){
        }

        public Transition this[int index]
        {
            get => _transitions.Keys.ElementAt(index);
        }

        public IEnumerator<Transition> GetEnumerator() => _transitions.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Transition GetRandom(Random random)
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

        public void Push(Transition transition) {
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

        public Transition Pop(Random random)
        {
            var next = GetRandom(random);
            _transitions.Remove(next);
            _probabilities = null;
            return next;
        }
        
        private Dictionary<Transition, double> ComputeProbabilities()
        {
            double count = _transitions.Sum(t => t.Value);
            return _transitions.ToDictionary(t => t.Key, t => t.Value / count);
        }

        public int Count() => _transitions.Count();

        public bool Any() => _transitions.Any();
    }
}
