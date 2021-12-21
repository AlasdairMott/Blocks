using Blocks.Objects;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Blocks.Readers
{
    /// <summary>
    /// Read transitions from a Rhino file.
    /// </summary>
    public class ReadTransitions
    {
        public ReadTransitions()
        {
        }

        /// <summary>
        /// Read a InstanceObject in a Rhino file as a list of Transitions.
        /// </summary>
        /// <param name="instances">InstanceObjects to read.</param>
        /// <param name="distanceThreshold">The distance threshold to consider InstanceObjects neighbours.</param>
        /// <returns>Converts the InstanceObjects into BlockDefinitions and bundles them into a list of Transitions.</returns>
		public Transitions Read(List<InstanceObject> instances, double distanceThreshold)
		{
			var blocks = new Dictionary<InstanceDefinition, BlockDefinition>();
            var transitions = new Transitions();

			//for each block, read it's closest neighbours, build a list of connections and transforms
			for (var i = 0; i < instances.Count; i++)
			{
				var instance = instances[i];

				for (var j = i + 1; j < instances.Count; j++)
				{
					var other = instances[j];

					if (other.InsertionPoint.DistanceTo(instance.InsertionPoint) > distanceThreshold ||
						!Functions.CollisionCheck.CheckCollision(instance, other))
                    {
						continue;
                    }

                    var transition = new Transition(instance.ToInstance(), other.ToInstance());
                    transitions.Add(transition);
                }
			}

            transitions.NormalizeRelationships();

            return transitions;
		}
	}
}
