using Blocks.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Solvers
{
    /// <summary>
    /// Read a block assembly from a Rhino file.
    /// </summary>
    public class ReadBlockAssembly
    {
        public ReadBlockAssembly()
        {
        }

        /// <summary>
        /// Read a InstanceObject in a Rhino file as a BlockPool.
        /// </summary>
        /// <param name="instances">InstanceObjects to read.</param>
        /// <param name="distanceThreshold">The distance threshold to consider InstanceObjects neighbours.</param>
        /// <returns>Converts the InstanceObjects into BlockDefinitions and bundles them in a BlockPool.</returns>
		public BlockPool Read(List<InstanceObject> instances, double distanceThreshold)
		{
			var comparer = new RelationshipComparer();
			var blocks = new Dictionary<InstanceDefinition, BlockDefinition>();

			//for each block, read it's closest neighbours, build a list of connections and transforms
			for (var i = 0; i < instances.Count; i++)
			{
				var instance = instances[i];
				var blockA = AddNewBlock(instance, blocks);

				for (var j = i + 1; j < instances.Count; j++)
				{
					var other = instances[j];
					var blockB = AddNewBlock(other, blocks);

					if (other.Id == instance.Id ||
						other.InsertionPoint.DistanceTo(instance.InsertionPoint) > distanceThreshold ||
						!Functions.CollisionCheck.CheckCollision(instance, other))
                    {
						continue;
                    }

					var transform = CalculateRelativeTransform(instance, other);

					AddNewRelationship(other, transform.Transform, blockA);
					AddNewRelationship(instance, transform.Inverse, blockB);
				}
			}

			foreach (var block in blocks)
			{
				block.Value.NormalizeRelationships();
			}

			return new BlockPool(blocks.Select(b => b.Value));
		}

		private BlockDefinition AddNewBlock(InstanceObject instance, Dictionary<InstanceDefinition, BlockDefinition> blocks)
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
			return block;
		}
		private (Transform Transform, Transform Inverse) CalculateRelativeTransform(InstanceObject instance, InstanceObject other)
        {
			var xform1 = instance.InstanceXform;
			var xform2 = other.InstanceXform;

			xform1.TryGetInverse(out var xformInverse);

			var transform = xformInverse * xform2;
			transform.TryGetInverse(out var inverse);

			return (transform, inverse);
		}

		private void AddNewRelationship(InstanceObject instance, Transform transform, BlockDefinition blockDefinition)
        {
			var key = new Relationship(instance.InstanceDefinition.ToDefinition(), transform);

            //If the relationship already exists in the blockDefinition's relationships, increase its strength.
            if (blockDefinition.Relationships.Contains(key))
			{
				var existing = blockDefinition.FindRelationship(key);
				existing.Strength += 1;
			}
			else
			{
				key.Strength = 1;
				blockDefinition.AddRelationship(key);
			}
		}
	}
}
