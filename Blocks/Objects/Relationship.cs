using Rhino.Geometry;

namespace Blocks.Objects
{
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
