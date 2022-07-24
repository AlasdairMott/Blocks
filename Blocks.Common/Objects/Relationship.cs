using Rhino.Geometry;
using System;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// The transform between two BlockDefinitions.
    /// </summary>
    public class Relationship: ICloneable
	{
		public BlockDefinition From { get; set; }
        public BlockDefinition To { get; set; }
		public Transform Transform { get; set; }
        public Transform Inverse { get; set; }

		public Relationship(BlockDefinition from, BlockDefinition to, Transform transform, Transform inverse)
		{
			From = from;
            To = to;
            Transform = transform;
            Inverse = inverse;
        }

        public Relationship(BlockInstance from, BlockInstance to)
        {
            var T = CalculateRelativeTransform(from.Transform, to.Transform);
            From = from.BlockDefinition;
            To = to.BlockDefinition;
            Transform = T.Transform;
            Inverse = T.Inverse;
        }

        public static (Transform Transform, Transform Inverse) CalculateRelativeTransform(Transform from, Transform to)
        {
            from.TryGetInverse(out var xformInverse);

            var transform = xformInverse * to;
            transform.TryGetInverse(out var inverse);

            return (transform, inverse);
        }

        public Relationship(Relationship relationship) : this(relationship.From, relationship.To, relationship.Transform, relationship.Inverse) { }

        public Relationship Invert() => new Relationship(To, From, Inverse, Transform);

        public Relationship Clone() => new Relationship(this);

        object ICloneable.Clone() => Clone();
    }
}
