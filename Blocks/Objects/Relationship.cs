using Rhino.Geometry;

namespace Blocks.Objects
{
    /// <summary>
    /// The transform between two BlockDefinitions.
    /// </summary>
    public class Relationship
	{
		public BlockInstance From { get; set; }
        public BlockInstance To { get; set; }
		public Transform Transform { get; set; }
        public Transform Inverse { get; set; }

		public Relationship(BlockInstance from, BlockInstance to)
		{
			From = from;
            To = to;

            var transform = CalculateRelativeTransform(from, to);
            Transform = transform.Transform;
            Inverse = transform.Inverse;
        }

        public static (Transform Transform, Transform Inverse) CalculateRelativeTransform(BlockInstance from, BlockInstance to)
        {
            from.Transform.TryGetInverse(out var xformInverse);

            var transform = xformInverse * to.Transform;
            transform.TryGetInverse(out var inverse);

            return (transform, inverse);
        }

        public Relationship Invert() => new Relationship(To, From);
    }
}
