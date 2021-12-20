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

            var T = CalculateRelativeTransform(from.Transform, to.Transform);
            Transform = T.Transform;
            Inverse = T.Inverse;
        }

        public static (Transform Transform, Transform Inverse) CalculateRelativeTransform(Transform from, Transform to)
        {
            var plane1 = Plane.WorldXY;
            plane1.Transform(from);

            var plane2 = Plane.WorldXY;
            plane2.Transform(to);

            return (Transform.PlaneToPlane(plane1, plane2), Transform.PlaneToPlane(plane2, plane1));


            //from.TryGetInverse(out var xformInverse);

            //var transform = xformInverse * to;
            //transform.TryGetInverse(out var inverse);

            //return (transform, inverse);
        }

        public Relationship Invert() => new Relationship(To, From);
    }
}
