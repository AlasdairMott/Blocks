using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Solvers
{
    public class ReadBlockAssembly
    {
        public ReadBlockAssembly()
        {
        }

		public BlockPool LearnRelationships(IEnumerable<InstanceObject> instances, double distanceThreshold)
		{
			//need to make a tranform comparer
			var comparer = new RelationshipComparer();
			var blocks = new Dictionary<InstanceDefinition, BlockDefinition>();

			//for each block, read it's closest neighbours, build a list of connections and transforms
			foreach (var instance in instances)
			{
				BlockDefinition block;
				if (blocks.ContainsKey(instance.InstanceDefinition))
				{
					block = blocks[instance.InstanceDefinition];
				}
				else
				{
					block = new BlockDefinition(instance.InstanceDefinition);
					blocks.Add(instance.InstanceDefinition, block);
				}

				foreach (var other in instances)
				{
					if (other.Id == instance.Id) { continue; }
					if (other.InsertionPoint.DistanceTo(instance.InsertionPoint) > distanceThreshold) { continue; }
					if (!Functions.CollisionCheck.CheckCollision(instance, other)) { continue; }

					var xform1 = instance.InstanceXform;
					var xform2 = other.InstanceXform;

					var plane1 = Plane.WorldXY;
					plane1.Transform(xform1);

					var plane2 = Plane.WorldXY;
					plane2.Transform(xform2);

					xform1.TryGetInverse(out var xformInverse);

					var transform = xformInverse * xform2;

					var key = new Relationship(other.InstanceDefinition.ToDefinition(), transform);
					if (block.Relationships.Contains(key))
					{
						var existing = block.FindRelationship(key);
						existing.Strength += 1;
					}
					else
					{
						key.Strength = 1;
						block.AddRelationship(key);
					}
				}
			}

			foreach (var block in blocks)
			{
				block.Value.NormalizeRelationships();
			}

			return new BlockPool(blocks.Select(b => b.Value));
		}
	}
}
