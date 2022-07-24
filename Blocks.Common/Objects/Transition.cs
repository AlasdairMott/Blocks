using Rhino.Geometry;
using System;

namespace Blocks.Common.Objects
{
    /// <summary>
    /// The transform between two BlockDefinitions.
    /// </summary>
    public class Transition: ICloneable
	{
		public BlockDefinition From { get; set; }
        public BlockDefinition To { get; set; }
		public Transform Transform { get; set; }
        public Transform Inverse { get; set; }

		public Transition(BlockDefinition from, BlockDefinition to, Transform transform, Transform inverse)
		{
			From = from;
            To = to;
            Transform = transform;
            Inverse = inverse;
        }

        public Transition(BlockInstance from, BlockInstance to)
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

        public Transition(Transition transition) : this(transition.From, transition.To, transition.Transform, transition.Inverse) { }

        public Transition Invert() => new Transition(To, From, Inverse, Transform);

        public Transition Clone() => new Transition(this);

        object ICloneable.Clone() => Clone();
    }
}
