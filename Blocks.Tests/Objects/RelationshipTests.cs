using Blocks.Objects;
using Blocks.Tests.Helpers;
using Rhino.Geometry;
using Rhino.DocObjects;
using Xunit;
using Xunit.Abstractions;

namespace Blocks.Tests.Objects
{
    [Collection("RhinoTestingCollection")]
    public class RelationshipTests
    {
        private readonly ITestOutputHelper _output;

        public RelationshipTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CalculateRelativeTransform_IsSuccessful()
        {
            //Arrange
            var plane1 = new Plane(new Point3d(0.1, 45, 5), new Vector3d(2.2, -4.4, 1));
            var plane2 = new Plane(new Point3d(-3.2, 5.1, 5.1), new Vector3d(-0.5, 1.2, -1.1));

            var transform1 = Transform.PlaneToPlane(plane1, Plane.WorldXY);
            var transform2 = Transform.PlaneToPlane(plane2, Plane.WorldXY);

            var point1 = plane1.Origin;
            var point2 = plane2.Origin;

            //Act
            var result = Relationship.CalculateRelativeTransform(transform1, transform2);

            point1.Transform(result.Inverse);
            point2.Transform(result.Transform);

            //Assert
            var tolerance = 0.01;
            Assert.InRange(point1.DistanceTo(plane2.Origin), 0, tolerance);
            Assert.InRange(point2.DistanceTo(plane1.Origin), 0, tolerance);
        }

        [Fact]
        public void InvertRelationship_ItInverts()
        {
            //Arrange
            var transition = Stubs.TestingStubs.Relationships["A1-B1"];
            var point1 = Point3d.Origin;
            var point2 = Point3d.Origin;
            var point3 = Point3d.Origin;

            //Act
            var inverse = transition.Invert();
            point1.Transform(transition.Transform * inverse.Transform);
            point2.Transform(transition.Inverse);
            point3.Transform(inverse.Transform);

            //Assert
            var tolerance = 0.01;
            Assert.InRange(point1.DistanceTo(Point3d.Origin), 0, tolerance);
            Assert.InRange(point2.DistanceTo(point3), 0, tolerance);
        }
    }
}
