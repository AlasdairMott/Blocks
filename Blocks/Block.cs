﻿using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocks
{
	public class Block
	{
		private RelationshipComparer _comparer = new RelationshipComparer();
		private HashSet<Relationship> _relationships;

		public InstanceDefinition Definition { get; set; }
		public IEnumerable<Relationship> Relationships => _relationships;

		public Block(InstanceDefinition definition)
		{
			Definition = definition;
			_relationships =  new HashSet<Relationship>(_comparer);
		}

		public void AddRelationships(IEnumerable<Relationship> relationships)
		{
			_relationships.UnionWith(relationships);
		}

		public void AddRelationship(Relationship relationship)
		{
			_relationships.Add(relationship);
		}

		public Relationship Next(Random random)
		{
			if (_relationships.Count() == 0) { throw new Exception("No relationships to choose from"); }
			var value = random.NextDouble();
			var shuffled = _relationships.OrderBy(r => random.NextDouble());
			var result = shuffled.FirstOrDefault(r => r.Strength > value);
			if (result != null) { return result; }
			return shuffled.First();
		}

		public void NormalizeRelationships()
		{
			var mass = _relationships.Sum(r => r.Strength);
			foreach (var relationship in _relationships)
			{
				relationship.Strength /= mass;
			}
		}
	}
}
