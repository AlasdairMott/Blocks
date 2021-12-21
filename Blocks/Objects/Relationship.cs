﻿using Rhino.Geometry;

namespace Blocks.Objects
{
    /// <summary>
    /// The transform between two BlockDefinitions.
    /// </summary>
    public class Relationship
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
            //var plane1 = Plane.WorldXY;
            //plane1.Transform(from);

            //var plane2 = Plane.WorldXY;
            //plane2.Transform(to);

            //return (Transform.PlaneToPlane(plane1, plane2), Transform.PlaneToPlane(plane2, plane1));

            from.TryGetInverse(out var xformInverse);

            var transform = xformInverse * to;
            transform.TryGetInverse(out var inverse);

            return (transform, inverse);
        }

        public Relationship Invert() => new Relationship(To, From, Inverse, Transform);
    }
}
