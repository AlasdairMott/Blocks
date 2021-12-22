using Rhino.Geometry;

namespace Blocks.Objects
{
    /// <summary>
    /// The transform between two BlockDefinitions.
    /// </summary>
    public class Relationship
	{
		public BlockDefinition Definition { get; set; }
		public Transform Transform { get; set; }
		public double Strength { get; set; }

		public Relationship(BlockDefinition instanceDefinition, Transform transform)
		{
			Definition = instanceDefinition;
			Transform = transform;
		}
	}
}
